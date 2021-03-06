using System;
using System.Collections.Generic;
using Bencodex.Types;
using LaylasIsland.Backend.Action;
using LaylasIsland.Backend.Renderer;
using LaylasIsland.Backend.State;
using LaylasIsland.Frontend.Extensions;
using LaylasIsland.Frontend.State;
using UnityEngine;

namespace LaylasIsland.Frontend.BlockChain
{
    using UniRx;
    /// <summary>
    /// 현상태 : 각 액션의 랜더 단계에서 즉시 게임 정보에 반영시킴. 아바타를 선택하지 않은 상태에서 이전에 성공시키지 못한 액션을 재수행하고
    ///       이를 핸들링하면, 즉시 게임 정보에 반영시길 수 없기 때문에 에러가 발생함.
    /// 참고 : 이후 언랜더 처리를 고려한 해법이 필요함.
    /// 해법 1: 랜더 단계에서 얻는 `eval` 자체 혹은 변경점을 queue에 넣고, 게임의 상태에 따라 꺼내 쓰도록.
    /// </summary>
    public class ActionRenderHandler : ActionHandler
    {
        private static class Singleton
        {
            internal static readonly ActionRenderHandler Value = new ActionRenderHandler();
        }

        public static ActionRenderHandler Instance => Singleton.Value;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private ActionRenderer _renderer;

        private ActionRenderHandler()
        {
        }

        public void Start(ActionRenderer renderer)
        {
            _renderer = renderer;
            _renderer.EveryRender<SignUp>()
                .Where(ValidateEvaluationSignerEqualsAgent)
                .ObserveOnMainThread()
                .Subscribe(RenderSignUp)
                .AddTo(_disposables);
        }

        public void Stop()
        {
            _disposables.DisposeAllAndClear();
        }

        private static void RenderSignUp(BaseAction.ActionEvaluation<SignUp> eval)
        {
            Debug.Log("Render SignUp");
            if (!(eval.Exception is null))
            {
                Debug.LogError(eval.Exception.ToString());
                return;
            }

            var agentStateValue = eval.OutputStates.GetState(MainController.Instance.Agent.Address);
            if (agentStateValue is null)
            {
                Debug.LogError($"{nameof(agentStateValue)} is null");
                return;
            }
            
            States.Instance.SetAgentState(new AgentState((Dictionary) agentStateValue));
        }
    }
}
