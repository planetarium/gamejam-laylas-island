using System;
using System.Collections.Generic;
using System.Linq;
using LaylasIsland.Frontend.Extensions;
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
        
        private readonly List<IDisposable> _disposablesOnEnable = new List<IDisposable>();
        private Action _navigationCallback;

        private void Awake()
        {
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

        private void OnEnable()
        {
            SharedGameModel.Player.Subscribe(player =>
            {
                if (player is null)
                {
                    return;
                }
                
                player.portrait.Subscribe(value => Debug.Log(value));
            }).AddTo(_disposablesOnEnable);
        }

        private void OnDisable()
        {
            _disposablesOnEnable.DisposeAllAndClear();
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
