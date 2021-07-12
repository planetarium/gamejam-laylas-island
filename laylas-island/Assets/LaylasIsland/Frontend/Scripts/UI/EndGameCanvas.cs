using UnityEngine;

namespace LaylasIsland.Frontend.UI
{
    public class EndGameCanvas : MonoBehaviour
    {
        private void OnEnable()
        {
            UIHolder.HeaderCanvas.Show(
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Settings);
        }
    }
}
