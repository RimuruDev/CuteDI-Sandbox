using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    public static class DiProvider
    {
        [Preserve] public static DIContainer Project { get; private set; }
        [Preserve] private static readonly Dictionary<int, DIContainer> scenes = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
        private static void Hook()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        [Preserve]
        public static DIContainer SetProject(DIContainer container) => Project = container;

        [Preserve]
        public static void SetScene(Scene scene, DIContainer container)
        {
            // NOTE: обновляем кеш, по этому без Contains.
            // Так на практиве вышло лучше. Но обязательно нужно будет отдельно подумаль как можно улучшить 
            // Ну написав 2 игры проблем не возникло, мб и так сойдет. Главное не забыть залогировать по человечески а не вот таким дебаг логам :D
            scenes[scene.handle] = container;
            Debug.Log($"<color=green>DiLocator::SetScene(scene:{scene.name}, container:{container});</color>");
        }


        [Preserve]
        public static DIContainer GetScene(Scene scene) =>
            scenes.GetValueOrDefault(key: scene.handle, defaultValue: Project);

        [Preserve]
        public static DIContainer GetSceneContainerBySceneName(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            
            if (!scene.IsValid())
            {
                Debug.Log($"GetSceneContainerBySceneName: IsValid -> null sceneName: {sceneName}");
                return null;
            }

            if (scenes.TryGetValue(scene.handle, out var container))
            {
                return container;
            }

            return null;
        }


        #region Utils

        // TODO: Move in Utils! Dependency -> class App.cs
        [Preserve]
        public static DIContainer GetCurrentSceneContainer()
        {
            // TODO: Add App utils
            var scene = SceneManager.GetActiveScene();
            Debug.Log($"App.CurrentScene: {scene.name}");
            return GetScene(scene);
        }
        
        [Preserve]
        public static DIContainer SceneContextBuilder(
            DIContainer parent,
            string sceneName,
            string containerName = null)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                containerName = $"{sceneName} Context";

            var cache = GetSceneContainerBySceneName(sceneName);
            if (cache != null)
            {
                //Debug.Log($"Return cache: {cache}");
                return cache;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            
            if (!scene.IsValid())
                Debug.LogWarning($"SceneContextBuilder: scene '{sceneName}' is not valid yet. Did you call it before scene loaded?");

            var sceneContainer = new DIContainer(parent, containerName);
            SetScene(scene, sceneContainer);
            
            return sceneContainer;
        }


        #endregion

        [Preserve]
        public static void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            if (Project != null)
            {
                Debug.Log($"<color=green>{Project} Disposed!</color>");
                Project.Dispose();
                Project = null;
            }

            if (scenes.Count > 0)
            {
                foreach (var kv in scenes)
                {
                    var c = kv.Value;
                    if (c != null)
                    {
                        Debug.Log($"<color=green>{c} Disposed!</color>");
                        c.Dispose();
                    }
                }

                scenes.Clear();
            }
        }

        [Preserve]
        private static void OnSceneLoaded(Scene s, LoadSceneMode m) { }

        [Preserve]
        private static void OnSceneUnloaded(Scene s)
        {
            if (!scenes.TryGetValue(s.handle, out var c))
                return;

            if (c != null && c != Project)
                c.Dispose();

            Debug.Log($"<color=green>DiLocator::Unloaded(scene:{s.name}, container:{scenes[s.handle]});</color>");

            scenes.Remove(s.handle);
        }
    }
}