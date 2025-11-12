using UnityEngine;

namespace MiningGame
{
    public class ShopInteractions : MonoBehaviour
    {
        private bool isPlayerNearby = false;
        public GameObject shopUI;

        void Update()
        {
            if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            {
                shopUI.SetActive(!shopUI.activeSelf);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
                shopUI.SetActive(false);
            }
        }
    }
}
