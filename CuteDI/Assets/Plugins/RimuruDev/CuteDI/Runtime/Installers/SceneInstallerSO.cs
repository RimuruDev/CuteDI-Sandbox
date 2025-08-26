using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace AbyssMoth.CuteDI
{
    [Preserve]
    public abstract class SceneInstallerSO : ScriptableObject, ISceneInstaller
    {
        [Preserve]
        public abstract void Compose(in DIContainer scene, in DIContainer project);
        
        public virtual IEnumerable<BindingHint> PreviewHints() { yield break; }
    }
}