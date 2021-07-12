using Boscohyun;
using UnityEngine;

namespace LaylasIsland.Frontend.UI
{
    public class UIHolder : MonoSingleton<UIHolder>
    {
        [SerializeField] private IntroCanvas _introCanvas;
        public static IntroCanvas IntroCanvas => Instance._introCanvas;

        [SerializeField] private MainCanvas _mainCanvas;
        public static MainCanvas MainCanvas => Instance._mainCanvas;
    }
}
