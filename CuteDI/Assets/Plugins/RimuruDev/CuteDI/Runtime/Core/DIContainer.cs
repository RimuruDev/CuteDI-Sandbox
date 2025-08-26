using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    public class DIContainer : IDisposable
    {
        public readonly string ContainerName;
        
        private readonly DIContainer parent;

        private readonly Dictionary<(string, Type), DIEntry> map = new();
        private readonly HashSet<(string, Type)> resolving = new();

        [Preserve]
        public DIContainer(DIContainer parentContainer = null, string containerName = null)
        {
            ContainerName = containerName;
            parent = parentContainer;
        }

        public RegistrationBuilder<T> Register<T>(Func<DIContainer, T> factory, string tag = null)
        {
            var key = (tag, typeof(T));
            if (map.ContainsKey(key))
                throw new Exception(
                    $"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} already registered");
            var entry = new DIEntry<T>(this, factory);
            map[key] = entry;
            return new RegistrationBuilder<T>(this, key);
        }

        public RegistrationBuilder<TService> RegisterType<TService, TImpl>(string tag = null) where TImpl : TService
        {
            return Register(c => (TService)DiActivator.CreateInstance(typeof(TImpl), c), tag);
        }

        public RegistrationBuilder<object> RegisterType(Type service, Type impl, string tag = null)
        {
            if (!service.IsAssignableFrom(impl))
                throw new ArgumentException($"{impl.FullName} is not assignable to {service.FullName}");
            var key = (tag, service);
            if (map.ContainsKey(key))
                throw new Exception(
                    $"DI: Type with tag {key.Item1} and type {key.Item2.FullName} already registered");
            map[key] = new DIEntry<object>(this, c => DiActivator.CreateInstance(impl, c));
            return new RegistrationBuilder<object>(this, key);
        }

        public RegistrationBuilder<T> RegisterInstance<T>(T instance, string tag = null)
        {
            var key = (tag, typeof(T));
            if (map.ContainsKey(key))
                throw new Exception(
                    $"DI: Instance with tag {key.Item1} and type {key.Item2.FullName} already registered");
            map[key] = new DIEntry<T>(instance);
            return new RegistrationBuilder<T>(this, key);
        }

        public TService Resolve<TService>(string tag = null)
        {
            if (typeof(TService) == typeof(DIContainer) && tag == null) return (TService)(object)this;
            var key = (tag, typeof(TService));
            if (resolving.Contains(key))
                throw new Exception($"DI: Cyclic dependency for tag {key.Item1} and type {key.Item2.FullName}");
            resolving.Add(key);
            try
            {
                if (map.TryGetValue(key, out var entry)) return entry.Resolve<TService>();
                if (parent != null) return parent.Resolve<TService>(tag);
            }
            finally
            {
                resolving.Remove(key);
            }

            throw new Exception($"Couldn't find dependency for tag {tag} and type {key.Item2.FullName}");
        }

        public object Resolve(Type type, string tag = null)
        {
            if (type == typeof(DIContainer) && tag == null) return this;
            var key = (tag, type);
            if (resolving.Contains(key))
                throw new Exception($"DI: Cyclic dependency for tag {key.Item1} and type {key.Item2.FullName}");
            resolving.Add(key);
            try
            {
                if (map.TryGetValue(key, out var entry)) return entry.ResolveObject();
                if (parent != null) return parent.Resolve(type, tag);
            }
            finally
            {
                resolving.Remove(key);
            }

            throw new Exception($"Couldn't find dependency for tag {tag} and type {key.Item2.FullName}");
        }

        public IEnumerable<(string Tag, Type ServiceType)> GetRegistrations(bool includeParents = false)
        {
            foreach (var kv in map.Keys)
                yield return (kv.Item1, kv.Item2);
            
            if (!includeParents || parent == null) 
                yield break;
            
            foreach (var r in parent.GetRegistrations(true)) 
                yield return r;
        }

        public IEnumerable<(string Tag, Type ServiceType)> FindRegistrations(
            Func<string, Type, bool> predicate,
            bool includeParents = false)
        {
            foreach (var (tag, type) in GetRegistrations(includeParents))
                if (predicate(tag, type))
                    yield return (tag, type);
        }

        public IReadOnlyList<TBase> ResolveAll<TBase>(bool includeParents = true)
        {
            var list = new List<TBase>();
            foreach (var (tag, type) in GetRegistrations(includeParents))
            {
                if (!typeof(TBase).IsAssignableFrom(type)) continue;
                var v = Resolve(type, tag);
                list.Add((TBase)v);
            }

            return list;
        }

        public bool TryResolve<TService>(out TService value, string tag = null)
        {
            try
            {
                value = Resolve<TService>(tag);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public bool HasRegistration<TService>(string tag = null)
        {
            var key = (tag, typeof(TService));
          
            if (map.ContainsKey(key))
                return true;
           
            return parent != null && parent.HasRegistration<TService>(tag);
        }

        public RegistrationBuilder<T> Replace<T>(Func<DIContainer, T> factory, string tag = null)
        {
            var key = (tag, typeof(T));
           
            map[key] = new DIEntry<T>(this, factory);
          
            return new RegistrationBuilder<T>(this, key);
        }
        
        public void Dispose()
        {
            foreach (var e in map.Values) e.Dispose();
            map.Clear();
        }

        public readonly struct RegistrationBuilder<T>
        {
            private readonly DIContainer container;
            private readonly (string, Type) key;

            public RegistrationBuilder(DIContainer container, (string, Type) key)
            {
                this.container = container;
                this.key = key;
            }

            public RegistrationBuilder<T> AsSingle()
            {
                var e = (DIEntry<T>)container.map[key];
                e.IsSingleton = true;
                return this;
            }

            public void NonLazy()
            {
                container.Resolve(key.Item2, key.Item1);
            }
        }

        public override string ToString()
        {
            return ContainerName;
        }

#if UNITY_EDITOR // TODO: Add partial layer
        public IEnumerable<string> DebugEntries() => 
            map.Keys.Select(key => $"[{key.Item1 ?? "no-tag"}] {key.Item2.Name}");
        
        public IEnumerable<RegistrationInfo> DebugSnapshot(bool includeParents = false)
        {
            foreach (var kv in map)
            {
                var tag = kv.Key.Item1;
                var type = kv.Key.Item2;
                var single = kv.Value.IsSingleton;
                yield return new RegistrationInfo(tag, type, single);
            }

            if (!includeParents || parent == null) yield break;

            foreach (var r in parent.DebugSnapshot(true)) yield return r;
        }
        
        public IEnumerable<DebugEntry> DebugLayers()
        {
            var d = 0;
            for (var c = this; c != null; c = c.parent, d++)
            {
                foreach (var kv in c.map)
                    yield return new DebugEntry(kv.Key.Item1, kv.Key.Item2, kv.Value.IsSingleton, d, c.ContainerName);
            }
        }
#endif

    }

    [Preserve]
    public abstract class DIEntry : IDisposable
    {
        public bool IsSingleton { get; set; }
        public abstract TService Resolve<TService>();
        public abstract object ResolveObject();
        public abstract void Dispose();
    }

    [Preserve]
    public sealed class DIEntry<T> : DIEntry
    {
        private readonly DIContainer container;
        private readonly Func<DIContainer, T> factory;
        private readonly bool isInstance;
        private T instance;
        private IDisposable disposable;

        public DIEntry(DIContainer container, Func<DIContainer, T> factory)
        {
            this.container = container;
            this.factory = factory;
        }

        public DIEntry(T value)
        {
            instance = value;
            if (instance is IDisposable d) disposable = d;
            IsSingleton = true;
            isInstance = true;
        }

        public override TService Resolve<TService>()
        {
            var val = IsSingleton ? GetOrReturn() : factory(container);
            return (TService)(object)val;
        }

        public override object ResolveObject()
        {
            return IsSingleton ? GetOrReturn() : factory(container);
        }

        public override void Dispose()
        {
            disposable?.Dispose();
            instance = default;
            disposable = null;
        }

        private T GetOrReturn()
        {
            if (isInstance) return instance;
            if (IsUnityNull(instance))
            {
                instance = factory(container);
                if (instance is IDisposable d) disposable = d;
            }

            return instance;
        }

        private static bool IsUnityNull(object obj)
        {
            if (obj is UnityEngine.Object uo) return uo == null;
            return obj == null;
        }
    }
}