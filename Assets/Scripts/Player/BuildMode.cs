using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame.Player
{
    public class BuildMode : MonoBehaviour
    {
        [SerializeField] private GrappleHook grappleHook;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase ladderTile;
        private bool isInBuildMode = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                isInBuildMode = !isInBuildMode;
                if (grappleHook != null)
                {
                    grappleHook.enabled = !isInBuildMode;
                }
                Debug.Log("Build mode: " + (isInBuildMode ? "ON" : "OFF"));
            }

            if (isInBuildMode && Input.GetMouseButton(0))
            {
                PlaceLadder();
            }
        }

        void PlaceLadder()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            tilemap.SetTile(cellPosition, ladderTile);
            tilemap.CompressBounds();

            var tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                tilemapCollider.ProcessTilemapChanges();
            }
            else
            {
                Debug.LogWarning("Brakuje TilemapCollider2D na obiekcie Tilemap!");
            }
        }

    }
}
