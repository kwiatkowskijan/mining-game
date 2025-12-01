using MiningGame.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace MiningGame
{
    public class DirtBall : MonoBehaviour
    {
        private Transform player;
        [Header("Attraction to Player")]
        [SerializeField] private float attractionRange = 3f;
        [SerializeField] private float attractionSpeed = 5f;
        private Rigidbody2D rb;
        private float randomMass;
        [Header("Rubble Amount")]
        [SerializeField] private float randomMassLowerRange;
        [SerializeField] private float randomMassHigherRange;
        private Stats stats;

        void Start()
        {
            rb= GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            randomMass = Random.Range(randomMassLowerRange,randomMassHigherRange);
            randomMass = Mathf.Round(randomMass * 100f) / 100f;
        }

        void FixedUpdate()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if(distance < attractionRange) 
            {
                Vector2 direction =(player.position - transform.position).normalized;
                rb.linearVelocity = direction * attractionSpeed;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                stats = other.GetComponent<Stats>();
                stats.AddRubble(randomMass);
                Destroy(gameObject);
            }
        }
    }
}
