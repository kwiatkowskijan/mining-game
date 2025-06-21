using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MiningGame.Player
{
    public class BuildMode : MonoBehaviour
    {
        [SerializeField] private GrappleHook grappleHook;
        [SerializeField] private Tilemap ladderTilemap;
        [SerializeField] private Tilemap buildTilemap;
        [SerializeField] private TileBase ladderTile;
        [SerializeField] private TileBase ropeTile;
        [SerializeField] private TileBase torchTile;
        [SerializeField] private TileBase cartTile;

        private bool isInBuildMode = false;

        public Image modeIcon;
        public Sprite normalModeSprite;
        public Sprite buildModeSprite;

        private int buildIndex = 0;

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
                SetBuildMode(isInBuildMode);
            }

            if (isInBuildMode && Input.GetMouseButton(0))
            {
                switch (buildIndex)
                {
                    case 0:
                        Debug.Log("Ladder chosen");
                        PlaceLadder();
                        break;
                    case 1:
                        Debug.Log("Torch chosen");
                        PlaceTorch();
                        break;
                    case 2:
                        Debug.Log("Rope chosen");
                        PlaceRope();
                        break;
                    case 3:
                        Debug.Log("Cart chosen");
                        PlaceCart();
                        break;
                    default:
                        Debug.LogWarning("Brak przypisanej funkcji dla indeksu: " + buildIndex);
                        break;
                }
            }
        }

        public void SetBuildIndex(int index)
        {
            Debug.Log("klik");
            buildIndex = index;
            Debug.Log("Build Index set to: " + buildIndex);
        }

        void PlaceLadder()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = ladderTilemap.WorldToCell(mouseWorldPos);

            ladderTilemap.SetTile(cellPosition, ladderTile);
            ladderTilemap.CompressBounds();

            var tilemapCollider = ladderTilemap.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                tilemapCollider.ProcessTilemapChanges();
            }
            else
            {
                Debug.LogWarning("Brakuje TilemapCollider2D na obiekcie Tilemap!");
            }
        }

        void PlaceTorch()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = buildTilemap.WorldToCell(mouseWorldPos);

            buildTilemap.SetTile(cellPosition, torchTile);
            buildTilemap.CompressBounds();

            //var tilemapCollider = buildTilemap.GetComponent<TilemapCollider2D>();
            //if (tilemapCollider != null)
            //{
            //    tilemapCollider.ProcessTilemapChanges();
            //}
        }

        void PlaceRope()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = buildTilemap.WorldToCell(mouseWorldPos);

            buildTilemap.SetTile(cellPosition, ropeTile);
            buildTilemap.CompressBounds();

        //    var tilemapCollider = buildTilemap.GetComponent<TilemapCollider2D>();
        //    if (tilemapCollider != null)
        //    {
        //        tilemapCollider.ProcessTilemapChanges();
        //    }
        }

        void PlaceCart()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = buildTilemap.WorldToCell(mouseWorldPos);

            buildTilemap.SetTile(cellPosition, cartTile);
            buildTilemap.CompressBounds();

            //var tilemapCollider = buildTilemap.GetComponent<TilemapCollider2D>();
            //if (tilemapCollider != null)
            //{
            //    tilemapCollider.ProcessTilemapChanges();
            //}
        }

        public void SetBuildMode(bool isBuildMode)
        {
            if (isBuildMode)
                modeIcon.sprite = buildModeSprite;
            else
                modeIcon.sprite = normalModeSprite;
        }
    }
}
