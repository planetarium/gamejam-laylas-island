using System;
using LaylasIsland.Backend.Action;
using LaylasIsland.Backend.Renderer;

namespace LaylasIsland.Frontend.BlockChain
{
    using UniRx;
    
    /// <summary>
    /// 게임의 Action을 생성하고 Agent에 넣어주는 역할을 한다.
    /// </summary>
    public class ActionManager
    {
        private static readonly TimeSpan ActionTimeout = TimeSpan.FromSeconds(360f);

        private readonly IAgent _agent;

        private readonly ActionRenderer _renderer;

        private static ActionManager Instance => MainController.Instance.ActionManager;

        private void ProcessAction(GameAction gameAction)
        {
            _agent.EnqueueAction(gameAction);
        }

        private void HandleException(Guid actionId, Exception e)
        {
            if (e is TimeoutException)
            {
                throw new ActionTimeoutException(e.Message, actionId);
            }

            throw e;
        }

        public ActionManager(IAgent agent)
        {
            _agent = agent;
            _renderer = agent.ActionRenderer;
        }

        public static IObservable<BaseAction.ActionEvaluation<SignUp>> SignUp()
        {
            var action = new SignUp();
            Instance.ProcessAction(action);
            return Instance._renderer.EveryRender<SignUp>()
                .SkipWhile(eval => !eval.Action.Id.Equals(action.Id))
                .First()
                .ObserveOnMainThread()
                .Timeout(ActionTimeout);
        }
    }
}
