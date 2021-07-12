using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace LaylasIsland.Frontend.UI
{
    public class IntroCanvas : MonoBehaviour
    {
        #region View
        
        [SerializeField] private Slider _progress;
        [SerializeField] private Button _signInButton;

        #endregion
        
        private void Awake()
        {
            // View
            _signInButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.MainCanvas.gameObject.SetActive(true);
            }).AddTo(gameObject);
            // ~View
        }

        private void OnEnable()
        {
            UIHolder.HeaderCanvas.gameObject.SetActive(false);
            _progress.value = 0f;
        }

        private void Update()
        {
            if (_progress.value < 1f)
            {
                _progress.value += Time.deltaTime * .2f;
            }
        }
    }
}
