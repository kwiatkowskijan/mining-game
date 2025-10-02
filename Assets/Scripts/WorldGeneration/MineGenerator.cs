using System;
using System.Collections.Generic;
using MiningGame.MapGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace MiningGame.WorldGeneration
{
    public class MineGenerator : MonoBehaviour
    {
        private Tilemap _caveTilemap;
        [SerializeField] private List<Ore> ores;
        [SerializeField] private List<CommonBlock> commonBlocks;
        [SerializeField] private Ore deafultOre;
        [SerializeField] private CommonBlock bedrock;
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private float perlinNoiseScale;
        [SerializeField] private int chunkSize = 16;
        [SerializeField] private Transform test;

        private Vector3Int _startPosition = new Vector3Int(0, 0, 0);


        private void Awake()
        {
            _caveTilemap = GetComponentInChildren<Tilemap>();
        }

        private void Start()
        {
            _startPosition = new Vector3Int(20, mapHeight / 2, 0);
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
                    Vector3Int tilePosition = new Vector3Int(_startPosition.x + x, _startPosition.y - y, 0);
                    float noise = Mathf.PerlinNoise(x * perlinNoiseScale, y * perlinNoiseScale);
                    if (tilePosition.y == -(mapHeight / 2) + 1 || tilePosition.y == mapHeight / 2)
                    {
                        _caveTilemap.SetTile(tilePosition, bedrock.tile);
                    }
                    else
                    {
                        if (noise > 0.8f)
                        {
                            Ore ore = ChooseOre(tilePosition.y);
                            _caveTilemap.SetTile(tilePosition, ore.tile);
                        }
                        else
                        {
                            _caveTilemap.SetTile(tilePosition, commonBlocks.Find(t => t.isDescrutable).tile);
                        }
                    }
                }
            }
        }


        private Ore ChooseOre(int currentDepth)
        {
            List<Ore> choosenOres = new List<Ore>();

            foreach (var ore in ores)
            {
                if (currentDepth <= ore.minDepth && currentDepth >= ore.maxDepth)
                {
                    for (int i = 0; i < ore.commonness; i++)
                        choosenOres.Add(ore);
                }
            }

            if (choosenOres.Count > 0)
            {
                int index = Random.Range(0, choosenOres.Count);
                return choosenOres[index];
            }

            return deafultOre;
        }
    }
}
