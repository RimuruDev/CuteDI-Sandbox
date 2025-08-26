using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class AllBindingsEntryPoint : SceneEntryPoint
    {
        protected override void Initialize()
        {
            var okA = Scene.HasRegistration<AllBindingsInstaller>();
            var okB = Scene.TryResolve<TaggedConsumer>(out _);
            var okC = Scene.TryResolve<AttrConsumer>(out _);
            var okD = Scene.TryResolve<ProcessorAggregator>(out _);
            var okE = Scene.TryResolve<IFoo>(out _, tag: "foo_go");
            var okE2 = Scene.TryResolve<IFoo>(out _, tag: "foo_widget");
            var okF = Scene.TryResolve<SampleConfig>(out _);

            Debug.Assert(okA);
            Debug.Assert(okB);
            Debug.Assert(okC);
            Debug.Assert(okD);
            Debug.Assert(okE);
            Debug.Assert(okE2);
            Debug.Assert(okF);

            var inst = Scene.Resolve<IReplaceSample>();
            Debug.Assert(inst is ReplaceB);
        }
    }
}