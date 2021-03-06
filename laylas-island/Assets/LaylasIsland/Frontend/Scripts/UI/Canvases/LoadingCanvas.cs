using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Canvases
{
    public class LoadingCanvas : MonoBehaviour
    {
        #region View
        
        [SerializeField] private Slider _progress;
        
        #endregion
        
        private void OnEnable()
        {
            _progress.value = 0f;
        }

        private void Update()
        {
            if (_progress.value < 1f)
            {
                _progress.value += Time.deltaTime * .05f;
            }
        }
    }
}
