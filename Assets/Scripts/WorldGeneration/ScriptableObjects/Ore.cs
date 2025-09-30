using UnityEngine;

namespace MiningGame.MapGeneration
{
    [CreateAssetMenu(fileName = "Ore", menuName = "Scriptable Objects/Block/Ore")]
    public class Ore : Block
    {
        public int rarity;
    }
}
