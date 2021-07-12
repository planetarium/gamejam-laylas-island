using System;
using System.Collections;
using System.IO;
using Boscohyun;
using LibUnity.Frontend.BlockChain;
using LibUnity.Frontend.State;
using UnityEngine;

namespace LibUnity.Frontend
{
    using UniRx;

    [RequireComponent(typeof(Agent), typeof(RPCAgent))]
    public class Game : MonoSingleton<Game>
    {
        public IAgent Agent { get; private set; }
        
        public States States { get; private set; }

        public LocalLayer LocalLayer { get; private set; }

        public ActionManager ActionManager { get; private set; }
        public bool IsInitialized { get; private set; }

        private CommandLineOptions _options;

        private static readonly string CommandLineOptionsJsonPath =
            Path.Combine(Application.streamingAssetsPath, "clo.json");

        #region Mono & Initialization

        protected void Awake()
        {
            if (!IsValidInstance())
            {
                return;
            }
            
            _options = CommandLineOptions.Load(
                CommandLineOptionsJsonPath
            );

            if (_options.RpcClient)
            {
                Agent = GetComponent<RPCAgent>();
                SubscribeRPCAgent();
            }
            else
            {
                Agent = GetComponent<Agent>();
            }

            States = new States();
            LocalLayer = new LocalLayer();
        }

        private IEnumerator Start()
        {
            // Initialize Agent
            var agentInitialized = false;
            var agentInitializeSucceed = false;
            yield return StartCoroutine(
                CoLogin(
                    succeed =>
                    {
                        Debug.Log($"Agent initialized. {succeed}");
                        agentInitialized = true;
                        agentInitializeSucceed = succeed;
                    }
                )
            );

            yield return new WaitUntil(() => agentInitialized);
            
            ActionManager = new ActionManager(Agent);
            
            if (agentInitializeSucceed)
            {
                IsInitialized = true;
            }
            else
            {
                QuitWithAgentConnectionError(null);
            }
        }

        private void SubscribeRPCAgent()
        {
            if (!(Agent is RPCAgent rpcAgent))
            {
                return;
            }

            Debug.Log("[Game]Subscribe RPCAgent");

            rpcAgent.OnRetryStarted
                .ObserveOnMainThread()
                .Subscribe(agent =>
                {
                    Debug.Log($"[Game]RPCAgent OnRetryStarted. {rpcAgent.Address.ToHex()}");
                    OnRPCAgentRetryStarted(agent);
                })
                .AddTo(gameObject);

            rpcAgent.OnRetryEnded
                .ObserveOnMainThread()
                .Subscribe(agent =>
                {
                    Debug.Log($"[Game]RPCAgent OnRetryEnded. {rpcAgent.Address.ToHex()}");
                    OnRPCAgentRetryAndPreloadEnded(agent);
                })
                .AddTo(gameObject);

            rpcAgent.OnPreloadStarted
                .ObserveOnMainThread()
                .Subscribe(agent =>
                {
                    Debug.Log($"[Game]RPCAgent OnPreloadStarted. {rpcAgent.Address.ToHex()}");
                    OnRPCAgentRetryAndPreloadEnded(agent);
                })
                .AddTo(gameObject);

            rpcAgent.OnPreloadEnded
                .ObserveOnMainThread()
                .Subscribe(agent =>
                {
                    Debug.Log($"[Game]RPCAgent OnPreloadEnded. {rpcAgent.Address.ToHex()}");
                    OnRPCAgentRetryAndPreloadEnded(agent);
                })
                .AddTo(gameObject);

            rpcAgent.OnDisconnected
                .ObserveOnMainThread()
                .Subscribe(agent =>
                {
                    Debug.Log($"[Game]RPCAgent OnDisconnected. {rpcAgent.Address.ToHex()}");
                    QuitWithAgentConnectionError(agent);
                })
                .AddTo(gameObject);
        }

        private static void OnRPCAgentRetryStarted(RPCAgent rpcAgent)
        {
        }

        private static void OnRPCAgentRetryAndPreloadEnded(RPCAgent rpcAgent)
        {
        }

        private void QuitWithAgentConnectionError(RPCAgent rpcAgent)
        {
        }

        #endregion

        private IEnumerator CoLogin(Action<bool> callback)
        {
            yield break;
//             if (_options.Maintenance)
//             {
//                 var w = Widget.Create<SystemPopup>();
//                 w.CloseCallback = () =>
//                 {
//                     Application.OpenURL(GameConfig.DiscordLink);
// #if UNITY_EDITOR
//                     UnityEditor.EditorApplication.ExitPlaymode();
// #else
//                     Application.Quit();
// #endif
//                 };
//                 w.Show(
//                     "UI_MAINTENANCE",
//                     "UI_MAINTENANCE_CONTENT",
//                     "UI_OK"
//                 );
//                 yield break;
//             }
//
//             if (_options.TestEnd)
//             {
//                 var w = Widget.Find<Confirm>();
//                 w.CloseCallback = result =>
//                 {
//                     if (result == ConfirmResult.Yes)
//                     {
//                         Application.OpenURL(GameConfig.DiscordLink);
//                     }
//
// #if UNITY_EDITOR
//                     UnityEditor.EditorApplication.ExitPlaymode();
// #else
//                     Application.Quit();
// #endif
//                 };
//                 w.Show("UI_TEST_END", "UI_TEST_END_CONTENT", "UI_GO_DISCORD", "UI_QUIT");
//
//                 yield break;
//             }
//
//             var settings = Widget.Find<UI.Settings>();
//             settings.UpdateSoundSettings();
//             settings.UpdatePrivateKey(_options.PrivateKey);
//
//             var loginPopup = Widget.Find<LoginPopup>();
//
//             if (Application.isBatchMode)
//             {
//                 loginPopup.Show(_options.KeyStorePath, _options.PrivateKey);
//             }
//             else
//             {
//                 var intro = Widget.Find<Intro>();
//                 intro.Show(_options.KeyStorePath, _options.PrivateKey);
//                 yield return new WaitUntil(() => loginPopup.Login);
//             }
//
//             Agent.Initialize(
//                 _options,
//                 loginPopup.GetPrivateKey(),
//                 callback
//             );
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
