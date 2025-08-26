using UnityEngine;
using System.Collections.Generic;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(fileName = "MainMenuInstaller", menuName = "DI/Scene Installers/MainMenu")]
    public sealed class MainMenuInstaller : SceneInstallerSO
    {
        [SerializeField] private GameObject menuRootPrefab;

        public override void Compose(in DIContainer scene, in DIContainer project)
        {
            scene.RegisterInstance(this).AsSingle().NonLazy();
            scene.RegisterType<IMenuService, MenuService>().AsSingle().NonLazy();
            scene.Register(c => new MenuViewModel(c.Resolve<IMenuService>())).AsSingle().NonLazy();
         
            if (menuRootPrefab)
                scene.BindPrefab<IMenuRoot, MenuRoot>(menuRootPrefab, isUI: true);
        }
        
#if UNITY_EDITOR
        public override IEnumerable<BindingHint> PreviewHints()
        {
            yield return new BindingHint(typeof(MainMenuInstaller), typeof(MainMenuInstaller), null, true);
            yield return new BindingHint(typeof(IMenuService), typeof(MenuService), null, true);
            yield return new BindingHint(typeof(MenuViewModel), typeof(MenuViewModel), null, true);

            if (menuRootPrefab)
                yield return new BindingHint(typeof(IMenuRoot), typeof(MenuRoot), null, true);
        }
#endif
    }
}