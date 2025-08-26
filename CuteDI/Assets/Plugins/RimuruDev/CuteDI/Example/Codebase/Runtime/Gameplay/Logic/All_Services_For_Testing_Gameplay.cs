using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public interface IEnemySpawner { }
    
    public sealed class EnemySpawner : IEnemySpawner { }
    
    public sealed class GameplayController
    {
        public GameplayController(IEnemySpawner spawner, IGameNavigation nav) { }
    }
    
    public sealed class HUD : MonoBehaviour { }
}