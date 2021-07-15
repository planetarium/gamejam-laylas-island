using System;
using System.Collections.Generic;
using System.Linq;
using Libplanet;
using Libplanet.Crypto;
using Libplanet.KeyStore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    using UniRx;

    public class IntroCanvas : MonoBehaviour
    {
        [Serializable]
        public struct Signing
        {
            public GameObject rootGameObject;
            public TMP_Dropdown selectAddress;
            public TMP_InputField secretInputField;
            public Button button;
            public TextMeshProUGUI buttonText;
        }

        #region View

        [SerializeField] private Signing _signing;

        #endregion

        #region Model

        private readonly ReactiveProperty<List<Tuple<Guid, ProtectedPrivateKey>>> _signingOptionSource =
            new ReactiveProperty<List<Tuple<Guid, ProtectedPrivateKey>>>();

        #endregion

        private bool _createdNewOne;
        private PrivateKey _createdPrivateKey;

        public readonly Subject<Unit> OnClickSigning = new Subject<Unit>();

        public PrivateKey SelectedPrivateKey { get; private set; }

        private void Awake()
        {
            // Model
            _signingOptionSource.Subscribe(optionSource =>
            {
                _signing.selectAddress.ClearOptions();
                if (optionSource is null)
                {
                    _signing.selectAddress.AddOptions(new List<string> {"Create New One"});
                    _signing.selectAddress.value = 0;
                    return;
                }

                var options = optionSource
                    .Select(e => e.Item2.Address.ToString())
                    .ToList();
                if (!_createdNewOne)
                {
                    options.Add("Create New One");
                }

                _signing.selectAddress.AddOptions(options);
                _signing.selectAddress.value = 0;
            }).AddTo(gameObject);
            // ~Model

            // View
            _signing.selectAddress.onValueChanged.AsObservable().Subscribe(index =>
            {
                // Play Click SFX
                var selection = _signing.selectAddress.options[index];
                if (selection.text.Equals("Create New One"))
                {
                    _createdNewOne = true;
                    _createdPrivateKey = new PrivateKey();
                    SelectedPrivateKey = _createdPrivateKey;
                    selection.text = _createdPrivateKey.ToAddress().ToString();
                    _signing.selectAddress.RefreshShownValue();
                    _signing.button.interactable = true;
                }

                if (_createdNewOne &&
                    index == _signing.selectAddress.options.Count - 1)
                {
                    _signing.buttonText.text = "Sign-up";
                }
                else
                {
                    _signing.buttonText.text = "Sign-in";
                }
            }).AddTo(gameObject);

            _signing.secretInputField.onValueChanged.AsObservable().Subscribe(value =>
            {
                // Play Typing SFX
                if (_createdNewOne &&
                    SelectedPrivateKey.Equals(_createdPrivateKey))
                {
                    return;
                }

                UnprotectSelected(value);
            }).AddTo(gameObject);

            _signing.secretInputField.onEndEdit.AsObservable().Subscribe(value => _signing.button.onClick.Invoke())
                .AddTo(gameObject);

            _signing.button.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                if (_createdNewOne &&
                    SelectedPrivateKey.Equals(_createdPrivateKey))
                {
                    var ppk =
                        ProtectedPrivateKey.Protect(_createdPrivateKey, _signing.secretInputField.text);
                    Web3KeyStore.DefaultKeyStore.Add(ppk);
                }

                OnClickSigning.OnNext(Unit.Default);
            }).AddTo(gameObject);
            // ~View
        }

        private void OnEnable()
        {
            UIHolder.HeaderCanvas.gameObject.SetActive(false);
            _signing.rootGameObject.SetActive(false);
            _signing.secretInputField.text = string.Empty;
            _signing.button.interactable = false;
            _signing.buttonText.text = "Sign-in";
        }

        public void ShowSigning()
        {
            var keyStore = Web3KeyStore.DefaultKeyStore;
            var keyList = keyStore.List().ToList();
            _signingOptionSource.Value = keyList;
            _signing.rootGameObject.SetActive(true);
        }

        private void UnprotectSelected(string passphrase)
        {
            try
            {
                SelectedPrivateKey =
                    _signingOptionSource.Value[_signing.selectAddress.value].Item2.Unprotect(passphrase);
                _signing.button.interactable = true;
            }
            catch (Exception)
            {
                _signing.button.interactable = false;
            }
        }
    }
}
