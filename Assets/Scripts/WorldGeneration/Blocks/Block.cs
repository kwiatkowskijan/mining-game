using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.WorldGeneration
{
    public class Block : ScriptableObject
    {
        public int id;
        public List<Tile> tiles;
        public bool isDescrutable = true;
    }
}
