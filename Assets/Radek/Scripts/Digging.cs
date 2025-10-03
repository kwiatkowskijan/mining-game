using MiningGame.MapGeneration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

namespace MiningGame
{
    public class Digging : MonoBehaviour
    {
        public TileSnapSelector selector;
        public Tilemap tilemap;
        public Equipment eq;

        public float miningTime = 2f;
        [SerializeField] private float holdTimer = 0f;
        private Vector3Int? lastTargetedTile = null;
        [SerializeField] private GameObject rubbleMessage;

        [Header("Dirt Block")]

        [SerializeField] private GameObject dirtPickup;

        [Header("Mineral Block")]
        [SerializeField] private TileBase[] mineralTiles;
        [SerializeField] private GameObject mineralPickup;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            eq = GameObject.FindGameObjectWithTag("Player").GetComponent<Equipment>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3Int? tileToDig = selector.GetCurrentTile();

            if (!tileToDig.HasValue)
            {
                holdTimer = 0f;
                lastTargetedTile = null;
                return;
            }

            if (!lastTargetedTile.HasValue || tileToDig.Value != lastTargetedTile.Value)
            {
                holdTimer = 0f;
                lastTargetedTile = tileToDig;
            }

            if (eq.playerRubble < eq.rubbleMax)
            {
                rubbleMessage.SetActive(false);
                if (Input.GetMouseButton(0)) // lewy przycisk myszy
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer >= miningTime)
                    {
                        TileBase highlightedTile = selector.GetCurrentTileType();
                        if (highlightedTile == null) return;

                        if(MiningGame.WorldGeneration.MineGenerator.TileToBlockMap.TryGetValue(highlightedTile, out Block block))
                        {
                            Debug.Log("Wykopywany blok to: " + block.name);

                            GameObject toSpawn = null;

                            if (block is CommonBlock)
                            {
                                toSpawn = dirtPickup;
                            }

                            if (block is Ore)
                            {
                                toSpawn = mineralPickup;
                            }

                            tilemap.SetTile(tileToDig.Value, null); // niszczenie tile'a
                            Vector3 worldPos = tilemap.GetCellCenterWorld(tileToDig.Value);
                            if (toSpawn != null)
                            Instantiate(toSpawn, worldPos, Quaternion.identity);
                        }
                        
                        holdTimer = 0f;
                        lastTargetedTile = null;
                    }
                }
                else holdTimer = 0f;
            }
            else
            {
                rubbleMessage.SetActive(true);
            }

        }
    }
}
