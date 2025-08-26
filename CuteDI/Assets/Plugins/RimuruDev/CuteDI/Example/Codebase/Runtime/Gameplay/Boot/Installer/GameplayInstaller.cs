using UnityEngine;
using System.Collections.Generic;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(fileName = "GameplayInstaller", menuName = "DI/Scene Installers/Gameplay")]
    public sealed class GameplayInstaller : SceneInstallerSO
    {
        [SerializeField] private GameObject hudPrefab;

        public override void Compose(in DIContainer scene, in DIContainer project)
        {
            scene.RegisterInstance(this).AsSingle().NonLazy();
            scene.RegisterType<IEnemySpawner, EnemySpawner>().AsSingle().NonLazy();

            var gameNavigation = project.Resolve<IGameNavigation>();
            scene.Register(c => new GameplayController(c.Resolve<IEnemySpawner>(), gameNavigation)).AsSingle().NonLazy();

            if (hudPrefab)
                scene.BindPrefabSelf<HUD>(hudPrefab);
        }

#if UNITY_EDITOR
        public override IEnumerable<BindingHint> PreviewHints()
        {
            yield return new BindingHint(typeof(GameplayInstaller), typeof(GameplayInstaller), null, true);
            yield return new BindingHint(typeof(IEnemySpawner), typeof(EnemySpawner), null, true);
            yield return new BindingHint(typeof(GameplayController), typeof(GameplayController), null, true);

            if (hudPrefab)
                yield return new BindingHint(typeof(HUD), typeof(HUD), null, true);
        }
#endif
    }
}