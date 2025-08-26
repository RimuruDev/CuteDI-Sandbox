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
        
#if UNITY_EDITOR
        public virtual IEnumerable<BindingHint> PreviewHints() { yield break; }
 #endif
    }
}