using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI
{
    using UniRx;

    public class MessagePopupCanvas : MonoBehaviour
    {
        #region View

        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _bodyText;
        [SerializeField] private TextMeshProUGUI _footerText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private TextMeshProUGUI _leftButtonText;
        [SerializeField] private Button _rightButton;
        [SerializeField] private TextMeshProUGUI _rightButtonText;

        #endregion

        private Action _leftButtonCallback;
        private Action _rightButtonCallback;

        private void Awake()
        {
            _leftButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                _leftButtonCallback?.Invoke();
                gameObject.SetActive(false);
            }).AddTo(gameObject);

            _rightButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                _rightButtonCallback?.Invoke();
                gameObject.SetActive(false);
            }).AddTo(gameObject);
        }

        public void ShowWithoutButtons(string header, string body, string footer)
        {
            SetTexts(header, body, footer);
            SetButtons(string.Empty, null, string.Empty, null);
            gameObject.SetActive(true);
        }

        public void ShowWithoutButtons(string header, string body) =>
            ShowWithoutButtons(header, body, string.Empty);

        public void ShowWithoutButtons(string body) =>
            ShowWithoutButtons(string.Empty, body, string.Empty);

        public void ShowWithASingleButton(string header, string body, string footer, string buttonText,
            Action buttonCallback)
        {
            SetTexts(header, body, footer);
            SetButtons(buttonText, buttonCallback, string.Empty, null);
            gameObject.SetActive(true);
        }

        public void ShowWithASingleButton(string header, string body, string buttonText, Action buttonCallback) =>
            ShowWithASingleButton(header, body, string.Empty, buttonText, buttonCallback);

        public void ShowWithASingleButton(string body, string buttonText, Action buttonCallback) =>
            ShowWithASingleButton(string.Empty, body, string.Empty, buttonText, buttonCallback);

        public void ShowWithACoupleOfButtons(
            string header,
            string body,
            string footer,
            string leftButtonText,
            Action leftButtonCallback,
            string rightButtonText,
            Action rightButtonCallback)
        {
            SetTexts(header, body, footer);
            SetButtons(leftButtonText, leftButtonCallback, rightButtonText, rightButtonCallback);
            gameObject.SetActive(true);
        }

        public void ShowWithACoupleOfButtons(
            string header,
            string body,
            string leftButtonText,
            Action leftButtonCallback,
            string rightButtonText,
            Action rightButtonCallback) =>
            ShowWithACoupleOfButtons(
                header,
                body,
                string.Empty,
                leftButtonText,
                leftButtonCallback,
                rightButtonText,
                rightButtonCallback);

        public void ShowWithACoupleOfButtons(
            string body,
            string leftButtonText,
            Action leftButtonCallback,
            string rightButtonText,
            Action rightButtonCallback) =>
            ShowWithACoupleOfButtons(
                string.Empty,
                body,
                string.Empty,
                leftButtonText,
                leftButtonCallback,
                rightButtonText,
                rightButtonCallback);

        private void SetTexts(string header, string body, string footer)
        {
            if (string.IsNullOrEmpty(header))
            {
                _headerText.enabled = false;
            }
            else
            {
                _headerText.text = header;
                _headerText.enabled = true;
            }

            if (string.IsNullOrEmpty(body))
            {
                _bodyText.enabled = false;
            }
            else
            {
                _bodyText.text = body;
                _bodyText.enabled = true;
            }

            if (string.IsNullOrEmpty(footer))
            {
                _footerText.enabled = false;
            }
            else
            {
                _footerText.text = footer;
                _footerText.enabled = true;
            }
        }

        private void SetButtons(
            string leftButtonText,
            Action leftButtonCallback,
            string rightButtonText,
            Action rightButtonCallback)
        {
            _leftButtonCallback = leftButtonCallback;
            _rightButtonCallback = rightButtonCallback;
            _leftButton.gameObject.SetActive(!(_leftButtonCallback is null));
            _leftButtonText.text = leftButtonText;
            _rightButton.gameObject.SetActive(!(_rightButtonCallback is null));
            _rightButtonText.text = rightButtonText;
        }
    }
}
