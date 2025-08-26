using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class GameNavigation : IGameNavigation
    {
        private readonly ICoroutineRunner coroutines;

        [InjectionConstructor]
        public GameNavigation(ICoroutineRunner coroutines)
        {
            this.coroutines = coroutines;
        }

        public void GoToMainMenu(Action<DIContainer> onLoaded = null) =>
            coroutines.StartCoroutine(LoadRoutine("MainMenu", onLoaded));

        public void GoToGameplay(Action<DIContainer> onLoaded = null) =>
            coroutines.StartCoroutine(LoadRoutine("Gameplay", onLoaded));

        public void LoadScene(string sceneName, Action<DIContainer> onLoaded = null) =>
            coroutines.StartCoroutine(LoadRoutine(sceneName, onLoaded));

        private static IEnumerator LoadRoutine(string sceneName, Action<DIContainer> onLoaded)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
           
            Debug.Assert(op != null, nameof(op) + " != null");
           
            op.allowSceneActivation = false;
            
            while (op.progress < 0.9f) 
                yield return null;
            
            op.allowSceneActivation = true;
            while (!op.isDone)
                yield return null;

            yield return new WaitUntil(() => DiProvider.GetSceneContainerBySceneName(sceneName) != null);

            var container = DiProvider.GetSceneContainerBySceneName(sceneName);
            onLoaded?.Invoke(container);
        }

    }
}