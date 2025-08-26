using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    [DefaultExecutionOrder(-10000)]
    public sealed class SceneContext : MonoBehaviour
    {
        [field: SerializeField] public string SceneNameOverride { get; private set; }
        [field: SerializeField] public List<SceneInstallerSO> Installers { get; private set; } = new();

        private List<SceneInstallerSO> installers;
        
        private DIContainer scene;
        private bool built;

        private void Awake()
        {
            installers = new List<SceneInstallerSO>(Installers);
            
            var sceneName = string.IsNullOrWhiteSpace(SceneNameOverride)
                ? gameObject.scene.name
                : SceneNameOverride;

            scene = DiProvider.SceneContextBuilder(DiProvider.Project, sceneName, $"{sceneName} Context");

            foreach (var installerSo in installers.Where(t => t != null))
                installerSo.Compose(scene, DiProvider.Project);

            built = true;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(SceneNameOverride))
            {
                // NOTE: Если это префаб (в Prefab Mode) — выходим. Все же имя сцены в префаб моде это имя объекта
                // А мне хочется что бы имя контейнера в противном случае было имя сцены.
                // Как по мне это идеально. Хотя тулы для отладки я еще нормальные не написал. Но даже с дебаг-логами нормуль выходит.
                if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null)
                    return;

                SceneNameOverride = gameObject.scene.name;
                
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }


        [Preserve] public DIContainer Scene => scene;

        [Preserve] public bool Built => built;
    }
}