using System.Collections;

namespace LaylasIsland.Frontend.Game.GameStateBehaviours
{
    public interface IGameStateBehaviour
    {
        void Enter();
        IEnumerator CoUpdate();
        void Exit();
    }
}
