using LaylasIsland.Backend.State;
using Libplanet.Assets;
using UniRx;

namespace LaylasIsland.Frontend.State.Subjects
{
    public static class GoldBalanceStateSubject
    {
        public static readonly Subject<FungibleAssetValue> Gold = new Subject<FungibleAssetValue>();

        public static void Set(GoldBalanceState goldBalanceState)
        {
            Gold.OnNext(goldBalanceState.Gold);
        }
    }
}
