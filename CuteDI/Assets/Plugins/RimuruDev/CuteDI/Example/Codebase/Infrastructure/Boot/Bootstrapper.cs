using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public class Bootstrapper : MonoBehaviour
    {
        private IProjectContext projectContext;

        private void Awake()
        {
            projectContext = new ProjectContext();
            projectContext.Register();
            projectContext.Resolve();

            DontDestroyOnLoad(gameObject);
        }


        private void OnDestroy() =>
            projectContext?.Release();
    }
}