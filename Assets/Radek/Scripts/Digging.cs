using MiningGame.WorldGeneration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

namespace MiningGame.Tools
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
                    
                        TileBase highlightedTile = selector.GetCurrentTileType();
                        if (highlightedTile == null) return;

                        if(WorldGeneration.MineGenerator.TileToBlockMap.TryGetValue(highlightedTile, out Block block))
                        {
                            if (block.isDescrutable)
                            {
                                Debug.Log("Wykopywany blok to: " + block.name);

                                GameObject toSpawn = null;
                                float miningMultiplier = 1f;

                                if (block is CommonBlock common)
                                {
                                    toSpawn = common.toSpawn;
                                    miningMultiplier = common.miningMultiplier;
                                }

                                if (block is Mineral mineral)
                                {
                                    toSpawn = mineral.toSpawn;
                                    miningMultiplier = mineral.miningMultiplier;
                                }

                                float blockMiningTime = miningTime * miningMultiplier;
                            
                                holdTimer += Time.deltaTime;

                                if (holdTimer >= blockMiningTime)
                                {
                                    tilemap.SetTile(tileToDig.Value, null); // niszczenie tile'a
                                    Vector3 worldPos = tilemap.GetCellCenterWorld(tileToDig.Value);
                                    if (toSpawn != null)
                                        Instantiate(toSpawn, worldPos, Quaternion.identity);

                                    holdTimer = 0f;
                                    lastTargetedTile = null;
                                }
                            }
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
