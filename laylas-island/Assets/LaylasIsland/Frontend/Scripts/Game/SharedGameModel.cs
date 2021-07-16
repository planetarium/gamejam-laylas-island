using System.Linq;

namespace LaylasIsland.Frontend.Game
{
    using UniRx;

    // FIXME: 마스터 클라이언트의 모델과 분리해야 합니다.
    public static class SharedGameModel
    {
        public static readonly ReactiveProperty<Player> Player
            = new ReactiveProperty<Player>();

        public static readonly ReactiveProperty<bool> IsNetworkReady
            = new ReactiveProperty<bool>();

        public static readonly ReactiveProperty<GameState> State
            = new ReactiveProperty<GameState>(default);

        public static readonly ReactiveCollection<Player> BluePlayers
            = new ReactiveCollection<Player>();

        public static readonly ReactiveCollection<Player> RedPlayers
            = new ReactiveCollection<Player>();

        public static readonly Subject<Unit> OnClickStartFromRPC
            = new Subject<Unit>();

        public static readonly ReactiveProperty<int> Countdown
            = new ReactiveProperty<int>(-1);

        public static readonly ReactiveDictionary<string, int> BluePlayerScores
            = new ReactiveDictionary<string, int>();

        public static readonly ReactiveDictionary<string, int> RedPlayerScores
            = new ReactiveDictionary<string, int>();

        public static readonly IReadOnlyReactiveProperty<int> BlueScore
            = BluePlayerScores.ObserveCountChanged().Select(_ => Unit.Default)
                .Merge(BluePlayerScores.ObserveReplace().Select(_ => Unit.Default))
                .Merge(BluePlayerScores.ObserveReset())
                .Select(_ => BluePlayerScores.Values.Sum())
                .ToReactiveProperty(0);

        public static readonly IReadOnlyReactiveProperty<int> RedScore
            = RedPlayerScores.ObserveCountChanged().Select(_ => Unit.Default)
                .Merge(RedPlayerScores.ObserveReplace().Select(_ => Unit.Default))
                .Merge(RedPlayerScores.ObserveReset())
                .Select(_ => RedPlayerScores.Values.Sum())
                .ToReactiveProperty(0);
    }
}
