using UnityEngine;

namespace MiningGame.WorldGeneration
{
    [CreateAssetMenu(fileName = "Mineral", menuName = "Scriptable Objects/Block/Mineral")]
    public class Mineral : Block
    {
        public int minDepth = 100;
        public int maxDepth = -100;
        public Sprite icon;
        public bool isDiscovered = false;
    }
}
