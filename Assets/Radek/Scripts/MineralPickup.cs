using MiningGame.Core.Interfaces;
using MiningGame.Core;
using MiningGame.Managers;
using MiningGame.WorldGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiningGame
{
    public class MineralPickup : MonoBehaviour
    {
        private IMineralsService mineralsService;

        [HideInInspector] public Transform player;
        [SerializeField] private Mineral mineral;
        [SerializeField] private float attractionRange = 2f;
        [SerializeField] private float attractionSpeed = 5f;
        private Rigidbody2D rb;
        //private float randomMass;
        //pytanie: czy minera�y powinny mie� losow� wag� czy po prostu warto�� 1?
        [HideInInspector] public Equipment eq;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private void Awake()
        {
            mineralsService = ServiceLocator.Get<IMineralsService>();
        }
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
                mineralsService.DiscoverMineral(mineral);
                Destroy(gameObject);
            }
        }
    }
}
