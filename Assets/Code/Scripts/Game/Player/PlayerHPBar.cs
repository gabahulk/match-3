using Code.Scripts.SOArchitecture;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Game.Player
{
    public class PlayerHpBar : MonoBehaviour
    {
        public IntVariable playerHP;
        public Slider healthSlider;

        void Start()
        {
            playerHP.Changed += OnPlayerHPChanged;
            OnPlayerHPChanged();
        }

        void OnDestroy()
        {
            playerHP.Changed -= OnPlayerHPChanged;
        }

        private void OnPlayerHPChanged()
        {
            healthSlider.value = playerHP.Value;
        }
    }
}
