using System;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
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

            if (instance is IDisposable d)
                disposable = d;

            IsSingleton = true;
            isInstance = true;
        }

        public override TService Resolve<TService>()
        {
            var val = IsSingleton
                ? GetOrReturn()
                : factory(container);

            return (TService)(object)val;
        }

        public override object ResolveObject() =>
            IsSingleton
                ? GetOrReturn()
                : factory(container);

        public override void Dispose()
        {
            disposable?.Dispose();
            instance = default;
            disposable = null;
        }

        private T GetOrReturn()
        {
            if (isInstance)
                return instance;

            if (IsUnityNull(instance))
            {
                instance = factory(container);
                if (instance is IDisposable d) disposable = d;
            }

            return instance;
        }

        private static bool IsUnityNull(object obj)
        {
            if (obj is UnityEngine.Object uo)
                return uo == null;

            return obj == null;
        }
    }
}