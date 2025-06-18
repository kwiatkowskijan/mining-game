using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame
{
    public class MineralPickup : MonoBehaviour
    {
        public Transform player;
        [SerializeField] private float attractionRange = 2f;
        [SerializeField] private float attractionSpeed = 5f;
        private Rigidbody2D rb;
        //private float randomMass;
        //pytanie: czy minera³y powinny mieæ losow¹ wagê czy po prostu wartoœæ 1?
        public Equipment eq;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            //randomMass = Random.Range(0.5f, 1.5f);
            //randomMass = Mathf.Round(randomMass * 100f) / 100f;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < attractionRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = direction * attractionSpeed;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                eq = other.GetComponent<Equipment>();
                //eq.playerRubble += randomMass;
                eq.updateMineral();
                Destroy(gameObject);
            }
        }
    }
}
