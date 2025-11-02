using UnityEngine;

namespace MiningGame.MapGeneration
{
    [CreateAssetMenu(fileName = "Ore", menuName = "Scriptable Objects/Block/Ore")]
    public class Ore : Block
    {
        [Range(1, 100)]
        public int commonness;
        public int minDepth;
        public int maxDepth;
        public int rarity;
        public float miningMultiplier;
        public GameObject toSpawn;
    }
}
