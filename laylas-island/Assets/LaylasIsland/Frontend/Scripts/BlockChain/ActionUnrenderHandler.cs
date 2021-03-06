using System;
using System.Collections.Generic;
using LaylasIsland.Backend.Action;
using LaylasIsland.Backend.Renderer;
using LaylasIsland.Frontend.Extensions;
using UnityEngine;

namespace LaylasIsland.Frontend.BlockChain
{
    using UniRx;
    public class ActionUnrenderHandler : ActionHandler
    {
        private static class Singleton
        {
            internal static readonly ActionUnrenderHandler Value = new ActionUnrenderHandler();
        }

        public static readonly ActionUnrenderHandler Instance = Singleton.Value;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        
        private ActionRenderer _renderer;

        private ActionUnrenderHandler()
        {
        }

        public void Start(ActionRenderer renderer)
        {
            _renderer = renderer;
            _renderer.EveryUnrender<SignUp>()
                .Where(ValidateEvaluationSignerEqualsAgent)
                .ObserveOnMainThread()
                .Subscribe(UnRenderSignUp)
                .AddTo(_disposables);
        }

        public void Stop()
        {
            _disposables.DisposeAllAndClear();
        }

        private void UnRenderSignUp(BaseAction.ActionEvaluation<SignUp> eval)
        {
            if (!(eval.Exception is null))
            {
                Debug.LogError(eval.Exception.ToString());
                return;
            }

            var agent = eval.OutputStates.GetState(MainController.Instance.Agent.Address);
            Debug.Log(agent);
        }
    }
}
