using System;
using System.Collections.Generic;
using MiningGame.WorldGeneration;
using MiningGame.Core.Interfaces;
using UnityEngine;

namespace MiningGame.Services
{
    public class MineralsService : IMineralsService
    {
        public event Action<Mineral> OnMineralDiscovered;
        public IReadOnlyList<Mineral> Minerals => _minerals;
        private readonly List<Mineral> _minerals;

        public MineralsService(List<Mineral> initialMinerals)
        {
            _minerals = initialMinerals ?? new List<Mineral>();
        }

        public void DiscoverMineral(Mineral mineral)
        {
            if (mineral == null)
            {
                Debug.LogWarning("Tried to discover a null mineral!");
                return;
            }

            if (mineral.isDiscovered)
                return;


            mineral.isDiscovered = true;
            Debug.Log("Mineral discovered: " + mineral.blockName);
            OnMineralDiscovered?.Invoke(mineral);
        }

        public void UndisoverAllMinerals()
        {
            foreach (var mineral in _minerals)
            {
                mineral.isDiscovered = false;
            }
        }
    }
}