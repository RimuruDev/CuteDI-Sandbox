using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class GameplayEntryPoint : SceneEntryPoint
    {
        protected override void Initialize()
        {
            var ok1 = Project.TryResolve<IGameNavigation>(out _);
            var ok2 = Scene.HasRegistration<GameplayInstaller>();
           
            Debug.Assert(ok1);
            Debug.Assert(ok2);
        }
    }
}