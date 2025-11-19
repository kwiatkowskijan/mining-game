using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame
{
    public class CartMoving : MonoBehaviour
    {
        [Header("References")]
        private Tilemap buildableTilemap;
        [SerializeField] private TileBase railsTile;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rayDistance = 0.5f;

        private Rigidbody2D rb;
        private bool isMoving = false;
        private int moveDirection = 1;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            //znajdywanie tilemapy z torami (buildableTilemap)
            Tilemap[] maps = FindObjectsOfType<Tilemap>();
            foreach (Tilemap map in maps)
            {
                if (map.gameObject.name.Contains("Buildable"))
                {
                    buildableTilemap = map;
                    break;
                }
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                if (IsRailUnder() && IsRailAhead())
                {
                    Vector2 nextPos = rb.position + Vector2.right * moveDirection * moveSpeed * Time.fixedDeltaTime;
                    rb.MovePosition(nextPos);
                }
                else
                    StopMoving();
            }
        }

        public bool CanMoveLeft() => CanMoveInDirection(-1);
        public bool CanMoveRight() => CanMoveInDirection(1);

        public void StartMoving(int direction)
        {
            if (isMoving) return;
            moveDirection = direction;
            isMoving = true;
        }

        void StopMoving()
        {
            isMoving = false;
            rb.linearVelocity = Vector2.zero;
        }

        public bool IsMoving() => isMoving;

        bool IsRailUnder()
        {
            Vector3Int cell = buildableTilemap.WorldToCell(transform.position + Vector3.down * 0.1f);
            TileBase tile = buildableTilemap.GetTile(cell);
            return tile == railsTile;
        }

        bool IsRailAhead()
        {
            Vector3Int cell = buildableTilemap.WorldToCell(transform.position + Vector3.right * moveDirection * rayDistance);
            TileBase tile = buildableTilemap.GetTile(cell);
            return tile == railsTile;
        }

        public bool CanMoveInDirection(int direction)
        {
            Vector3Int cell = buildableTilemap.WorldToCell(transform.position + Vector3.right * direction * rayDistance);
            TileBase tile = buildableTilemap.GetTile(cell);
            return tile == railsTile;
        }
    }
}
