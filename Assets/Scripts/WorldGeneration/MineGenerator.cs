using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.WorldGeneration
{
    public class MineGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap caveTilemap;
        [SerializeField] private Tile dirtTile;
        [SerializeField] private int mapHeight;
        [SerializeField] private int mapWidth;
        [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);


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
                    int worldY = startPosition.y - y;
                    Vector3Int tilePosition = new Vector3Int(startPosition.x + x, worldY, 0);
                    caveTilemap.SetTile(tilePosition, dirtTile);
                }
            }
        }
    }

    public enum Tiles
    {
        dirt,
        mineral
    }
}
