using UnityEngine;

namespace MiningGame.Environment
{
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player.Stats currentHealth))
            {
                Debug.Log("Gracz otrzymuje obra¿enia");
                currentHealth.TakeDamage(damage);
            }
        }
    }
}
