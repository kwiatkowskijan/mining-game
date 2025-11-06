using UnityEngine;

namespace MiningGame.WorldGeneration
{
    [CreateAssetMenu(fileName = "Ore", menuName = "Scriptable Objects/Block/Ore")]
    public class Mineral : Block
    {
        public string mineralName;
        [Range(1, 100)]
        public int commonness;
        public int minDepth;
        public int maxDepth;
        public int rarity;
        public float miningMultiplier;
        public GameObject toSpawn;
        public Sprite icon;
        public bool isDiscovered = false;
    }
}
