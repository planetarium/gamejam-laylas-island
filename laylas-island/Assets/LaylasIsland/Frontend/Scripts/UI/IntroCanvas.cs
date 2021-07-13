using System;
using System.Collections.Generic;
using System.Linq;
using Libplanet.Crypto;
using TMPro;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI
{
    using UniRx;

    public class IntroCanvas : MonoBehaviour
    {
        [Serializable]
        public struct Signing
        {
            public GameObject rootGameObject;
            public TMP_Dropdown selectPrivateKey;
            public TMP_InputField secretInputField;
            public Button button;
        }

        #region View

        [SerializeField] private Slider _progress;
        [SerializeField] private Signing _signing;

        #endregion

        #region Model

        private readonly ReactiveProperty<List<string>> _signingOptions = new ReactiveProperty<List<string>>();
        private bool _hasCreateNewPrivateKey;

        #endregion

        private void Awake()
        {
            // View
            _signing.selectPrivateKey.onValueChanged.AsObservable().Subscribe(index =>
            {
                // Play Click SFX
                var selection = _signing.selectPrivateKey.options[index];
                if (selection.text.Equals("Create New One"))
                {
                    // Create New One
                    _hasCreateNewPrivateKey = true;
                    var privateKey = new PrivateKey();
                    return;
                }
                
                // Sign-in
            }).AddTo(gameObject);

            _signing.secretInputField.OnSubmitAsObservable().Subscribe(value => { }).AddTo(gameObject);

            _signing.button.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.MainCanvas.gameObject.SetActive(true);
            }).AddTo(gameObject);
            // ~View

            // Model
            _signingOptions.Subscribe(options =>
            {
                _signing.selectPrivateKey.ClearOptions();
                if (options is null)
                {
                    _signing.selectPrivateKey.AddOptions(new List<string> { "Create New One" });
                    _signing.selectPrivateKey.value = 0;
                    return;
                }
                
                _signing.selectPrivateKey.AddOptions(options);
                _signing.selectPrivateKey.value = 0;
            }).AddTo(gameObject);
            // ~Model
        }

        private void OnEnable()
        {
            UIHolder.HeaderCanvas.gameObject.SetActive(false);
            _progress.value = 0f;
            _signing.rootGameObject.SetActive(false);
            Test();
        }

        private void Update()
        {
            if (_progress.value < 1f)
            {
                _progress.value += Time.deltaTime;
            }
            else if (!_signing.rootGameObject.activeSelf)
            {
                _signing.rootGameObject.SetActive(true);
            }
        }

        private void Test()
        {
            var keyStore = Libplanet.KeyStore.Web3KeyStore.DefaultKeyStore;
            var keyList = keyStore.List().ToList();
            for (var i = keyList.Count; i > 0; i--)
            {
                Debug.Log(keyList[i - 1].ToString());
            }
        }

        public void ShowSigning(params string[] privateKeys)
        {
            var options = new List<string>(privateKeys);
            if (!_hasCreateNewPrivateKey)
            {
                options.Add("Create New One");
            }

            _signingOptions.Value = options;
            _signing.rootGameObject.SetActive(true);
        }
    }
}
