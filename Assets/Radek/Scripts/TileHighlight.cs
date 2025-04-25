using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSnapSelector : MonoBehaviour
{
    public Tilemap tilemap;
    public Transform player;
    public float playerReach = 4f;
    public float cursorMaxDistance = 3f;

    private Vector3Int? currentTile = null;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3Int? selectedTile = null;
        float bestCursorDistance = Mathf.Infinity;

        Vector3Int cursorCell = tilemap.WorldToCell(mouseWorldPos);
        for (int x = -10; x <= 10; x++)
        {
            for (int y = -10; y <= 10; y++)
            {
                Vector3Int pos = cursorCell + new Vector3Int(x, y, 0);
                if (!tilemap.HasTile(pos)) continue;

                Vector3 tileWorldPos = tilemap.GetCellCenterWorld(pos);

                float cursorDistance = Vector3.Distance(mouseWorldPos, tileWorldPos);
                float playerDistance = Vector3.Distance(player.position, tileWorldPos);

                if (cursorDistance > cursorMaxDistance) continue;
                if (playerDistance > playerReach) continue;
                if (IsObstructed(player.position, tileWorldPos, pos)) continue;

                if (cursorDistance < bestCursorDistance)
                {
                    bestCursorDistance = cursorDistance;
                    selectedTile = pos;
                }
            }
        }

        if (selectedTile.HasValue)
        {
            currentTile = selectedTile;
            transform.position = tilemap.GetCellCenterWorld(selectedTile.Value);
        }
        else
        {
            currentTile = null;
            transform.position = new Vector3(9999, 9999, 0); // poza ekranem
        }
    }

    bool IsObstructed(Vector3 fromWorld, Vector3 toWorld, Vector3Int targetTile)
    {
        Vector3 direction = (toWorld - fromWorld).normalized;
        float distance = Vector3.Distance(fromWorld, toWorld);
        int steps = Mathf.CeilToInt(distance * 10); // dok³adnoœæ raycasta

        for (int i = 1; i < steps; i++)
        {
            Vector3 sample = Vector3.Lerp(fromWorld, toWorld, i / (float)steps);
            Vector3Int sampleCell = tilemap.WorldToCell(sample);

            if (sampleCell == targetTile)
                continue;

            if (tilemap.HasTile(sampleCell))
                return true; // coœ zas³ania
        }

        return false;
    }
}
