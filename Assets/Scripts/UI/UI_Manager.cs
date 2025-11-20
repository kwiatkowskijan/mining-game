using UnityEngine;
using UnityEngine.UI;
using MiningGame.Core;
using MiningGame.Managers;
using MiningGame.Player;
using MiningGame.WorldGeneration;
using MiningGame.Core.Interfaces;

namespace MiningGame.UI
{
    public class UI_Manager : MonoBehaviour
    {
        private IMineralsService mineralsService;
        private IAudioService audioService;
        private IMinimapService minimapService;



        [Header("UI Canvas Elements")]
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject debugPanel;
        [Header("Minimap")]
        [SerializeField] private Camera minimapCamera;

        [Header("Components")]
        [SerializeField] private Stats statsComponent;
        [Header("Minerals UI")]
        [SerializeField] private Transform mineralsListContent;
        [SerializeField] private MineralUiEntry mineralEntryPrefab;
        [SerializeField] private Sprite unknownSprite;
        [Header("Audio")]
        [SerializeField] private AudioClip clickSFX;


        private void Awake()
        {
            mineralsService = ServiceLocator.Get<IMineralsService>();
            audioService = ServiceLocator.Get<IAudioService>();
            minimapService = ServiceLocator.Get<IMinimapService>();
        }

        private void Start()
        {
            debugPanel.SetActive(false);

            if (statsComponent == null)
                statsComponent = FindFirstObjectByType<Stats>();

            UpdateHealthBar(statsComponent.CurrentHealth);
            statsComponent.OnHealthChanged += UpdateHealthBar;

            UpdateMineralsUI();

            if (mineralsService != null)
                mineralsService.OnMineralDiscovered += HandleMineralDiscovered;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                ToggleDebugPanel();
            }

            minimapCamera.orthographicSize = minimapService.CurrentZoom;
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

            foreach (var mineral in mineralsService.Minerals)
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

        public void ZoomInMinimap()
        {
            minimapService.ZoomIn(1f);
        }

        public void ZoomOutMinimap()
        {
            minimapService.ZoomOut(1f);
        }


        public void PlayClickAudio()
        {
            audioService.PlaySfx(clickSFX);
        }

        private void OnDisable()
        {
            if (mineralsService != null)
                mineralsService.OnMineralDiscovered -= HandleMineralDiscovered;
        }
    }
}
