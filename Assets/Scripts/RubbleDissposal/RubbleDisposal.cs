using MiningGame.Player;
using UnityEngine;

namespace MiningGame
{
    public class RubbleDisposal : MonoBehaviour
    {
        [SerializeField] private float disposalRate = 2f;
        private bool playerInRange = false;
        private Stats stats;
        private Controller movement;

        private void Update()
        {
            if (playerInRange && stats != null && movement != null)
            {
                if (Input.GetKey(KeyCode.E) && stats.CurrentRubble > 0f)
                {
                    DisposeRubble();
                    movement.disableMovement();
                }
                else
                {
                    movement.enableMovement();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                movement = other.GetComponent<Controller>();
                movement.saveDefaults();
                stats = other.GetComponent<Stats>();
                playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                movement = null;
                stats = null;
                playerInRange = false;
            }
        }
        
        private void DisposeRubble()
        {
            stats.RemoveRubble(disposalRate);
            stats.UpdateRubbleUI();
        }
    }
}
