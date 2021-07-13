using System;
using System.Collections;
using System.IO;
using Bencodex.Types;
using Boscohyun;
using LaylasIsland.Backend.State;
using Libplanet;
using LaylasIsland.Frontend.BlockChain;
using LaylasIsland.Frontend.Game;
using LaylasIsland.Frontend.State;
using LaylasIsland.Frontend.UI;
using Libplanet.Crypto;
using UnityEngine;

namespace LaylasIsland.Frontend
{
    using UniRx;

    public class MainController : MonoSingleton<MainController>
    {
        [SerializeField] private Agent _agent;

        private static readonly string CommandLineOptionsJsonPath =
            Path.Combine(Application.streamingAssetsPath, "clo.json");

        private CommandLineOptions _options;

        public IAgent Agent => _agent;

        public States States { get; private set; }

        public LocalLayer LocalLayer { get; private set; }

        public ActionManager ActionManager { get; private set; }
        
        public bool HasSignedIn { get; private set; }

        #region Mono & Initialization

        protected void Awake()
        {
            if (!IsValidInstance())
            {
                return;
            }

            _options = CommandLineOptions.Load(CommandLineOptionsJsonPath);
            if (_options.RpcClient)
            {
                // Agent = GetComponent<RPCAgent>();
                // SubscribeRPCAgent();
                throw new Exception("Does Not Support RPC Agent");
            }

            States = new States();
            LocalLayer = new LocalLayer();
        }

        private IEnumerator Start()
        {
            // Sign-in
            yield return StartCoroutine(CoSignIn(succeed => HasSignedIn = succeed));
            Debug.Log($"Agent has signed-in. {HasSignedIn}");
        }

        #endregion

        private IEnumerator CoSignIn(Action<bool> callback)
        {
            PrivateKey privateKey = null;
            if (string.IsNullOrEmpty(_options.PrivateKey))
            {
                var onClickSigning = false;
                UIHolder.IntroCanvas.OnClickSigning.First().Subscribe(_ => onClickSigning = true);
                UIHolder.IntroCanvas.ShowSigning();
                yield return new WaitUntil(() => onClickSigning);

                privateKey = UIHolder.IntroCanvas.SelectedPrivateKey;
            }
            else
            {
                privateKey = new PrivateKey(ByteUtil.ParseHex(_options.PrivateKey));
            }

            Agent.Initialize(_options, privateKey, success =>
            {
                if (!success)
                {
                    throw new Exception("Agent initialization failed");
                }

                ActionManager = new ActionManager(Agent);

                var agentStateValue = Agent.GetState(privateKey.ToAddress());
                if (agentStateValue is null)
                {
                    // Sign-up
                    UIHolder.LoadingCanvas.gameObject.SetActive(true);
                    ActionManager.SignUp().Subscribe(eval =>
                    {
                        if (!(eval.Exception is null))
                        {
                            Debug.LogError(eval.Exception.ToString());
                            throw eval.Exception;
                        }

                        // Sign-in
                        UIHolder.LoadingCanvas.gameObject.SetActive(false);
                        UIHolder.IntroCanvas.gameObject.SetActive(false);
                        UIHolder.MainCanvas.gameObject.SetActive(true);
                        callback?.Invoke(success);
                    });
                }
                else
                {
                    // Sign-in
                    States.Instance.SetAgentState(new AgentState((Dictionary) agentStateValue));
                    UIHolder.IntroCanvas.gameObject.SetActive(false);
                    UIHolder.MainCanvas.gameObject.SetActive(true);
                    callback?.Invoke(success);
                }
            });
        }

        public void ResetStore()
        {
//             var confirm = Widget.Find<Confirm>();
//             var storagePath = _options.StoragePath ?? BlockChain.Agent.DefaultStoragePath;
//             confirm.CloseCallback = result =>
//             {
//                 if (result == ConfirmResult.No)
//                 {
//                     return;
//                 }
//
//                 StoreUtils.ResetStore(storagePath);
//
// #if UNITY_EDITOR
//                 UnityEditor.EditorApplication.ExitPlaymode();
// #else
//                 Application.Quit();
// #endif
//             };
//             confirm.Show("UI_CONFIRM_RESET_STORE_TITLE", "UI_CONFIRM_RESET_STORE_CONTENT");
        }
    }
}
