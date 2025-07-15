using UnityEngine;

namespace MiningGame.MapGeneration
{
    [CreateAssetMenu(fileName = "TileSO", menuName = "Scriptable Objects/TileSO/OreTile")]
    public class OreTile : TileSO
    {
        public int rarity;
    }
}
