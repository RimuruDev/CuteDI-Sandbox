using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(fileName = "MainMenuInstaller", menuName = "DI/Scene Installers/MainMenu")]
    public sealed class MainMenuInstaller : SceneInstallerSO
    {
        public override void Compose(in DIContainer scene, in DIContainer project)
        {
            
        }
    }
}