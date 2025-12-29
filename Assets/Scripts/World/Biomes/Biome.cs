using UnityEngine;
using System.Collections.Generic;

namespace MiningGame.WorldGeneration
{
    [CreateAssetMenu(fileName = "Biome", menuName = "Scriptable Objects/Biome")]
    public class Biome : ScriptableObject
    {
        public string biomeName;
        public int startY;
        public List<CommonBlock> commonBlocks;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(biomeName) || biomeName != this.name)
            {
                biomeName = this.name;
            }
        }
    }
}
