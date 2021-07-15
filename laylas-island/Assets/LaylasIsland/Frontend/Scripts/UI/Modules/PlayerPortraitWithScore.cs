using LaylasIsland.Frontend.Game;
using TMPro;
using UnityEngine;

namespace LaylasIsland.Frontend.UI.Modules
{
    using UniRx;

    public class PlayerPortraitWithScore : MonoBehaviour
    {
        // View

        [SerializeField] private PlayerPortrait _portrait;
        [SerializeField] private TextMeshProUGUI _scoreText;

        // ~View

        // Model

        private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>();

        // ~Model

        private void Awake()
        {
            _score.Subscribe(value =>
            {
                _scoreText.text = value == 0
                    ? "-"
                    : value.ToString();
            }).AddTo(gameObject);
        }

        public void Show(Player player, int score = default)
        {
            _portrait.Show(player);
            _score.Value = score;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
