using System;
using System.Collections.Generic;
using LibUnity.Backend.Renderer;
using LibUnity.Frontend.Extensions;
using UnityEngine;

namespace LibUnity.Frontend.BlockChain
{
    using UniRx;

    public class BlockRenderHandler
    {
        private static class Singleton
        {
            internal static readonly BlockRenderHandler Value = new BlockRenderHandler();
        }

        public static BlockRenderHandler Instance => Singleton.Value;

        private BlockRenderer _blockRenderer;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private BlockRenderHandler()
        {
        }

        public void Start(BlockRenderer blockRenderer)
        {
            Stop();
            _blockRenderer = blockRenderer;

            Reorg();
        }

        public void Stop()
        {
            _disposables.DisposeAllAndClear();
        }

        private void Reorg()
        {
            _blockRenderer.ReorgSubject
                .ObserveOnMainThread()
                .Subscribe(_ => { Debug.LogWarning("Reorg"); })
                .AddTo(_disposables);
        }
    }
}
