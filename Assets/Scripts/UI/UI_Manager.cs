using UnityEngine;
using UnityEngine.UI;
using MiningGame.Managers;
using MiningGame.Player;
using MiningGame.WorldGeneration;

namespace MiningGame.UI
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("UI Canvas Elements")]
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject debugPanel;

        [Header("Components")]
        [SerializeField] private Stats statsComponent;
        [Header("Minerals UI")]
        [SerializeField] private Transform mineralsListContent;
        [SerializeField] private MineralUiEntry mineralEntryPrefab;
        [SerializeField] private Sprite unknownSprite;


        private void Start()
        {
            debugPanel.SetActive(false);

            if (statsComponent == null)
                statsComponent = FindFirstObjectByType<Stats>();

            UpdateHealthBar(statsComponent.CurrentHealth);
            statsComponent.OnHealthChanged += UpdateHealthBar;

            UpdateMineralsUI();

            if (MineralsManager.Instance != null)
                MineralsManager.Instance.OnMineralDiscovered += HandleMineralDiscovered;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
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

        private void HandleMineralDiscovered(Mineral mineral)
        {
            UpdateMineralsUI();
        }

        public void UpdateMineralsUI()
        {
            foreach (Transform child in mineralsListContent)
                Destroy(child.gameObject);

            foreach (var mineral in MineralsManager.Instance.minerals)
            {
                var entry = Instantiate(mineralEntryPrefab, mineralsListContent);

                if (mineral.isDiscovered)
                {
                    entry.iconImage.sprite = mineral.icon;
                    entry.nameText.text = mineral.mineralName;
                }
                else
                {
                    entry.iconImage.sprite = unknownSprite;
                    entry.nameText.text = "???";
                }
            }
        }

        private void OnDisable()
        {
            if (MineralsManager.Instance != null)
                MineralsManager.Instance.OnMineralDiscovered -= HandleMineralDiscovered;
        }
    }
}
