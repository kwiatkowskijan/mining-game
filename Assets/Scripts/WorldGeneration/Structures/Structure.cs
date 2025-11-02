using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Structure", menuName = "Scriptable Objects/Structure")]
public class Structure : ScriptableObject
{
    public string structureName;
    public TileBase[] tiles; 
    public int width;
    public int height;

    public TileBase GetTile(int x, int y)
    {
        return tiles[y * width + x];
    }

    [Header("Spawn Chance Settings")]
    [Range(0f, 1f)] public float spawnChance = 0.02f;
}
