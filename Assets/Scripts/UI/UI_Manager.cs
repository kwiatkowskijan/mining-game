using MiningGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MiningGame.UI
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("UI Canvas Elements")]
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject debugPanel;

        [Header("Components")]
        [SerializeField] private Stats statsComponent;

        private void Start()
        {
            debugPanel.SetActive(false);

            if (statsComponent == null)
                statsComponent = FindFirstObjectByType<Stats>();

            UpdateHealthBar(statsComponent.CurrentHealth);
            statsComponent.OnHealthChanged += UpdateHealthBar;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F3))
            {
                ToggleDebugPanel();
            }
        }

        private void UpdateHealthBar(float health)
        {
            float fill = health / statsComponent.maxHealth;
            healthBar.fillAmount = fill;
        }

        private void ToggleDebugPanel()
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
        }
    }
}
