using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Building")]
    public class Building : ScriptableObject
    {
        public string buildingName;
        public Tilemap tilemap;
        public TileBase buildingTile;
    }
}
