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
        private IMineralsService _mineralsService;
        private IAudioService _audioService;

        [HideInInspector] public Transform player;
        [SerializeField] private Mineral mineral;
        [SerializeField] private float attractionRange = 2f;
        [SerializeField] private float attractionSpeed = 5f;
        [SerializeField] private AudioClip pickupAudio;
        private Rigidbody2D rb;
        //private float randomMass;
        //pytanie: czy minera�y powinny mie� losow� wag� czy po prostu warto�� 1?
        [HideInInspector] public Equipment eq;

        private void Awake()
        {
            _mineralsService = ServiceLocator.Get<IMineralsService>();
            _audioService = ServiceLocator.Get<IAudioService>();
        }
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
            //randomMass = Random.Range(0.5f, 1.5f);
            //randomMass = Mathf.Round(randomMass * 100f) / 100f;
        }

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
                _mineralsService.DiscoverMineral(mineral);
                _audioService.PlaySfx(pickupAudio);
                Destroy(gameObject);
            }
        }
    }
}
