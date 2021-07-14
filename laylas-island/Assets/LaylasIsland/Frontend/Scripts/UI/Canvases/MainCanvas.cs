using LaylasIsland.Frontend.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    public class MainCanvas : MonoBehaviour
    {
        #region View
        
        [SerializeField] private Button _createGameButton;
        [SerializeField] private Button _joinGameButton;
        [SerializeField] private Button _createItemButton;
        [SerializeField] private Button _tradeButton;
        [SerializeField] private Button _creditButton;

        #endregion
        
        private void Awake()
        {
            // View
            _createGameButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.CreateGameCanvas.gameObject.SetActive(true);
            }).AddTo(gameObject);

            _joinGameButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.JoinGameCanvas.gameObject.SetActive(true);
            }).AddTo(gameObject);

            _createItemButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);

            _tradeButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);

            _creditButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);
            // ~View
        }

        private void OnEnable()
        {
            UIHolder.HeaderCanvas.Show(
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Post,
                HeaderCanvas.Element.Settings);
        }
    }
}
