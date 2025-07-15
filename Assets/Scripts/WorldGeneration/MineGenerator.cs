using System.Collections.Generic;
using MiningGame.MapGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.WorldGeneration
{
    public class MineGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap caveTilemap;
        [SerializeField] private List<OreTile> oreTiles;
        [SerializeField] private List<DirtTile> dirtTiles;
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);
        [SerializeField] private float perlinNoiseScale;

        private void Awake()
        {
            GenerateCave(mapWidth, mapHeight);
        }

        private void GenerateCave(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(startPosition.x + x, startPosition.y - y, 0);
                    float noise = Mathf.PerlinNoise(x * perlinNoiseScale, y * perlinNoiseScale);
                    
                    if (noise > 0.7f)
                        caveTilemap.SetTile(tilePosition, oreTiles.Find(t => t.isDescrutable).tile);
                    else
                        caveTilemap.SetTile(tilePosition, dirtTiles.Find(t => t.isDescrutable).tile);
                }
            }
        }
    }
}
