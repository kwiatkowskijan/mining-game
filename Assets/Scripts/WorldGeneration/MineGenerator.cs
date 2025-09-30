using System.Collections.Generic;
using MiningGame.MapGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.WorldGeneration
{
    public class MineGenerator : MonoBehaviour
    {
        private Tilemap _caveTilemap;
        [SerializeField] private List<Ore> ores;
        [SerializeField] private List<CommonBlock> commonBlocks;
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);
        [SerializeField] private float perlinNoiseScale;
        [SerializeField] private int chunkSize = 16;


        private void Awake()
        {
            _caveTilemap = GetComponentInChildren<Tilemap>();
        }

        private void Start()
        {
            GenerateCave(mapWidth, mapHeight);
        }

        private void GenerateCave(int width, int height)
        {
            int chunksX = Mathf.CeilToInt((float)width / chunkSize);
            int chunksY = Mathf.CeilToInt((float)height / chunkSize);

            for (int x = 0; x < chunksX; x++)
            {
                for (int y = 0; y < chunksY; y++)
                {
                    GenerateChunk(x, y);
                }
            }
        }

        private void GenerateChunk(int chunkX, int chunkY)
        {
            int startX = chunkX * chunkSize;
            int startY = chunkY * chunkSize;

            for (int x = startX; x < startX + chunkSize; x++)
            {
                for (int y = startY; y < startY + chunkSize; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(startPosition.x + x, startPosition.y - y, 0);
                    float noise = Mathf.PerlinNoise(x * perlinNoiseScale, y * perlinNoiseScale);

                    if (noise > 0.7f)
                        _caveTilemap.SetTile(tilePosition, ores.Find(t => t.isDescrutable).tile);
                    else
                        _caveTilemap.SetTile(tilePosition, commonBlocks.Find(t => t.isDescrutable).tile);
                }
            }
        }
    }
}
