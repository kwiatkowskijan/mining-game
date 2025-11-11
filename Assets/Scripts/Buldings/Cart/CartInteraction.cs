using MiningGame.Player;
using UnityEngine;

namespace MiningGame
{
    public class CartInteraction : MonoBehaviour
    {
        private bool inTrigger = false;
        private bool inInteraction = false;

        [SerializeField] private CartMoving cartMoving;
        private Controller playerMovement;
        private 

        void Update()
        {
            if (inTrigger && Input.GetKeyDown(KeyCode.E))
            {
                if (!inInteraction)
                {
                    if (cartMoving != null && cartMoving.IsMoving()) 
                    {
                        return;
                    }

                    EnterInteraction();
                }
                else
                {
                    ExitInteraction();
                }
            }

            if (inInteraction)
            {
                if (cartMoving != null && cartMoving.IsMoving())
                {
                    ExitInteraction();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    TryMoving(-1);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    TryMoving(1);
                }
            }
        }

        private void EnterInteraction()
        {
            inInteraction = true;
            if (playerMovement != null)
                playerMovement.disableMovement();

            bool canLeft = cartMoving != null && cartMoving.CanMoveInDirection(-1);
            bool canRight = cartMoving != null && cartMoving.CanMoveInDirection(1);
            Debug.Log($"Entered cart interaction. Left {(canLeft ? "OK" : "NO")}");
        }

        private void ExitInteraction()
        {
            inInteraction = false;
            if (playerMovement != null)
                playerMovement.enableMovement();
            Debug.Log("Exited cart interaction");
        }

        private void TryMoving(int direction)
        {
            if (cartMoving == null) return;

            if (!cartMoving.CanMoveInDirection(direction))
            {
                Debug.Log($"Can't push {(direction == -1 ? "left" : "right")}: no rails ");
                return;
            }

            cartMoving.StartMoving(direction);
            ExitInteraction();
            Debug.Log($"Pushed cart {(direction == -1 ? "left": "right")}.");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            inTrigger = true;
            playerMovement = other.GetComponent<Controller>();
            Debug.Log("Player entered cart trigger (press E to interact) .");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            inTrigger = false;
            if (inInteraction)
            {
                ExitInteraction();
            }

            playerMovement = null;
            Debug.Log("Player exited cart trigger.");
        }
    }
}
