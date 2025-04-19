using System;
using UnityEngine;

namespace MiningGame.Player
{
    public class Stats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        public float CurrentHealth => currentHealth;
        public event Action<float> OnHealthChanged;

        [Header("Weight")]
        [SerializeField] private float maxWeight = 100f;
        [SerializeField] private float currentWeight;
        public float CurrentWeight => currentWeight;
        public event Action<float> OnWeightChanged;

        [Header("Money")]
        [SerializeField] private float currentMoney;
        public float CurrentMoney => currentMoney;
        public event Action<float> OnMoneyChanged;

        private void Awake()
        {
            
            currentHealth = maxHealth;
            currentWeight = 0;
            currentMoney = 0;
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
            currentWeight += amount;
            OnWeightChanged?.Invoke(currentWeight);
            
            if (currentWeight >= maxWeight) 
            {
                Debug.Log("Player is overloaded");
                //jakaœ mechanika obi¹¿enia
            }
        }

        public void RemoveWeight(float amount)
        {
            currentWeight -= amount;
            OnWeightChanged?.Invoke(currentWeight);

            //tutaj te¿ to bêdzie trzeba rozwin¹æ pewnie
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
