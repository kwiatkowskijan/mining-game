using MiningGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MiningGame.UI
{
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private Stats statsComponent;

        private void Start()
        {
            if (statsComponent == null)
                statsComponent = FindFirstObjectByType<Stats>();

            UpdateHealthBar(statsComponent.CurrentHealth);
            statsComponent.OnHealthChanged += UpdateHealthBar;
        }

        private void UpdateHealthBar(float health)
        {
            float fill = health / statsComponent.maxHealth;
            healthBar.fillAmount = fill;
        }
    }
}
