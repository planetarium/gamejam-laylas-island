using System;
using System.Collections.Generic;
using LibUnity.Backend.Renderer;
using LibUnity.Frontend.Extensions;

namespace LibUnity.Frontend.BlockChain
{
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
        }

        public void Stop()
        {
            _disposables.DisposeAllAndClear();
        }
    }
}
