using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class MainMenuEntryPoint : SceneEntryPoint
    {
        protected override void Initialize()
        {
            var ok1 = Project.TryResolve<IProjectContext>(out _);
            var ok2 = Scene.HasRegistration<MainMenuInstaller>();

            Debug.Assert(ok1);
            Debug.Assert(ok2);

           var ok3 = Project.Resolve<IGameNavigation>();
           
           Debug.Assert(ok3 != null);
        }
    }
}