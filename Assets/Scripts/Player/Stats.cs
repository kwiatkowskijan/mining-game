using MiningGame.Core;
using MiningGame.Core.Interfaces;
using MiningGame.Managers;
using MiningGame.Services;
using MiningGame.WorldGeneration;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public float CurrentWeight => currentRubble;
        public event Action<float> OnWeightChanged;

        [Header("Minerals")]

        [Header("Money")]
        [SerializeField] private float currentMoney;
        public float CurrentMoney => currentMoney;
        public event Action<float> OnMoneyChanged;

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

        public void AddWeight(float amount)
        {
            currentRubble += amount;
            OnWeightChanged?.Invoke(currentRubble);
            
            if (currentRubble >= maxRubble) 
            {
                Debug.Log("Player is overloaded");
                //jakas mechanika obciazenia
            }
        }

        public void RemoveWeight(float amount)
        {
            currentRubble -= amount;
            OnWeightChanged?.Invoke(currentRubble);

            //tutaj też to bedzie trzeba rozwinac pewnie
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
    }
}
