namespace LaylasIsland.Frontend.Game
{
    public enum GameState
    {
        None = 0,
        Initializing,
        InitializeFailed,
        Prepare,
        Play,
        End,
        Terminating,
        Terminated,
    }
}
