using MiningGame.Player;
using UnityEngine;

namespace MiningGame
{
    public class RubbleDisposal : MonoBehaviour
    {
        [SerializeField] private float disposalRate = 2f;
        [SerializeField] private float rubbleAmount;
        private bool playerInRange = false;
        private Equipment eq;
        private Controller movement;

        private void Update()
        {
            if (playerInRange && eq != null && movement != null)
            {
                if (Input.GetKey(KeyCode.E) && eq.playerRubble > 0f)
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
                eq = other.GetComponent<Equipment>();
                playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                movement = null;
                eq = null;
                playerInRange = false;
            }
        }
        
        private void DisposeRubble()
        {
            eq.playerRubble = Mathf.Max(0, eq.playerRubble - disposalRate * Time.deltaTime);
            eq.playerRubble = Mathf.Round(eq.playerRubble * 100f) / 100f;
            eq.updateRubble();
        }
    }
}
