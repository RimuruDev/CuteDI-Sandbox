using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public abstract class SceneEntryPoint : MonoBehaviour
    {
        protected DIContainer Scene { get; private set; }
        protected DIContainer Project { get; private set; }

        private void Awake()
        {
            Scene = gameObject.scene.GetSceneContainer();
            Project = gameObject.scene.GetProjectContainer();
            
            Debug.Assert(Scene != null);
            Debug.Assert(Project != null);
            
            PreInitialize();
        }

        private void Start() => 
            Initialize();

        private void OnDestroy() => 
            Finalization();

        protected virtual void PreInitialize() { }
        protected virtual void Initialize() { }
        protected virtual void Finalization() { }
    }
}