using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using MiningGame.WorldGeneration;
using MiningGame.Player;

namespace MiningGame.Shop
{
    /// <summary>
    /// Kontroler UI sklepu - pokazuje zebrane minerały z ikonkami i opcją sprzedaży
    /// </summary>
    public class ShopUIController : MonoBehaviour
    {
        [Header("Mineral Sell Prices")]
        [SerializeField] private MineralSellPrices mineralSellPrices;

        [Header("UI References")]
        [SerializeField] private Transform mineralsContainer;
        [SerializeField] private GameObject mineralEntryPrefab;
        [SerializeField] private Button sellAllButton;
        [SerializeField] private TMP_Text totalValueText;

        [Header("Player Stats")]
        [SerializeField] private Stats playerStats;

        private List<MineralShopEntry> _activeEntries = new List<MineralShopEntry>();
        private Dictionary<Mineral, int> _currentMinerals = new Dictionary<Mineral, int>();

        private void Awake()
        {
            if (sellAllButton != null)
            {
                sellAllButton.onClick.AddListener(OnSellAllClicked);
            }

            if (playerStats == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerStats = player.GetComponent<Stats>();
                    Debug.Log($"[SHOP] Found player: {player.name}, Stats component: {(playerStats != null ? "FOUND" : "NOT FOUND")}");
                }
                else
                {
                    Debug.LogError("[SHOP] Could not find GameObject with tag 'Player'!");
                }
            }
            else
            {
                Debug.Log($"[SHOP] PlayerStats already assigned in Inspector: {playerStats.gameObject.name}");
            }
        }

        /// <summary>
        /// Pokazuje minerały gracza w sklepie
        /// </summary>
        public void ShowPlayerMinerals()
        {
            Clear();

            if (playerStats == null)
            {
                Debug.LogError("ShopUIController: playerStats is null!");
                return;
            }

            if (mineralSellPrices == null)
            {
                Debug.LogError("ShopUIController: mineralSellPrices is null! Assign MineralSellPrices ScriptableObject.");
                return;
            }

            _currentMinerals = playerStats.GetMineralAmounts();

            Debug.Log($"[SHOP DEBUG] GetMineralAmounts returned: {(_currentMinerals == null ? "NULL" : $"Dictionary with {_currentMinerals.Count} entries")}");
            
            if (_currentMinerals != null && _currentMinerals.Count > 0)
            {
                Debug.Log("[SHOP DEBUG] Minerals in dictionary:");
                foreach (var kvp in _currentMinerals)
                {
                    Debug.Log($"  - {kvp.Key.blockName}: {kvp.Value}");
                }
            }

            if (_currentMinerals == null || _currentMinerals.Count == 0)
            {
                Debug.LogWarning($"[SHOP] Player has no minerals to sell. Dict is null: {_currentMinerals == null}, Count: {_currentMinerals?.Count ?? 0}");
                UpdateTotalValue(0f);
                return;
            }

            float totalValue = 0f;

            foreach (var kvp in _currentMinerals)
            {
                Mineral mineral = kvp.Key;
                int amount = kvp.Value;

                if (amount <= 0) continue;

                float sellPrice = mineralSellPrices.GetSellPrice(mineral);
                totalValue += sellPrice * amount;

                // Utwórz lub użyj istniejącego entry
                MineralShopEntry entry = GetOrCreateEntry();
                entry.Setup(mineral, amount, sellPrice, OnSellMineralClicked);
            }

            UpdateTotalValue(totalValue);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Sprzedaje pojedynczy minerał
        /// </summary>
        private void OnSellMineralClicked(Mineral mineral, int amount)
        {
            if (playerStats == null || mineralSellPrices == null) return;

            float totalValue = mineralSellPrices.CalculateTotalValue(mineral, amount);
            
            // Usuń minerał z ekwipunku gracza
            playerStats.ClearMineral(mineral);

            // Dodaj pieniądze
            playerStats.AddMoney(totalValue);

            Debug.Log($"ShopUIController: Sold {amount}x {mineral.blockName} for ${totalValue:F0}");

            // Odśwież UI
            ShowPlayerMinerals();
        }

        /// <summary>
        /// Sprzedaje wszystkie minerały naraz
        /// </summary>
        private void OnSellAllClicked()
        {
            if (playerStats == null || mineralSellPrices == null) return;

            var mineralAmounts = playerStats.GetMineralAmounts();
            if (mineralAmounts == null || mineralAmounts.Count == 0)
            {
                Debug.Log("ShopUIController: No minerals to sell.");
                return;
            }

            float totalValue = 0f;
            int totalMinerals = 0;
            List<Mineral> mineralsToSell = new List<Mineral>();

            // Oblicz całkowitą wartość
            foreach (var kvp in mineralAmounts)
            {
                if (kvp.Value > 0)
                {
                    totalValue += mineralSellPrices.CalculateTotalValue(kvp.Key, kvp.Value);
                    totalMinerals += kvp.Value;
                    mineralsToSell.Add(kvp.Key);
                }
            }

            if (totalValue <= 0)
            {
                Debug.Log("ShopUIController: No valuable minerals to sell.");
                return;
            }

            // Usuń wszystkie minerały
            foreach (var mineral in mineralsToSell)
            {
                playerStats.ClearMineral(mineral);
            }

            // Dodaj pieniądze
            playerStats.AddMoney(totalValue);

            Debug.Log($"ShopUIController: Sold ALL minerals ({totalMinerals} total) for ${totalValue:F0}");

            // Odśwież UI
            ShowPlayerMinerals();
        }

        /// <summary>
        /// Aktualizuje tekst całkowitej wartości
        /// </summary>
        private void UpdateTotalValue(float value)
        {
            if (totalValueText != null)
            {
                totalValueText.text = $"Total Value: ${value:F0}";
            }

            if (sellAllButton != null)
            {
                sellAllButton.interactable = value > 0;
            }
        }

        /// <summary>
        /// Pobiera lub tworzy nowy MineralShopEntry
        /// </summary>
        private MineralShopEntry GetOrCreateEntry()
        {
            // Sprawdź czy jest nieaktywny entry do użycia
            foreach (var entry in _activeEntries)
            {
                if (!entry.gameObject.activeSelf)
                {
                    return entry;
                }
            }

            // Utwórz nowy entry
            if (mineralEntryPrefab != null && mineralsContainer != null)
            {
                GameObject entryObj = Instantiate(mineralEntryPrefab, mineralsContainer);
                MineralShopEntry entry = entryObj.GetComponent<MineralShopEntry>();
                
                if (entry == null)
                {
                    entry = entryObj.AddComponent<MineralShopEntry>();
                }

                _activeEntries.Add(entry);
                return entry;
            }

            Debug.LogError("ShopUIController: Cannot create mineral entry - prefab or container is null!");
            return null;
        }

        /// <summary>
        /// Czyści wszystkie wpisy
        /// </summary>
        public void Clear()
        {
            foreach (var entry in _activeEntries)
            {
                if (entry != null)
                {
                    entry.Clear();
                }
            }

            _currentMinerals.Clear();
            UpdateTotalValue(0f);
        }
    }
}
