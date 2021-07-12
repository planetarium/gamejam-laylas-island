using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace LaylasIsland.Frontend.UI
{
    public class MainCanvas : MonoBehaviour
    {
        [SerializeField] private Button _playerButton;
        [SerializeField] private Button _postButton;
        [SerializeField] private Button _goldButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _createGameButton;
        [SerializeField] private Button _joinGameButton;
        [SerializeField] private Button _createItemButton;
        [SerializeField] private Button _tradeButton;
        [SerializeField] private Button _creditButton;

        private void Awake()
        {
            _playerButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);
            
            _postButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);
            
            _goldButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);
            
            _settingsButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);
            
            _createGameButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);

            _joinGameButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);

            _createItemButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);

            _tradeButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);

            _creditButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                // Go to Main
            }).AddTo(gameObject);
        }
    }
}
