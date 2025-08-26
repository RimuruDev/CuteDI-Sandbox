using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    public interface ISceneInstaller
    {
        [Preserve]
        public void Compose(in DIContainer scene, in DIContainer project);
    }
}