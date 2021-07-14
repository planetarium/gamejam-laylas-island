using UniRx;

namespace LaylasIsland.Frontend.Game
{
    public static class SharedGameModel
    {
        public static readonly ReactiveProperty<Player> Player
            = new ReactiveProperty<Player>();
        
        public static readonly ReactiveProperty<GameState> State
            = new ReactiveProperty<GameState>(default);

        public static readonly ReactiveCollection<Player> BluePlayers
            = new ReactiveCollection<Player>();

        public static readonly ReactiveCollection<Player> RedPlayers
            = new ReactiveCollection<Player>();
    }
}
