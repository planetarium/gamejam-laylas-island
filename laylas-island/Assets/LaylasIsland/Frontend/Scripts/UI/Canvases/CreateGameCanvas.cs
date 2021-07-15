using System;
using LaylasIsland.Frontend.Game;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using UniRx;

    public class CreateGameCanvas : MonoBehaviour
    {
        private enum Visible
        {
            Public,
            Private,
        }

        #region View

        [Serializable]
        public struct SelectVisible
        {
            public Image selection;
            public Button publicButton;
            public Button privateButton;
        }

        [SerializeField] private SelectVisible _selectVisible;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _createButton;

        #endregion

        #region Model

        private readonly ReactiveProperty<Visible> _visible = new ReactiveProperty<Visible>(default);

        #endregion

        private void Awake()
        {
            // View
            _selectVisible.publicButton.OnClickAsObservable()
                .Subscribe(_ => _visible.Value = Visible.Public)
                .AddTo(gameObject);

            _selectVisible.privateButton.OnClickAsObservable()
                .Subscribe(_ => _visible.Value = Visible.Private)
                .AddTo(gameObject);

            _createButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.LoadingCanvas.gameObject.SetActive(true);
                GameController.Instance.EnterAsObservable(new GameNetworkManager.JoinOrCreateRoomOptions(
                    GameNetworkManager.JoinOrCreate.Create,
                    _roomNameInputField.text,
                    _passwordInputField.text
                )).Subscribe(e =>
                {
                    if (e is null)
                    {
                        UIHolder.LoadingCanvas.gameObject.SetActive(false);
                        UIHolder.PrepareGameCanvas.gameObject.SetActive(true);
                    }
                    else
                    {
                        UIHolder.MessagePopupCanvas.ShowWithASingleButton(
                            "Failed",
                            e.Message,
                            "OK",
                            () =>
                            {
                                UIHolder.LoadingCanvas.gameObject.SetActive(false);
                                gameObject.SetActive(true);
                            });
                    }
                });
            }).AddTo(gameObject);
            // ~View

            // Model
            _visible.Subscribe(value =>
            {
                var localPosition = _selectVisible.selection.transform.localPosition;
                switch (value)
                {
                    case Visible.Public:
                        localPosition.x = math.abs(localPosition.x) * -1f;
                        _passwordInputField.transform.parent.gameObject.SetActive(false);
                        break;
                    case Visible.Private:
                        localPosition.x = math.abs(localPosition.x);
                        _passwordInputField.transform.parent.gameObject.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                _selectVisible.selection.transform.localPosition = localPosition;
            }).AddTo(gameObject);
            // ~Model
        }

        private void OnEnable()
        {
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
    }
}
