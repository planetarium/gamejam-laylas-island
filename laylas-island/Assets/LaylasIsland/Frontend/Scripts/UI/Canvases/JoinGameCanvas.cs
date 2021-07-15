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

        private void Awake()
        {
            // View
            _joinButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.LoadingCanvas.gameObject.SetActive(true);
                GameController.Instance.EnterAsObservable(new GameNetworkManager.JoinOrCreateRoomOptions(
                        GameNetworkManager.JoinOrCreate.Join,
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
