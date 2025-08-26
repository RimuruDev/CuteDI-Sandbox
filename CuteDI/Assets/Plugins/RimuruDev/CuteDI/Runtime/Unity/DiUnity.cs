using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    public static class DiUnity
    {
        [Preserve]
        public static TInterface BindNewGo<TInterface, TImpl>(
            this DIContainer container,
            string name = null,
            bool dontDestroyOnLoad = false,
            string tag = null)
            where TImpl : MonoBehaviour, TInterface
        {
            var go = new GameObject(string.IsNullOrEmpty(name)
                ? typeof(TImpl).Name
                : name);

            if (dontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(go);

            var comp = go.AddComponent<TImpl>();

            RegisterMono(container, comp, tag);
            container.RegisterInstance<TInterface>(comp, tag);

            return comp;
        }
        
        [Preserve]
        public static TInterface BindOnGo<TInterface, TImpl>(
            this DIContainer container,
            TImpl instance,
            string tag = null)
            where TImpl : MonoBehaviour, TInterface
        {
            if (!instance || !instance.gameObject.scene.IsValid())
                throw new InvalidOperationException($"BindOnGo<{typeof(TInterface).Name},{typeof(TImpl).Name}> expects a scene instance. Use BindPrefab for assets.");
    
            RegisterMono(container, instance, tag);
            container.RegisterInstance<TInterface>(instance, tag);
          
            return instance;
        }


        // TODO: сделать отдельно для UI просто ибо isUI не оч
        [Preserve]
        public static TInterface BindPrefab<TInterface, TImpl>(
            this DIContainer container,
            GameObject prefab,
            Transform parent = null,
            Vector3? position = null,
            Quaternion? rotation = null,
            string name = null,
            string tag = null,
            bool dontDestroyOnLoad = false,
            bool isUI = false)
            where TImpl : MonoBehaviour, TInterface
        {
            var pos = position ?? Vector3.zero;
            var rot = rotation ?? Quaternion.identity;

            GameObject go;

            if (isUI)
                go = UnityEngine.Object.Instantiate(prefab, parent, instantiateInWorldSpace: false) as GameObject;
            else
                go = UnityEngine.Object.Instantiate(prefab, pos, rot, parent);

            if (!string.IsNullOrEmpty(name))
                go.name = name;

            if (dontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(go);

            var comp = go.GetComponent<TImpl>();
            if (!comp)
                throw new InvalidOperationException($"{typeof(TImpl).Name} not found on prefab {prefab.name}");

            RegisterMono(container, comp, tag);
            container.RegisterInstance<TInterface>(comp, tag);

            return comp;
        }

        [Preserve]
        public static TImpl BindNewGoSelf<TImpl>(
            this DIContainer container,
            string name = null,
            bool dontDestroyOnLoad = false,
            string tag = null)
            where TImpl : MonoBehaviour
        {
            var go = new GameObject(string.IsNullOrEmpty(name)
                ? typeof(TImpl).Name
                : name);

            if (dontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(go);

            var comp = go.AddComponent<TImpl>();
            RegisterMono(container, comp, tag);

            return comp;
        }

        [Preserve]
        public static TImpl BindOnGoSelf<TImpl>(
            this DIContainer container,
            TImpl instance,
            string tag = null)
            where TImpl : MonoBehaviour
        {
            RegisterMono(container, instance, tag);

            return instance;
        }

        [Preserve]
        public static TImpl BindPrefabSelf<TImpl>(
            this DIContainer container,
            GameObject prefab,
            Transform parent = null,
            Vector3? position = null,
            Quaternion? rotation = null,
            string name = null,
            string tag = null,
            bool dontDestroyOnLoad = false)
            where TImpl : MonoBehaviour
        {
            var pos = position ?? Vector3.zero;
            var rot = rotation ?? Quaternion.identity;

            var go = UnityEngine.Object.Instantiate(prefab, pos, rot, parent);

            if (!string.IsNullOrEmpty(name))
                go.name = name;

            if (dontDestroyOnLoad)
                UnityEngine.Object.DontDestroyOnLoad(go);

            var comp = go.GetComponent<TImpl>();

            if (!comp)
                throw new InvalidOperationException($"{typeof(TImpl).Name} not found on prefab {prefab.name}");

            RegisterMono(container, comp, tag);

            return comp;
        }

        [Preserve]
        public static void DisposeBoundMono<TImpl>(
            this DIContainer container,
            string tag = null)
            where TImpl : MonoBehaviour
        {
            var handle = container.Resolve<MonoBehaviourHandle<TImpl>>(tag);
            handle.Dispose();
        }

        [Preserve]
        private static void RegisterMono<TImpl>(
            DIContainer container,
            TImpl instance,
            string tag)
            where TImpl : MonoBehaviour
        {
            container.RegisterInstance(instance, tag);
            container.RegisterInstance(new MonoBehaviourHandle<TImpl>(instance), tag);
        }
    }
}