using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace MiningGame.Player
{
    public class BuildMode : MonoBehaviour
    {
        [Header("Build System")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float maxBuildDistance = 3f;

        [Header("Tilemaps")]
        [SerializeField] private Tilemap ladderTilemap;
        [SerializeField] private Tilemap buildTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase ladderTile;
        [SerializeField] private TileBase ropeTile;
        [SerializeField] private TileBase torchTile;
        [SerializeField] private TileBase cartTile;
        [SerializeField] private TileBase railsTile;

        [Header("Mode Switching")]
        [SerializeField] private GrappleHook grappleHook;
        [SerializeField] private Image modeIcon;
        [SerializeField] private Sprite normalModeSprite;
        [SerializeField] private Sprite buildModeSprite;
        [SerializeField] private Tilemap previewTilemap;
        [SerializeField] private TileBase previewTileBase;


        private bool isInBuildMode = false;
        private BuildType currentBuildType = BuildType.Ladder;
        private Dictionary<BuildType, BuildData> buildOptions;

        private enum BuildType
        {
            Ladder = 0,
            Torch = 1,
            Rope = 2,
            Cart = 3,
            Rails = 4
        }

        private struct BuildData
        {
            public Tilemap tilemap;
            public TileBase tile;
            public string name;
        }

        void Start()
        {
            buildOptions = new Dictionary<BuildType, BuildData>
            {
                { BuildType.Ladder, new BuildData { tilemap = ladderTilemap, tile = ladderTile, name = "Ladder" } },
                { BuildType.Torch, new BuildData { tilemap = buildTilemap, tile = torchTile, name = "Torch" } },
                { BuildType.Rope, new BuildData { tilemap = buildTilemap, tile = ropeTile, name = "Rope" } },
                { BuildType.Cart, new BuildData { tilemap = buildTilemap, tile = cartTile, name = "Cart" } },
                { BuildType.Rails, new BuildData { tilemap = buildTilemap, tile = railsTile, name = "Rails" } }
            };
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                HandleBuildModeToggle();
            }

            if (isInBuildMode && Input.GetMouseButton(0))
            {
                TryPlaceTile();
            }

            if (isInBuildMode)
            {
                UpdatePreviewTile();
            }
            else
            {
                previewTilemap.ClearAllTiles();
            }

        }

        public void HandleBuildModeToggle()
        {
            isInBuildMode = !isInBuildMode;

            if (grappleHook != null)
                grappleHook.enabled = !isInBuildMode;

            SetBuildModeIcon(isInBuildMode);

            Debug.Log("Build mode: " + (isInBuildMode ? "ON" : "OFF"));
        }

        private void TryPlaceTile()
        {
            if (!buildOptions.TryGetValue(currentBuildType, out var buildData))
            {
                Debug.LogWarning("Nieznany typ budowli: " + currentBuildType);
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = buildData.tilemap.WorldToCell(mouseWorldPos);
            Vector3 cellWorld = buildData.tilemap.CellToWorld(cellPos);

            if (Vector2.Distance(playerTransform.position, cellWorld) > maxBuildDistance)
            {
                Debug.Log("Za daleko! Maksymalny zasi�g budowania to " + maxBuildDistance);
                return;
            }

            buildData.tilemap.SetTile(cellPos, buildData.tile);
            buildData.tilemap.CompressBounds();

            TilemapCollider2D collider = buildData.tilemap.GetComponent<TilemapCollider2D>();
            if (collider != null)
                collider.ProcessTilemapChanges();

            Debug.Log($"{buildData.name} placed at {cellPos}");
            previewTilemap.SetTile(cellPos, null);
        }

        public void SetBuildIndex(int index)
        {
            if (System.Enum.IsDefined(typeof(BuildType), index))
            {
                currentBuildType = (BuildType)index;
                Debug.Log("Build type set to: " + currentBuildType);
            }
            else
            {
                Debug.LogWarning("Niepoprawny indeks budowli: " + index);
            }
        }

        private void SetBuildModeIcon(bool buildMode)
        {
            if (modeIcon != null)
                modeIcon.sprite = buildMode ? buildModeSprite : normalModeSprite;
        }

        private Vector3Int previousPreviewPos;
        private void UpdatePreviewTile()
        {
            if (!buildOptions.TryGetValue(currentBuildType, out var buildData))
                return;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = buildData.tilemap.WorldToCell(mouseWorldPos);

            if (cellPos != previousPreviewPos)
            {
                previewTilemap.SetTile(previousPreviewPos, null);
                previousPreviewPos = cellPos;
            }

            Vector3 cellWorld = buildData.tilemap.CellToWorld(cellPos);

            if (Vector2.Distance(playerTransform.position, cellWorld) > maxBuildDistance)
            {
                previewTilemap.SetTile(cellPos, null);
                return;
            }

            previewTilemap.SetTile(cellPos, previewTileBase ?? buildData.tile);
        }

    }
}
