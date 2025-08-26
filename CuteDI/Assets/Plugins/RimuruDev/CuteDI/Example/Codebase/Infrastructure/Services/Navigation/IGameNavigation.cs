using System;

namespace AbyssMoth.CuteDI.Example
{
    public interface IGameNavigation
    {
        public void GoToMainMenu(Action<DIContainer> onLoaded = null);
        public void GoToGameplay(Action<DIContainer> onLoaded = null);
        public void LoadScene(string sceneName, Action<DIContainer> onLoaded = null);
    }
}