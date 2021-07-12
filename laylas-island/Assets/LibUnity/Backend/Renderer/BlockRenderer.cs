using System;
using Libplanet.Action;
using Libplanet.Blockchain.Renderers;
using Libplanet.Blocks;
using LibUnity.Backend.Action;
#if UNITY_EDITOR || UNITY_STANDALONE
using UniRx;
#else
using System.Reactive.Subjects;
using System.Reactive.Linq;
#endif

namespace LibUnity.Backend.Renderer
{
    using NCAction = PolymorphicAction<BaseAction>;
    using NCBlock = Block<PolymorphicAction<BaseAction>>;

    public class BlockRenderer : IRenderer<NCAction>
    {
        public readonly Subject<(NCBlock OldTip, NCBlock NewTip)> BlockSubject =
            new Subject<(NCBlock OldTip, NCBlock NewTip)>();

        public readonly Subject<(NCBlock OldTip, NCBlock NewTip, NCBlock Branchpoint)> ReorgSubject =
            new Subject<(NCBlock OldTip, NCBlock NewTip, NCBlock Branchpoint)>();

        public readonly Subject<(NCBlock OldTip, NCBlock NewTip, NCBlock Branchpoint)> ReorgEndSubject =
            new Subject<(NCBlock OldTip, NCBlock NewTip, NCBlock Branchpoint)>();

        public void RenderBlock(
            NCBlock oldTip,
            NCBlock newTip
        )
        {
            BlockSubject.OnNext((oldTip, newTip));
        }

        public void RenderReorg(
            NCBlock oldTip,
            NCBlock newTip,
            NCBlock branchpoint
        )
        {
            ReorgSubject.OnNext((oldTip, newTip, branchpoint));
        }

        public void RenderReorgEnd(
            NCBlock oldTip,
            NCBlock newTip,
            NCBlock branchpoint
        ) =>
            ReorgEndSubject.OnNext((oldTip, newTip, branchpoint));

        public IObservable<(NCBlock OldTip, NCBlock NewTip)> EveryBlock() =>
            BlockSubject.AsObservable();

        public IObservable<(NCBlock OldTip, NCBlock NewTip, NCBlock Branchpoint)>
            EveryReorg() =>
            ReorgSubject.AsObservable();

        public IObservable<(NCBlock OldTip, NCBlock NewTip, NCBlock Branchpoint)>
            EveryReorgEnd() =>
            ReorgEndSubject.AsObservable();
    }
}
