using UnityEngine;

namespace AbyssMoth.CuteDI
{
    [DefaultExecutionOrder(-10001)]
    public sealed class SceneContextBootstrap : MonoBehaviour
    {
        [SerializeField] private SceneContext context;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null)
                return;
            
            if (!context)
            {
                context = FindFirstObjectByType<SceneContext>();

                if (context)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }
        }
#endif

        private void Awake()
        {
            if (!context)
                context = FindFirstObjectByType<SceneContext>();

            if (!context)
                gameObject.AddComponent<SceneContext>();
        }
    }
}