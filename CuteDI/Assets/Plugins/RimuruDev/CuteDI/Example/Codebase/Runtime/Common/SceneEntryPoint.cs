using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    [DisallowMultipleComponent]
    public abstract class SceneEntryPoint : MonoBehaviour
    {
        private void Awake() => PreInitialize();

        private void Start() => Initialize();

        private void OnDestroy() => Finalization();

        /// <summary>MonoBehaviour.Awake() callback</summary>
        protected virtual void PreInitialize() { }
        
        /// <summary>MonoBehaviour.Start() callback</summary>
        protected virtual void Initialize() { }
        
        /// <summary>MonoBehaviour.OnDestroy() callback</summary>
        protected virtual void Finalization() { }
    }
}