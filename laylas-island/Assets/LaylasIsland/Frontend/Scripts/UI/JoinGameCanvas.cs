using LaylasIsland.Frontend.Game;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI
{
    public class JoinGameCanvas : MonoBehaviour
    {
        #region View

        [SerializeField] private CreateGameCanvas.SelectPeople _selectPeople;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _joinButton;

        #endregion

        #region Model

        private static readonly int[] _peopleCounts = {1, 2, 4, -1};

        private readonly ReactiveProperty<int> _peopleCountsIndex = new ReactiveProperty<int>(_peopleCounts.Length - 1);

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

            _joinButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.LoadingCanvas.gameObject.SetActive(true);
                GameController.Instance.InitializeAsObservable("Game Room Name", string.Empty)
                    .First()
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
                    var count = _peopleCounts[value];
                    _selectPeople.countTexts[i - 1].text = count == -1
                        ? "?"
                        : count.ToString();
                }
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
