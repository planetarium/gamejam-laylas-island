using LaylasIsland.Frontend.UI;
using UnityEngine;

namespace LaylasIsland.Frontend.UI.Canvases
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
