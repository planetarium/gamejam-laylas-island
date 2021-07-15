using System;
using System.Collections.Generic;
using LaylasIsland.Frontend.Extensions;
using LaylasIsland.Frontend.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using UniRx;

    public class JoinGameCanvas : MonoBehaviour
    {
        #region View

        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _joinButton;

        #endregion

        private readonly List<IDisposable> _disposablesOnEnable = new List<IDisposable>();

        private void Awake()
        {
            // View
            _joinButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                GameController.Instance.Enter(new GameNetworkManager.JoinOrCreateRoomOptions(
                    GameNetworkManager.JoinOrCreate.Join,
                    _roomNameInputField.text,
                    _passwordInputField.text
                ));
            }).AddTo(gameObject);
            // ~View
        }

        private void OnEnable()
        {
            SharedGameModel.IsNetworkReady
                .Subscribe(value => _joinButton.interactable = value)
                .AddTo(_disposablesOnEnable);

            UIHolder.HeaderCanvas.Show(
                () =>
                {
                    gameObject.SetActive(false);
                    UIHolder.MainCanvas.gameObject.SetActive(true);
                },
                HeaderCanvas.Element.Back,
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Settings);
        }

        private void OnDisable()
        {
            _disposablesOnEnable.DisposeAllAndClear();
        }
    }
}
