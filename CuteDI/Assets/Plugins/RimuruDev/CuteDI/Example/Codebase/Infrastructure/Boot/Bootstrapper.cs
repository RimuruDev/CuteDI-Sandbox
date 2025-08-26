using System;
using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public class Bootstrapper : MonoBehaviour
    {
        private IProjectContext projectContext;
        private IGameNavigation navigation;

        private void Awake()
        {
            projectContext = new ProjectContext();
            projectContext.Register();
            projectContext.Resolve();
            
            navigation = projectContext.Container.Resolve<IGameNavigation>();

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                navigation.GoToMainMenu();
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
                navigation.GoToGameplay();  
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
                navigation.GoToOther();
        }

        private void OnDestroy() =>
            projectContext?.Release();
    }
}