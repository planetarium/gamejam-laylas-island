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
