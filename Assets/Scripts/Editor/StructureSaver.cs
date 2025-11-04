using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureSaver
{
    [MenuItem("GameObject/Save Tilemap As Structure", false, 10)]
    static void SaveStructure()
    {
        Tilemap tilemap = Selection.activeGameObject?.GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("Choose a Tilemap GameObject to save as Structure.");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        Structure structure = ScriptableObject.CreateInstance<Structure>();
        structure.structureName = tilemap.name;
        structure.width = bounds.size.x;
        structure.height = bounds.size.y;
        structure.tiles = allTiles;

        string path = EditorUtility.SaveFilePanelInProject("Zapisz strukturę", tilemap.name + "_Structure", "asset", "Wybierz miejsce zapisu struktury");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(structure, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"Structure saved: {path}");
        }
    }
}
