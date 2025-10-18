using System;
using System.Collections;
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
        [Header("Blocks")]
        [SerializeField] private List<Ore> ores;
        [SerializeField] private List<CommonBlock> commonBlocks;
        [SerializeField] private Ore deafultOre;
        [SerializeField] private CommonBlock bedrock;
        [Header("Map Settings")]
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private int chunkSize = 16;
        [SerializeField] private int loadDistance = 2;
        [Header("Perlin Noise Settings")]
        [Range(-1000000, 1000000)][SerializeField] private int seed = 0;
        [Range(0f, 1f)][SerializeField] private float noiseScale = 0.1f;

        private Vector3Int _startPosition = new Vector3Int(0, 0, 0);
        private Transform _player;
        private Dictionary<Vector2Int, bool> _generatedChunks = new Dictionary<Vector2Int, bool>();


        private void Awake()
        {
            _caveTilemap = GetComponentInChildren<Tilemap>();
        }

        private void Start()
        {
            InitValues();
            // GenerateCave(mapWidth, mapHeight);
            StartCoroutine(UpdateChunks());
        }

        private void InitValues()
        {
            // _startPosition = new Vector3Int(20, mapHeight / 2, 0);
            _startPosition = new Vector3Int(20, 0, 0);
            if (seed == 0)
                seed = Random.Range(-1000000, 1000000);
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Generate the entire cave at once (lef for testing - not used in final implementation)
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

        private IEnumerator UpdateChunks()
        {
            while (true)
            {
                Vector2Int playerChunk = GetPlayerChunk();
                loadChunksNearPlayer(playerChunk);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private Vector2Int GetPlayerChunk()
        {
            int chunkX = Mathf.FloorToInt((_player.position.x - _startPosition.x) / chunkSize);
            int chunkY = Mathf.FloorToInt((_player.position.y - _startPosition.y) / chunkSize);
            return new Vector2Int(chunkX, chunkY);
        }

        private void loadChunksNearPlayer(Vector2Int playerChunk)
        {
            for (int x = Mathf.Max(0, playerChunk.x - loadDistance); x <= playerChunk.x + loadDistance; x++)
            {
                for (int y = playerChunk.y - loadDistance; y <= playerChunk.y + loadDistance; y++)
                {
                    Vector2Int chunkCoord = new Vector2Int(x, y);
                    if (!_generatedChunks.ContainsKey(chunkCoord))
                    {
                        GenerateChunk(x, y);
                        _generatedChunks[chunkCoord] = true;
                    }
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
                    Vector3Int tilePosition = new Vector3Int(_startPosition.x + x, _startPosition.y + y, 0);
                    float noise = Mathf.PerlinNoise((x + seed) * noiseScale, (y + seed) * noiseScale);

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

        private void OnDrawGizmos()
        {
            foreach (var chunk in _generatedChunks)
            {
                Vector3 worldPos = new Vector3(
                    _startPosition.x + chunk.Key.x * chunkSize,
                    _startPosition.y + chunk.Key.y * chunkSize,
                    0
                );

                if (chunk.Key == new Vector2Int(0, 0))
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawWireCube(
                    worldPos + new Vector3(chunkSize / 2f, chunkSize / 2f, 0),
                    new Vector3(chunkSize, chunkSize, 0.1f)
                );
            }

        }
    }
}
