using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace MiningGame.WorldGeneration
{
    public class MineGenerator : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField] private Tilemap mineTilemap;
        [SerializeField] private Tilemap backgroundTilemap;
        [Header("Biomes")]
        [SerializeField] private List<Biome> biomes;
        [Header("Blocks")]
        [SerializeField] private List<Mineral> minerals;
        [SerializeField] private Mineral defaultMineral;
        [SerializeField] private Bedrock bedrock;
        [SerializeField] private Tile caveBackgroundTile;
        [Header("Map Settings")]
        [SerializeField] private int startX;
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private int chunkSize = 16;
        [SerializeField] private int loadDistance = 2;
        [Header("Perlin Noise Settings")]
        [Range(-1000000, 1000000)][SerializeField] private int seed = 0;
        [Range(0f, 1f)][SerializeField] private float mineNoiseScale = 0.13f;
        [Range(0f, 1f)][SerializeField] private float mineralNoiseScale = 0.05f;
        private Vector3Int _startPosition = new Vector3Int(0, 0, 0);
        private Transform _player;
        private Dictionary<Vector2Int, bool> _generatedChunks = new Dictionary<Vector2Int, bool>();
        public static Dictionary<TileBase, Block> TileToBlockMap = new Dictionary<TileBase, Block>();

        private void Awake()
        {
            MapTileToBlock();
        }

        private void Start()
        {
            InitValues();
            StartCoroutine(UpdateChunks());
        }

        private void InitValues()
        {
            seed = seed == 0 ? Random.Range(-1000000, 1000000) : seed;
            _startPosition = new Vector3Int(startX, 0, 0);
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            biomes.Sort((a, b) => a.startY.CompareTo(b.startY));
        }

        private void MapTileToBlock()
        {
            if (defaultMineral != null && defaultMineral.tiles != null)
            {
                foreach (var tile in defaultMineral.tiles)
                {
                    if (!TileToBlockMap.ContainsKey(tile))
                        TileToBlockMap.Add(tile, defaultMineral);
                }
            }
            foreach (var mineral in minerals)
            {
                if (mineral.tiles != null)
                {
                    foreach (var tile in mineral.tiles)
                    {
                        if (!TileToBlockMap.ContainsKey(tile))
                            TileToBlockMap.Add(tile, mineral);
                    }
                }
            }
            foreach (var biome in biomes)
            {
                if (biome == null || biome.commonBlocks == null) continue;
                foreach (var block in biome.commonBlocks)
                {
                    if (block.tiles == null) continue;
                    foreach (var tile in block.tiles)
                    {
                        if (!TileToBlockMap.ContainsKey(tile))
                            TileToBlockMap.Add(tile, block);
                    }
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
                    Biome biome = GetBiomeForY(tilePosition.y);
                    float mineNoise = Mathf.PerlinNoise((x + seed) * mineNoiseScale, (y + seed) * mineNoiseScale);

                    if (tilePosition.y == -(mapHeight / 2) + 1 || tilePosition.y == mapHeight / 2)
                    {
                        mineTilemap.SetTile(tilePosition, bedrock.tiles[0]);
                    }
                    else if (tilePosition.x == mapWidth)
                    {
                        mineTilemap.SetTile(tilePosition, bedrock.tiles[0]);
                    }
                    else
                    {
                        if (mineNoise > 0.8f)
                        {
                            float mineralNoise = Mathf.PerlinNoise((x + seed) * mineralNoiseScale, (y + seed) * mineralNoiseScale);
                            Mineral mineral = ChooseMineralFromNoise(mineralNoise, y);
                            mineTilemap.SetTile(tilePosition, mineral.tiles[0]);
                        }
                        else if (mineNoise > 0.1f && mineNoise < 0.2f)
                        {
                            mineTilemap.SetTile(tilePosition, null);
                        }
                        else
                        {
                            CommonBlock commonBlock = ChooseCommonBlock(x, y, biome);
                            int tileIndex = Mathf.FloorToInt(DeterministicRandom(x + 7000, y + 8000, seed) * commonBlock.tiles.Count);
                            tileIndex = Mathf.Clamp(tileIndex, 0, commonBlock.tiles.Count - 1);
                            mineTilemap.SetTile(tilePosition, commonBlock.tiles[tileIndex]);
                        }
                        backgroundTilemap.SetTile(tilePosition, caveBackgroundTile);
                    }
                }
            }
        }

        private CommonBlock ChooseCommonBlock(int x, int y, Biome biome)
        {
            foreach (var block in biome.commonBlocks)
            {
                float roll = DeterministicRandom(x + 5000, y + 6000, seed);
                if (roll < (1f / biome.commonBlocks.Count))
                {
                    return block;
                }
            }
            return biome.commonBlocks[0];
        }

        private Mineral ChooseMineralFromNoise(float noiseValue, int y)
        {
            List<Mineral> validMinerals = minerals.FindAll(o => y <= o.minDepth && y >= o.maxDepth);

            if (validMinerals.Count == 0) return defaultMineral;

            int index = Mathf.FloorToInt(noiseValue * validMinerals.Count);
            index = Mathf.Clamp(index, 0, validMinerals.Count - 1);
            return validMinerals[index];
        }

        private Biome GetBiomeForY(int y)
        {
            for (int i = biomes.Count - 1; i >= 0; i--)
            {
                if (y >= biomes[i].startY)
                    return biomes[i];
            }
            return null;
        }

        private float DeterministicRandom(int x, int y, int seed)
        {
            int hash = x;
            hash = unchecked(hash * 31 + y);
            hash = unchecked(hash * 31 + seed);
            System.Random rand = new System.Random(hash);
            return (float)rand.NextDouble();
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
