using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(fileName = "GameplayInstaller", menuName = "DI/Scene Installers/Gameplay")]
    public sealed class GameplayInstaller : SceneInstallerSO
    {
        public override void Compose(in DIContainer scene, in DIContainer project)
        {
        }
    }
}