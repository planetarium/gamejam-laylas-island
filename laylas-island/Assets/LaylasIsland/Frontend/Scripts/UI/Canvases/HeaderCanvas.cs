using System;
using System.Linq;
using LaylasIsland.Frontend.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using UniRx;
    
    public class HeaderCanvas : MonoBehaviour
    {
        public enum Element
        {
            Back,
            Player,
            Gold,
            Post,
            Settings,
        }

        #region View
        
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _playerButton;
        [SerializeField] private Button _goldButton;
        [SerializeField] private Button _postButton;
        [SerializeField] private Button _settingsButton;

        #endregion
        
        private Action _navigationCallback;

        private void Awake()
        {
            // Model
            SharedGameModel.Player.Subscribe(player =>
            {
                if (player is null)
                {
                    return;
                }
                
                player.portrait.Subscribe(value => Debug.Log(value));
            }).AddTo(gameObject);
            // ~Model
            
            // View
            _backButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                _navigationCallback?.Invoke();
            }).AddTo(gameObject);

            _playerButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
            }).AddTo(gameObject);

            _postButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
            }).AddTo(gameObject);

            _goldButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
            }).AddTo(gameObject);

            _settingsButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
            }).AddTo(gameObject);
            // ~View
        }

        public void Show(params Element[] elements) => Show(null, elements);

        public void Show(Action navigationCallback, params Element[] elements)
        {
            _navigationCallback = navigationCallback;
            UpdateElements(elements);
            gameObject.SetActive(true);
        }

        private void UpdateElements(Element[] elements)
        {
            _backButton.gameObject.SetActive(elements.Contains(Element.Back));
            _playerButton.gameObject.SetActive(elements.Contains(Element.Player));
            _goldButton.gameObject.SetActive(elements.Contains(Element.Gold));
            _postButton.gameObject.SetActive(elements.Contains(Element.Post));
            _settingsButton.gameObject.SetActive(elements.Contains(Element.Settings));
        }
    }
}
