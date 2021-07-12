using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace LaylasIsland.Frontend.UI
{
    public class IntroCanvas : MonoBehaviour
    {
        [SerializeField] private Slider _progress;
        [SerializeField] private Button _signInButton;

        private void Awake()
        {
            _signInButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Play Click SFX
                gameObject.SetActive(false);
                UIHolder.MainCanvas.gameObject.SetActive(true);
            }).AddTo(gameObject);
        }

        private void OnEnable()
        {
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
