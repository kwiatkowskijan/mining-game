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
        [SerializeField] private Tilemap movingObjectsTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase ladderTile;
        [SerializeField] private TileBase ropeTile;
        [SerializeField] private TileBase torchTile;
        [SerializeField] private TileBase cartTile;
        [SerializeField] private TileBase railsTile;

        [Header("Prefabs")]
        [SerializeField] private GameObject cartPrefab;

        [Header("Mode Switching")]
        [SerializeField] private GrappleHook grappleHook;
        [SerializeField] private Image modeIcon;
        [SerializeField] private Sprite normalModeSprite;
        [SerializeField] private Sprite buildModeSprite;
        [SerializeField] private Tilemap previewTilemap;
        [SerializeField] private TileBase previewTileBase;

        [SerializeField] private GameObject buildUIRoot;

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
            public GameObject prefab;
            public int type;
            public string name;
        }

        private int GetCost(BuildType t)
        {
            switch (t)
            {
                case BuildType.Torch: return 2;
                case BuildType.Ladder: return 5;
                case BuildType.Rope: return 3;
                case BuildType.Cart: return 15;
                case BuildType.Rails: return 10;
                default: return 0;
            }
        }

        void Start()
        {
            buildOptions = new Dictionary<BuildType, BuildData>
            //type: 1-Tilemap 2-gameObject 3-tilemapa+gameObject
            {
                { BuildType.Ladder, new BuildData { tilemap = ladderTilemap, tile = ladderTile, type = 1, name = "Ladder" } },
                { BuildType.Torch, new BuildData { tilemap = buildTilemap, tile = torchTile, type = 1, name = "Torch" } },
                { BuildType.Rope, new BuildData { tilemap = buildTilemap, tile = ropeTile, type = 1, name = "Rope" } },
                { BuildType.Cart, new BuildData { tilemap = movingObjectsTilemap, prefab = cartPrefab, type = 2, name = "Cart" } },
                { BuildType.Rails, new BuildData { tilemap = buildTilemap, tile = railsTile, type = 1, name = "Rails" } }
            };

            if (buildUIRoot) buildUIRoot.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                HandleBuildModeToggle();
            }

            if (isInBuildMode && Input.GetMouseButtonDown(0))
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
        public int GetCostByIndex(int index)
        {
            if (System.Enum.IsDefined(typeof(BuildType), index))
                return GetCost((BuildType)index);
            return 0;
        }

        public void HandleBuildModeToggle()
        {
            isInBuildMode = !isInBuildMode;

            if (grappleHook != null)
                grappleHook.enabled = !isInBuildMode;

            SetBuildModeIcon(isInBuildMode);

            if (buildUIRoot) buildUIRoot.SetActive(isInBuildMode);

            Debug.Log("Build mode: " + (isInBuildMode ? "ON" : "OFF"));
        }

        private void TryPlaceTile()
        {
            var stats = playerTransform.GetComponent<Stats>();
            if (!buildOptions.TryGetValue(currentBuildType, out var buildData))
            {
                Debug.LogWarning("Unknown build type: " + currentBuildType);
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = buildData.tilemap.WorldToCell(mouseWorldPos);
            Vector3 cellWorld = buildData.tilemap.CellToWorld(cellPos);

            if (Vector2.Distance(playerTransform.position, cellWorld) > maxBuildDistance)
            {
                Debug.Log("Too far. Max distance is: " + maxBuildDistance);
                return;
            }

            if (buildData.tilemap.HasTile(cellPos))
            {
                Debug.Log("This tile is occupied: " + buildData.name);
                return;
            }   

            int cost = GetCost(currentBuildType);
            if (stats.CurrentMoney < cost) { Debug.Log("Not enough money"); return; }

            //jaka struktura chce zostac postawiona?
            bool canBePlaced = whichStructure(currentBuildType, cellPos);
            if (!canBePlaced)
            {
                Debug.Log("Can't be placed on this tile");
                return;
            }

            //w jaki sposób powinna zostaæ stworzona struktura?
            switch(buildData.type){
                case 1:
                    buildData.tilemap.SetTile(cellPos, buildData.tile);
                    buildData.tilemap.CompressBounds();

                    TilemapCollider2D collider = buildData.tilemap.GetComponent<TilemapCollider2D>();
                    if (collider != null)
                        collider.ProcessTilemapChanges();
                    break;

                case 2:
                    Vector3 worldPos = buildData.tilemap.CellToWorld(cellPos) + buildData.tilemap.tileAnchor;

                    GameObject obj = GameObject.Instantiate(buildData.prefab, worldPos, Quaternion.identity);
                    obj.name = buildData.name;
                    break;
            }

            Debug.Log($"{buildData.name} placed at {cellPos}");
            previewTilemap.SetTile(cellPos, null);

            stats.RemoveMoney(cost);

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
                Debug.LogWarning("Wrong index: " + index);
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

        private bool whichStructure(BuildType currentStructure, Vector3Int cellPos)
        {
            switch (currentStructure)
            {
                case BuildType.Cart:
                    if (!(buildTilemap.GetTile(cellPos) == railsTile)) return false;
                    break;
            }

            return true;
        }

    }
}
