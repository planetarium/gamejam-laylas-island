using System;
using LaylasIsland.Backend.Action;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blockchain.Renderers;
using Libplanet.Blocks;
using Serilog;
#if UNITY_EDITOR || UNITY_STANDALONE
using UniRx;
#else
using System.Reactive.Subjects;
using System.Reactive.Linq;
#endif
using static LaylasIsland.Backend.Action.BaseAction;

namespace LaylasIsland.Backend.Renderer
{
    using NCAction = PolymorphicAction<BaseAction>;
    using NCBlock = Block<PolymorphicAction<BaseAction>>;

    public class ActionRenderer : IActionRenderer<NCAction>
    {
        public Subject<BaseAction.ActionEvaluation<BaseAction>> ActionRenderSubject { get; }
            = new Subject<BaseAction.ActionEvaluation<BaseAction>>();

        public Subject<BaseAction.ActionEvaluation<BaseAction>> ActionUnrenderSubject { get; }
            = new Subject<BaseAction.ActionEvaluation<BaseAction>>();

        public readonly Subject<(NCBlock OldTip, NCBlock NewTip)> BlockEndSubject =
            new Subject<(NCBlock OldTip, NCBlock NewTip)>();

        public void RenderAction(
            IAction action,
            IActionContext context,
            IAccountStateDelta nextStates
        )
        {
            ActionRenderSubject.OnNext(new BaseAction.ActionEvaluation<BaseAction>()
            {
                Action = GetActionBase(action),
                Signer = context.Signer,
                BlockIndex = context.BlockIndex,
                OutputStates = nextStates,
                PreviousStates = context.PreviousStates,
            });
        }

        public void UnrenderAction(
            IAction action,
            IActionContext context,
            IAccountStateDelta nextStates
        )
        {
            ActionUnrenderSubject.OnNext(new BaseAction.ActionEvaluation<BaseAction>()
            {
                Action = GetActionBase(action),
                Signer = context.Signer,
                BlockIndex = context.BlockIndex,
                OutputStates = nextStates,
                PreviousStates = context.PreviousStates,
            });
        }

        public void RenderActionError(
            IAction action,
            IActionContext context,
            Exception exception
        )
        {
            Log.Error(exception, "{action} execution failed.", action);
            ActionRenderSubject.OnNext(new BaseAction.ActionEvaluation<BaseAction>()
            {
                Action = GetActionBase(action),
                Signer = context.Signer,
                BlockIndex = context.BlockIndex,
                OutputStates = context.PreviousStates,
                Exception = exception,
                PreviousStates = context.PreviousStates,
            });
        }

        public void UnrenderActionError(
            IAction action,
            IActionContext context,
            Exception exception
        )
        {
            ActionUnrenderSubject.OnNext(new BaseAction.ActionEvaluation<BaseAction>()
            {
                Action = GetActionBase(action),
                Signer = context.Signer,
                BlockIndex = context.BlockIndex,
                OutputStates = context.PreviousStates,
                Exception = exception,
                PreviousStates = context.PreviousStates,
            });
        }

        public void RenderBlock(
            NCBlock oldTip,
            NCBlock newTip
        )
        {
            // RenderBlock should be handled by BlockRenderer
        }

        public void RenderBlockEnd(
            NCBlock oldTip,
            NCBlock newTip
        )
        {
            BlockEndSubject.OnNext((oldTip, newTip));
        }

        public void RenderReorg(
            NCBlock oldTip,
            NCBlock newTip,
            NCBlock branchpoint
        )
        {
            // RenderReorg should be handled by BlockRenderer
        }

        public void RenderReorgEnd(
            NCBlock oldTip,
            NCBlock newTip,
            NCBlock branchpoint
        )
        {
            // RenderReorgEnd should be handled by BlockRenderer
        }

        public IObservable<BaseAction.ActionEvaluation<T>> EveryRender<T>()
            where T : BaseAction
        {
            return ActionRenderSubject.AsObservable().Where(
                eval => eval.Action is T
            ).Select(eval => new BaseAction.ActionEvaluation<T>
            {
                Action = (T) eval.Action,
                Signer = eval.Signer,
                BlockIndex = eval.BlockIndex,
                OutputStates = eval.OutputStates,
                Exception = eval.Exception,
                PreviousStates = eval.PreviousStates,
            });
        }

        public IObservable<BaseAction.ActionEvaluation<T>> EveryUnrender<T>()
            where T : BaseAction
        {
            return ActionUnrenderSubject.AsObservable().Where(
                eval => eval.Action is T
            ).Select(eval => new BaseAction.ActionEvaluation<T>
            {
                Action = (T) eval.Action,
                Signer = eval.Signer,
                BlockIndex = eval.BlockIndex,
                OutputStates = eval.OutputStates,
                Exception = eval.Exception,
                PreviousStates = eval.PreviousStates,
            });
        }

        public IObservable<BaseAction.ActionEvaluation<BaseAction>> EveryRender(Address updatedAddress)
        {
            return ActionRenderSubject.AsObservable().Where(
                eval => eval.OutputStates.UpdatedAddresses.Contains(updatedAddress)
            ).Select(eval => new BaseAction.ActionEvaluation<BaseAction>
            {
                Action = eval.Action,
                Signer = eval.Signer,
                BlockIndex = eval.BlockIndex,
                OutputStates = eval.OutputStates,
                Exception = eval.Exception,
                PreviousStates = eval.PreviousStates,
            });
        }

        public IObservable<BaseAction.ActionEvaluation<BaseAction>> EveryUnrender(Address updatedAddress)
        {
            return ActionUnrenderSubject.AsObservable().Where(
                eval => eval.OutputStates.UpdatedAddresses.Contains(updatedAddress)
            ).Select(eval => new BaseAction.ActionEvaluation<BaseAction>
            {
                Action = eval.Action,
                Signer = eval.Signer,
                BlockIndex = eval.BlockIndex,
                OutputStates = eval.OutputStates,
                Exception = eval.Exception,
                PreviousStates = eval.PreviousStates,
            });
        }

        public IObservable<(NCBlock OldTip, NCBlock NewTip)> EveryBlockEnd() =>
            BlockEndSubject.AsObservable();

        private static BaseAction GetActionBase(IAction action)
        {
            if (action is NCAction polymorphicAction)
            {
                return polymorphicAction.InnerAction;
            }
            return (BaseAction) action;
        }
    }
}
