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
        [Header("Blocks")]
        [SerializeField] private List<Ore> ores;
        [SerializeField] private List<CommonBlock> commonBlocks;
        [SerializeField] private Ore deafultOre;
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
        [Range(0f, 1f)][SerializeField] private float oreNoiseScale = 0.05f;
        [Header("Structures")]
        [SerializeField] private List<Structure> structures;

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

            Debug.Log("Perlin noise test: " + Mathf.PerlinNoise(0f * mineNoiseScale, -32f * mineNoiseScale));
        }

        private void InitValues()
        {
            _startPosition = new Vector3Int(startX, 0, 0);
            if (seed == 0)
                seed = Random.Range(-1000000, 1000000);
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void MapTileToBlock()
        {
            foreach (var ore in ores)
            {
                if (ore.tiles != null)
                {
                    foreach (var tile in ore.tiles)
                    {
                        if (!TileToBlockMap.ContainsKey(tile))
                            TileToBlockMap.Add(tile, ore);
                    }
                }
            }
            foreach (var dirt in commonBlocks)
            {
                if (dirt.tiles != null)
                {
                    foreach (var tile in dirt.tiles)
                    {
                        if (!TileToBlockMap.ContainsKey(tile))
                            TileToBlockMap.Add(tile, dirt);
                    }
                }
            }
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
                Debug.Log("Player chunk: " + playerChunk);
                loadChunksNearPlayer(playerChunk);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private Vector2Int GetPlayerChunk()
        {
            Debug.Log("Player position: " + _player.position);
            Debug.Log("Start position: " + _startPosition);
            int chunkX = Mathf.FloorToInt((_player.position.x - _startPosition.x) / chunkSize);
            int chunkY = Mathf.FloorToInt((_player.position.y - _startPosition.y) / chunkSize);
            return new Vector2Int(chunkX, chunkY);
        }

        private void loadChunksNearPlayer(Vector2Int playerChunk)
        {
            for (int x = Mathf.Max(0, playerChunk.x - loadDistance); x <= playerChunk.x + loadDistance; x++)
            {
                Debug.Log("Loading chunks at X: " + x);
                for (int y = playerChunk.y - loadDistance; y <= playerChunk.y + loadDistance; y++)
                {
                    Debug.Log("Loading chunks at Y: " + y);
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
            int startX = chunkX * chunkSize; // 0
            int startY = chunkY * chunkSize;  // -32

            for (int x = startX; x < startX + chunkSize; x++)
            {
                for (int y = startY; y < startY + chunkSize; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(_startPosition.x + x, _startPosition.y + y, 0);
                    float mineNoise = Mathf.PerlinNoise((x + seed) * mineNoiseScale, (y + seed) * mineNoiseScale);

                    if (tilePosition.y == -(mapHeight / 2) + 1 || tilePosition.y == mapHeight / 2)
                    {
                        mineTilemap.SetTile(tilePosition, bedrock.tiles[0]);
                    }
                    else
                    {
                        if (mineNoise > 0.8f)
                        {
                            float oreNoise = Mathf.PerlinNoise((x + seed) * oreNoiseScale, (y + seed) * oreNoiseScale);
                            Ore ore = ChooseOreFromNoise(oreNoise, y);
                            mineTilemap.SetTile(tilePosition, ore.tiles[0]);
                        }
                        else if (mineNoise > 0.1f && mineNoise < 0.2f)
                        {
                            mineTilemap.SetTile(tilePosition, null);
                        }
                        else
                        {
                            CommonBlock commonBlock = ChooseCommonBlock();
                            mineTilemap.SetTile(tilePosition, commonBlock.tiles[Random.Range(0, commonBlock.tiles.Count)]);
                        }
                        backgroundTilemap.SetTile(tilePosition, caveBackgroundTile);
                    }
                }
            }
            TryPlaceStructure(chunkX, chunkY);
        }

        private CommonBlock ChooseCommonBlock()
        { 
            foreach (var block in commonBlocks)
            {
                float roll = Random.Range(0f, 1f);
                if (roll < (1f / commonBlocks.Count))
                {
                    return block;
                }
            }

            return commonBlocks[0];
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
            Debug.Log("Trying to place structures in chunk: " + chunkX + ", " + chunkY);
            foreach (var structure in structures)
            {
                float roll = DeterministicRandom(chunkX, chunkY, seed);

                if (roll < structure.spawnChance)
                {
                    int startX = chunkX * chunkSize + Mathf.FloorToInt(DeterministicRandom(chunkX + 1000, chunkY + 2000, seed) * (chunkSize - structure.width));
                    int startY = chunkY * chunkSize + Mathf.FloorToInt(DeterministicRandom(chunkX + 3000, chunkY + 4000, seed) * (chunkSize - structure.height));

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
                        mineTilemap.SetTile(new Vector3Int(position.x + x, position.y + y, 0), tile);
                    }
                }
            }
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
