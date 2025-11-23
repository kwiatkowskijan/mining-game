using UnityEngine;

namespace MiningGame.WorldGeneration
{
    [CreateAssetMenu(fileName = "Mineral", menuName = "Scriptable Objects/Block/Mineral")]
    public class Mineral : Block
    {
        public string mineralName;
        [Range(1, 100)]
        public int commonness = 50;
        public int minDepth = 100;
        public int maxDepth = -100;
        // public int rarity;
        public float miningMultiplier = 1.0f;
        public GameObject toSpawn;
        public Sprite icon;
        public bool isDiscovered = false;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(mineralName) || mineralName != this.name)
            {
                mineralName = this.name;
            }
        }
    }
}
