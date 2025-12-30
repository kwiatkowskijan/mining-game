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
        
        // Getter do odczytu mineralAmounts (dla save systemu)
        public Dictionary<Mineral, int> GetMineralAmounts() => mineralAmounts;

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

            if (currentHealth <= 0) 
            {
                Die();
            }
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
            if (ui != null)
            {
                ui.updateRubble(currentRubble, maxRubble);
            }
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

        // === METODY DO WCZYTYWANIA SAVE'A ===
        
        public void SetHealthFromSave(float health)
        {
            currentHealth = Mathf.Clamp(health, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }

        public void SetMoneyFromSave(float money)
        {
            currentMoney = money;
            OnMoneyChanged?.Invoke(currentMoney);
        }

        public void SetRubbleFromSave(float rubble)
        {
            currentRubble = Mathf.Clamp(rubble, 0, maxRubble);
            OnWeightChanged?.Invoke(currentRubble);
            UpdateRubbleUI();
        }

        public void SetMineralAmountsFromSave(Dictionary<Mineral, int> amounts)
        {
            mineralAmounts = amounts ?? new Dictionary<Mineral, int>();
            
            // Zaktualizuj UI dla wszystkich minerałów - tylko jeśli ui istnieje
            if (ui != null)
            {
                foreach (var kvp in mineralAmounts)
                {
                    ui.UpdateMineralNumber(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
