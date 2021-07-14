using System;
using System.Collections.Generic;
using LaylasIsland.Frontend.Game;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI
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
        public struct SelectPeople
        {
            public List<Button> arrowUpButtons;
            public List<Button> arrowDownButtons;
            public List<TextMeshProUGUI> countTexts;
        }

        [Serializable]
        public struct SelectVisible
        {
            public Image selection;
            public Button publicButton;
            public Button privateButton;
        }

        [SerializeField] private SelectPeople _selectPeople;
        [SerializeField] private SelectVisible _selectVisible;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _createButton;

        #endregion

        #region Model

        private static readonly int[] _peopleCounts = {1, 2, 4};

        private readonly ReactiveProperty<int> _peopleCountsIndex = new ReactiveProperty<int>(_peopleCounts.Length - 1);

        private readonly ReactiveProperty<Visible> _visible = new ReactiveProperty<Visible>(default);

        #endregion

        private void Awake()
        {
            // View
            for (var i = _selectPeople.arrowUpButtons.Count; i > 0; i--)
            {
                _selectPeople.arrowUpButtons[i - 1].OnClickAsObservable().Subscribe(_ =>
                {
                    if (_peopleCountsIndex.Value >= _peopleCounts.Length - 1)
                    {
                        return;
                    }

                    _peopleCountsIndex.Value++;
                }).AddTo(gameObject);
            }

            for (var i = _selectPeople.arrowDownButtons.Count; i > 0; i--)
            {
                _selectPeople.arrowDownButtons[i - 1].OnClickAsObservable().Subscribe(_ =>
                {
                    if (_peopleCountsIndex.Value <= 0)
                    {
                        return;
                    }

                    _peopleCountsIndex.Value--;
                }).AddTo(gameObject);
            }

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
                GameController.Instance.InitializeAsObservable(new GameNetworkManager.JoinOrCreateRoomOptions(
                    GameNetworkManager.JoinOrCreate.Create,
                    "Game Room Name", // _roomNameInputField.text,
                    _passwordInputField.text
                    ))
                    .Subscribe(e =>
                    {
                        if (e is null)
                        {
                            UIHolder.LoadingCanvas.gameObject.SetActive(false);
                            UIHolder.PrepareGameCanvas.gameObject.SetActive(true);
                        }
                        else
                        {
                            UIHolder.MessagePopupCanvas.ShowWithASingleButton(e.Message, "OK", () =>
                            {
                                UIHolder.LoadingCanvas.gameObject.SetActive(false);
                                gameObject.SetActive(true);
                            });
                        }
                    });
            }).AddTo(gameObject);
            // ~View

            // Model
            _peopleCountsIndex.Subscribe(value =>
            {
                if (value < 0 || value >= _peopleCounts.Length)
                {
                    return;
                }

                for (var i = _selectPeople.countTexts.Count; i > 0; i--)
                {
                    _selectPeople.countTexts[i - 1].text = _peopleCounts[value].ToString();
                }
            }).AddTo(gameObject);

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
