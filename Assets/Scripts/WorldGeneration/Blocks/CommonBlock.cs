using UnityEngine;


namespace MiningGame.WorldGeneration
{
    [CreateAssetMenu(fileName = "Common", menuName = "Scriptable Objects/Block/Common")]
    public class CommonBlock : Block
    {
        public float miningMultiplier;
        public GameObject toSpawn;
        public Biome biome;
    }
}
