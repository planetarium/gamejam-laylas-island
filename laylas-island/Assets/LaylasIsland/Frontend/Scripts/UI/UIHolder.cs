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

        [SerializeField] private CreateGameCanvas _createGameCanvas;
        public static CreateGameCanvas CreateGameCanvas => Instance._createGameCanvas;

        [SerializeField] private JoinGameCanvas _joinGameCanvas;
        public static JoinGameCanvas JoinGameCanvas => Instance._joinGameCanvas;

        [SerializeField] private PrepareGameCanvas _prepareGameCanvas;
        public static PrepareGameCanvas PrepareGameCanvas => Instance._prepareGameCanvas;

        [SerializeField] private PlayGameCanvas _playGameCanvas;
        public static PlayGameCanvas PlayGameCanvas => Instance._playGameCanvas;

        [SerializeField] private EndGameCanvas _endGameCanvas;
        public static EndGameCanvas EndGameCanvas => Instance._endGameCanvas;
        
        [SerializeField] private HeaderCanvas _headerCanvas;
        public static HeaderCanvas HeaderCanvas => Instance._headerCanvas;
        
        [SerializeField] private LoadingCanvas _loadingCanvas;
        public static LoadingCanvas LoadingCanvas => Instance._loadingCanvas;
        
        [SerializeField] private MessagePopupCanvas _messagePopupCanvas;
        public static MessagePopupCanvas MessagePopupCanvas => Instance._messagePopupCanvas;
    }
}
