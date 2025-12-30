using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.WorldGeneration
{
    [CreateAssetMenu(fileName = "Block", menuName = "Scriptable Objects/Blocks")]
    public class Block : ScriptableObject
    {
        public string blockName;
        public List<Tile> tiles;
        public bool isDescrutable = true;
        public float miningMultiplier = 1f;
        public GameObject toSpawn;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(blockName) || blockName != this.name)
            {
                blockName = this.name;
            }
        }
    }
}
