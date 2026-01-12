using MiningGame.Core;
using MiningGame.Core.Interfaces;
using MiningGame.Managers;
using MiningGame.Services;
using MiningGame.UI;
using MiningGame.WorldGeneration;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace MiningGame.Player
{
    public class Stats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] public float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        public float CurrentHealth => currentHealth;
        public event Action<float> OnHealthChanged;

        [Header("Rubble")]
        [SerializeField] private float maxRubble = 10f;
        [SerializeField] private float currentRubble;
        public float CurrentRubble => currentRubble;
        public float MaxRubble => maxRubble;
        public event Action<float> OnWeightChanged;

        [Header("Minerals")]
        private Dictionary<Mineral, int> mineralAmounts = new();
        
        public Dictionary<Mineral, int> GetMineralAmounts()
        {
            return mineralAmounts;
        }

        [Header("Money")]
        [SerializeField] private float currentMoney;
        public float CurrentMoney => currentMoney;
        public event Action<float> OnMoneyChanged;

        [Header("UI")]
        [SerializeField] private UI_Manager ui;

        private void Awake()
        {
            currentHealth = maxHealth;
            currentRubble = 0;
            currentMoney = 10000;

            var minerals = ServiceLocator.Get<IMineralsService>().Minerals;
            foreach (var mineral in minerals)
            {
                Debug.Log("Mineral: " + mineral.name);
            }
        }

        public void TakeDamage(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Die()
        {
            Debug.Log("Player is dead");
            //tutaj trzeba dodac jakas animacje, respawn albo cokolwiek
        }

        public void AddRubble(float amount)
        {
            currentRubble += amount;
            OnWeightChanged?.Invoke(currentRubble);
            UpdateRubbleUI();
        }

        public void RemoveRubble(float disposalRate)
        {
            currentRubble = Mathf.Max(0, currentRubble - disposalRate * Time.deltaTime);
            currentRubble = Mathf.Round(currentRubble * 100f) / 100f;

            UpdateRubbleUI();
        }

        public void UpdateRubbleUI()
        {
            ui.updateRubble(currentRubble, maxRubble);
        }

        public void AddMineral(Mineral mineral, int amount = 1)
        {
            if (!mineralAmounts.ContainsKey(mineral))
                mineralAmounts[mineral] = 0;

            mineralAmounts[mineral] += amount;
            
            if (ui != null)
            {
                ui.UpdateMineralNumber(mineral, mineralAmounts[mineral]);
            }
        }

        public int GetMineralAmount(Mineral mineral)
        {
            return mineralAmounts.TryGetValue(mineral, out int amt) ? amt : 0;
        }

        public void RemoveMineral(Mineral mineral, int amount)
        {
            if (!mineralAmounts.ContainsKey(mineral)) return;

            mineralAmounts[mineral] = Mathf.Max(0, mineralAmounts[mineral] - amount);
            
            if (ui != null)
            {
                ui.UpdateMineralNumber(mineral, mineralAmounts[mineral]);
            }
        }

        public void ClearMineral(Mineral mineral)
        {
            if (mineralAmounts.ContainsKey(mineral))
            {
                mineralAmounts[mineral] = 0;
                
                if (ui != null)
                {
                    ui.UpdateMineralNumber(mineral, 0);
                }
            }
        }

        public void AddMoney(float amount)
        {
            currentMoney += amount;
            OnMoneyChanged?.Invoke(currentMoney);
        }

        public void RemoveMoney(float amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
        }

        public void UpdateMoneyUI()
        {
            ui.UpdateMoneyNumber(currentMoney);
        }
    }
}
