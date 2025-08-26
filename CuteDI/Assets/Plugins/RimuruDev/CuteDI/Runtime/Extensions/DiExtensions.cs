using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    public static class DiExtensions
    {
        [Preserve]
        public static DIContainer GetSceneContainer(this Scene scene) =>
            DiProvider.GetSceneContainerBySceneName(scene.name);

        [Preserve]
        public static DIContainer GetProjectContainer(this Scene scene) =>
            GetProjectContainerInternal();

        [Preserve]
        private static DIContainer GetProjectContainerInternal() =>
            DiProvider.Project;
    }
}