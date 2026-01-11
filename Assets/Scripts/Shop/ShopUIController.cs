using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using MiningGame.WorldGeneration;
using MiningGame.Player;

namespace MiningGame.Shop
{
    public class ShopUIController : MonoBehaviour
    {

        [Header("UI References")]
        [SerializeField] private TMP_Text mineralsListText;
        [SerializeField] private Button sellAllButton;
        [SerializeField] private TMP_Text totalValueText;

        [Header("Player Stats")]
        [SerializeField] private Stats playerStats;

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
                }
            }
        }
        
        public void ShowPlayerMinerals()
        {
            if (playerStats == null)
            {
                return;
            }

            _currentMinerals = playerStats.GetMineralAmounts();

            if (_currentMinerals == null || _currentMinerals.Count == 0)
            {
                if (mineralsListText != null)
                {
                    mineralsListText.text = "No minerals in your inventory";
                }
                UpdateTotalValue(0f);
                gameObject.SetActive(true);
                return;
            }

            float totalValue = 0f;
            string mineralsText = "";

            foreach (var kvp in _currentMinerals)
            {
                Mineral mineral = kvp.Key;
                int amount = kvp.Value;

                if (amount <= 0) continue;

                float sellPrice = mineral.sellPrice;
                float itemTotal = sellPrice * amount;
                totalValue += itemTotal;
                
                mineralsText += $"{mineral.blockName} x{amount} - ${itemTotal:F0}\n";
            }

            if (mineralsListText != null)
            {
                mineralsListText.text = mineralsText;
            }

            UpdateTotalValue(totalValue);
            gameObject.SetActive(true);
        }

        private void OnSellAllClicked()
        {
            if (playerStats == null) return;

            var mineralAmounts = playerStats.GetMineralAmounts();
            if (mineralAmounts == null || mineralAmounts.Count == 0)
            {
                return;
            }

            float totalValue = 0f;
            List<Mineral> mineralsToSell = new List<Mineral>();

            foreach (var kvp in mineralAmounts)
            {
                if (kvp.Value > 0)
                {
                    totalValue += kvp.Key.sellPrice * kvp.Value;
                    mineralsToSell.Add(kvp.Key);
                }
            }

            if (totalValue <= 0)
            {
                return;
            }

            foreach (var mineral in mineralsToSell)
            {
                playerStats.ClearMineral(mineral);
            }

            playerStats.AddMoney(totalValue);


            ShowPlayerMinerals();
        }

        private void UpdateTotalValue(float value)
        {
            if (totalValueText != null)
            {
                totalValueText.text = $"Total: ${value:F0}";
            }

            if (sellAllButton != null)
            {
                sellAllButton.interactable = value > 0;
            }
        }

        public void Clear()
        {
            if (mineralsListText != null)
            {
                mineralsListText.text = "";
            }
            _currentMinerals = new Dictionary<Mineral, int>();
            UpdateTotalValue(0f);
        }
    }
}
