using System;
using System.Collections.Generic;
using MiningGame.WorldGeneration;
using UnityEngine;

namespace MiningGame.Managers
{
    public class MineralsManager : MonoBehaviour
    {
        public static MineralsManager Instance { get; private set; }
        public event Action<Mineral> OnMineralDiscovered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public List<Mineral> minerals;

        public void DiscoverMineral(Mineral mineral)
        {
            Debug.Log("Mineral discovered: " + mineral.blockName);
            mineral.isDiscovered = true;
            OnMineralDiscovered?.Invoke(mineral);
        }
    }
}
