using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame
{
    public class Digging : MonoBehaviour
    {
        public TileSnapSelector selector;
        public Tilemap tilemap;
        public Equipment eq;
        [SerializeField] private GameObject dirtBall;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            eq = GameObject.FindGameObjectWithTag("Player").GetComponent<Equipment>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // lewy przycisk myszy
            {
                Vector3Int? tileToDig = selector.GetCurrentTile();

                
                if (tileToDig.HasValue)
                {
                    if (eq.playerRubble >= eq.rubbleMax) Debug.Log("You have to much rubble! You can't dig further!");
                    else
                    {
                        tilemap.SetTile(tileToDig.Value, null); // niszczenie tile'a
                        Debug.Log("Wykopano tile na pozycji: " + tileToDig.Value);

                        Vector3 worldPos = tilemap.GetCellCenterWorld(tileToDig.Value);
                        Instantiate(dirtBall, worldPos, Quaternion.identity);
                    }
                }
            }
        }
    }
}
