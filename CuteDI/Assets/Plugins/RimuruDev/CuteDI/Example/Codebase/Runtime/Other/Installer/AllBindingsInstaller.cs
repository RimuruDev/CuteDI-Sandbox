using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(fileName = "AllBindingsInstaller", menuName = "DI/Scene Installers/All Bindings")]
    public sealed class AllBindingsInstaller : SceneInstallerSO
    {
        [SerializeField] private GameObject UiWidgetPrefab;
        [SerializeField] private GameObject HudPrefab;
        [SerializeField] private SampleConfig Config;
        [SerializeField] private GameObject ExistingFooPrefab;

        private Transform uiParent;

        public override void Compose(in DIContainer scene, in DIContainer project)
        {
            scene.RegisterInstance(Config).AsSingle().NonLazy();
            
            uiParent = GameObject.Find("UIParent").transform;

            scene.RegisterInstance(this).AsSingle().NonLazy();

            scene.RegisterType<IEnemySpawner, EnemySpawner>().AsSingle().NonLazy();

            scene.RegisterType<IClock, UtcClock>("utc").AsSingle().NonLazy();
            scene.RegisterType<IClock, GameClock>("game").AsSingle().NonLazy();

            scene.RegisterType<IStorage, MemoryStorage>("mem").AsSingle().NonLazy();
            scene.RegisterType<IStorage, FileStorage>("file").AsSingle().NonLazy();

            scene.RegisterType<IMenuService, MenuService>().AsSingle().NonLazy();
            scene.RegisterType(typeof(IAnalytics), typeof(DummyAnalytics)).AsSingle().NonLazy();

            scene.Register(c => new TaggedConsumer(
                c.Resolve<IStorage>("mem"),
                c.Resolve<IStorage>("file"))).AsSingle().NonLazy();

            scene.RegisterType<AttrConsumer, AttrConsumer>().AsSingle().NonLazy();

            scene.RegisterType<FactoryProduct, FactoryProduct>().AsSingle().NonLazy();

            scene.RegisterType<IProcessor, ProcA>("proc_a").AsSingle().NonLazy();
            scene.RegisterType<IProcessor, ProcB>("proc_b").AsSingle().NonLazy();
            scene.Register(c => new ProcessorAggregator(c.ResolveAll<IProcessor>().ToArray())).AsSingle();

            scene.RegisterType<IReplaceSample, ReplaceA>().AsSingle();
            scene.Replace<IReplaceSample>(c => new ReplaceB()).AsSingle().NonLazy();

            var parent = uiParent
                ? uiParent
                : FindFirstObjectByType<Canvas>()?.transform;

            scene.BindNewGo<IFoo, FooMono>("FooGo", tag: "foo_go");

            if (ExistingFooPrefab)
                scene.BindPrefab<IFoo, FooMono>(ExistingFooPrefab, parent, isUI: true, tag: "foo_existing");

            if (UiWidgetPrefab)
                scene.BindPrefab<IFoo, FooMono>(UiWidgetPrefab, parent, isUI: true, tag: "foo_widget");

            if (HudPrefab)
                scene.BindPrefabSelf<HUD>(HudPrefab, parent);
        }

#if UNITY_EDITOR
        public override IEnumerable<BindingHint> PreviewHints()
        {
            yield return new BindingHint(typeof(AllBindingsInstaller), typeof(AllBindingsInstaller), null, true);

            yield return new BindingHint(typeof(IEnemySpawner), typeof(EnemySpawner), null, true);

            yield return new BindingHint(typeof(IClock), typeof(UtcClock), "utc", true);
            yield return new BindingHint(typeof(IClock), typeof(GameClock), "game", true);

            yield return new BindingHint(typeof(IStorage), typeof(MemoryStorage), "mem", true);
            yield return new BindingHint(typeof(IStorage), typeof(FileStorage), "file", true);

            yield return new BindingHint(typeof(IMenuService), typeof(MenuService), null, true);
            yield return new BindingHint(typeof(IAnalytics), typeof(DummyAnalytics), null, true);

            yield return new BindingHint(typeof(TaggedConsumer), typeof(TaggedConsumer), null, true);
            yield return new BindingHint(typeof(AttrConsumer), typeof(AttrConsumer), null, true);

            yield return new BindingHint(typeof(FactoryProduct), typeof(FactoryProduct), null, true);

            yield return new BindingHint(typeof(IProcessor), typeof(ProcA), "proc_a", true);
            yield return new BindingHint(typeof(IProcessor), typeof(ProcB), "proc_b", true);
            yield return new BindingHint(typeof(ProcessorAggregator), typeof(ProcessorAggregator), null, true);

            yield return new BindingHint(typeof(IReplaceSample), typeof(ReplaceB), null, true);

            if (UiWidgetPrefab)
                yield return new BindingHint(typeof(IFoo), typeof(FooMono), "foo_widget", true);

            yield return new BindingHint(typeof(IFoo), typeof(FooMono), "foo_go", true);

            if (ExistingFooPrefab)
                yield return new BindingHint(typeof(IFoo), typeof(FooMono), "foo_existing", true);

            if (HudPrefab)
                yield return new BindingHint(typeof(HUD), typeof(HUD), null, true);

            if (Config)
                yield return new BindingHint(typeof(SampleConfig), typeof(SampleConfig), null, true);
        }
#endif
    }
}