using LaylasIsland.Frontend.Game;
using UniRx;
using UnityEngine;

namespace LaylasIsland.Frontend.UI
{
    public class PrepareGameCanvas : MonoBehaviour
    {
        private void OnEnable()
        {
            UIHolder.HeaderCanvas.Show(
                () =>
                {
                    GameController.Instance.TerminateAsObservable().First().Subscribe(_ =>
                    {
                        gameObject.SetActive(false);
                        UIHolder.MainCanvas.gameObject.SetActive(true);
                    });
                },
                HeaderCanvas.Element.Back,
                HeaderCanvas.Element.Player,
                HeaderCanvas.Element.Gold,
                HeaderCanvas.Element.Settings);
        }
    }
}
