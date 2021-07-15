using System;
using UnityEngine;
using UnityEngine.UI;

namespace LaylasIsland.Frontend.UI.Modules
{
    public class DiceGroup : MonoBehaviour
    {
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Swing = Animator.StringToHash("Swing");

        [SerializeField] private Animator _animator;
        [SerializeField] private Image _diceImage;
        [SerializeField] private DiceSpritesSO _spritesSo;

        private int _number = -1;

        public void ShowAndPlay(int? number)
        {
            if (number.HasValue)
            {
                _number = number.Value;
            }
            else
            {
                _number = -1;
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);    
            }
            else
            {
                _animator.Play("Show");
            }
        }

        /// <param name="number">1...6</param>
        public void PlaySwing(int number)
        {
            if (number < 1 || number > 6)
            {
                throw new Exception($"{nameof(number)} should between 1 to 6");
            }

            _number = number;
            _animator.SetTrigger(Swing);
        }

        public void PlayClose()
        {
            _animator.SetTrigger(Close);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateDiceImage()
        {
            _diceImage.overrideSprite = _spritesSo.NumberSprites[_number - 1];
        }

        public void OnAnimationEnd(string animationName)
        {
            switch (animationName)
            {
                case "Show":
                    if (_number != -1)
                    {
                        PlaySwing(_number);
                    }
                    break;
                case "Swing":
                    PlayClose();
                    break;
                case "Close":
                    Hide();
                    break;
            }
        }
    }
}
