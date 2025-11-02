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
        private Tilemap _mineTilemap;
        [Header("Blocks")]
        [SerializeField] private List<Ore> ores;
        [SerializeField] private List<CommonBlock> commonBlocks;
        [SerializeField] private Ore deafultOre;
        [SerializeField] private CommonBlock bedrock;
        [Header("Map Settings")]
        [SerializeField] private int startX;
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private int chunkSize = 16;
        [SerializeField] private int loadDistance = 2;
        [Header("Perlin Noise Settings")]
        [Range(-1000000, 1000000)][SerializeField] private int seed = 0;
        [Range(0f, 1f)][SerializeField] private float mineNoiseScale = 0.13f;
        [Range(0f, 1f)][SerializeField] private float oreNoiseScale = 0.05f;
        [Header("Structures")]
        [SerializeField] private List<Structure> structures;



        private Vector3Int _startPosition = new Vector3Int(0, 0, 0);
        private Transform _player;
        private Dictionary<Vector2Int, bool> _generatedChunks = new Dictionary<Vector2Int, bool>();

        public static Dictionary<TileBase, Block> TileToBlockMap = new Dictionary<TileBase, Block>();


        private void Awake()
        {
            _mineTilemap = GetComponentInChildren<Tilemap>();

            foreach (var ore in ores)
            {
                if (ore.tile != null && !TileToBlockMap.ContainsKey(ore.tile))
                    TileToBlockMap.Add(ore.tile, ore);
            }
            foreach (var dirt in commonBlocks)
            {
                if (dirt.tile != null && !TileToBlockMap.ContainsKey(dirt.tile))
                    TileToBlockMap.Add(dirt.tile, dirt);
            }
        }

        private void Start()
        {
            InitValues();
            StartCoroutine(UpdateChunks());
        }

        private void InitValues()
        {
            _startPosition = new Vector3Int(startX, 0, 0);
            if (seed == 0)
                seed = Random.Range(-1000000, 1000000);
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Generate the entire mine at once (lef for testing - not used in final implementation)
        private void GenerateMine(int width, int height)
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
                    float mineNoise = Mathf.PerlinNoise((x + seed) * mineNoiseScale, (y + seed) * mineNoiseScale);

                    if (tilePosition.y == -(mapHeight / 2) + 1 || tilePosition.y == mapHeight / 2)
                    {
                        _mineTilemap.SetTile(tilePosition, bedrock.tile);
                    }
                    else
                    {
                        if (mineNoise > 0.8f)
                        {
                            float oreNoise = Mathf.PerlinNoise((x + seed) * oreNoiseScale, (y + seed) * oreNoiseScale);
                            Ore ore = ChooseOreFromNoise(oreNoise, y);
                            _mineTilemap.SetTile(tilePosition, ore.tile);
                        }
                        else
                        {
                            _mineTilemap.SetTile(tilePosition, commonBlocks.Find(t => t.isDescrutable).tile);
                        }
                    }
                }
            }
            TryPlaceStructure(chunkX, chunkY);
        }


        private Ore ChooseOreFromNoise(float noiseValue, int y)
        {
            List<Ore> validOres = ores.FindAll(o => y <= o.minDepth && y >= o.maxDepth);

            if (validOres.Count == 0) return deafultOre;

            int index = Mathf.FloorToInt(noiseValue * validOres.Count);
            index = Mathf.Clamp(index, 0, validOres.Count - 1);
            return validOres[index];
        }

        private void TryPlaceStructure(int chunkX, int chunkY)
        {
            foreach (var structure in structures)
            {
                if (Random.value < structure.spawnChance)
                {
                    int startX = chunkX * chunkSize + Random.Range(0, chunkSize - structure.width);
                    int startY = chunkY * chunkSize + Random.Range(0, chunkSize - structure.height);

                    Vector3Int worldPos = new Vector3Int(
                        _startPosition.x + startX,
                        _startPosition.y + startY,
                        0
                    );

                    PlaceStructure(structure, worldPos);

                }
            }
        }

        private void PlaceStructure(Structure structure, Vector3Int position)
        {
            for (int x = 0; x < structure.width; x++)
            {
                for (int y = 0; y < structure.height; y++)
                {
                    TileBase tile = structure.GetTile(x, y);
                    if (tile != null)
                    {
                        _mineTilemap.SetTile(new Vector3Int(position.x + x, position.y + y, 0), tile);
                    }
                }
            }
        }


        // Gizmos to visualize generated chunks in the editor
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
