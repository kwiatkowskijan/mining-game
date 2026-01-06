using UnityEngine;
using TMPro;
using System.Collections.Generic;
using MiningGame.WorldGeneration;
using MiningGame.Player;

namespace MiningGame.UI
{
    public class InventoryUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private TMP_Text mineralsListText;

        [Header("Player Stats")]
        [SerializeField] private Stats playerStats;

        private bool _isInventoryOpen = false;

        private void Awake()
        {
            if (playerStats == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerStats = player.GetComponent<Stats>();
                }
            }

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }

            if (_isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }

        private void ToggleInventory()
        {
            if (_isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        private void OpenInventory()
        {
            _isInventoryOpen = true;
            ShowPlayerMinerals();

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }
        }

        private void CloseInventory()
        {
            _isInventoryOpen = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            Clear();
        }

        private void ShowPlayerMinerals()
        {
            if (playerStats == null)
            {
                return;
            }

            var mineralAmounts = playerStats.GetMineralAmounts();

            if (mineralAmounts == null || mineralAmounts.Count == 0)
            {
                if (mineralsListText != null)
                {
                    mineralsListText.text = "Brak minerałów";
                }
                return;
            }

            string mineralsText = "";

            foreach (var kvp in mineralAmounts)
            {
                Mineral mineral = kvp.Key;
                int amount = kvp.Value;

                if (amount <= 0) continue;

                mineralsText += $"{mineral.blockName} x{amount}\n";
            }

            if (string.IsNullOrEmpty(mineralsText))
            {
                mineralsText = "Brak minerałów";
            }

            if (mineralsListText != null)
            {
                mineralsListText.text = mineralsText;
            }
        }

        private void Clear()
        {
            if (mineralsListText != null)
            {
                mineralsListText.text = "";
            }
        }
    }
}

