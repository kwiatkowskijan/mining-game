using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.MapGeneration
{
    public class Block : ScriptableObject
    {
        public Tile tile;
        public bool isDescrutable;

        //TODO: add more properties as needed
    }
}
