using System.Collections.Generic;
using MiningGame.Core.Interfaces;
using MiningGame.WorldGeneration;
using MiningGame.Services;
using UnityEngine;

namespace MiningGame.Core
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private List<Mineral> mineralsDatabase;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var mineralsService = new MineralsService(mineralsDatabase);
            ServiceLocator.Register<IMineralsService>(mineralsService);

            Debug.Log("[Bootstrap] MineralsService registered!");
        }
    }
}