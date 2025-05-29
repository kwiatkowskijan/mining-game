using UnityEngine;

namespace MiningGame
{
    public class BaseHubInteractions : MonoBehaviour
    {
        private bool isPlayerNearby = false;
        public GameObject upgradeUI;

        void Update()
        {
            if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            {
                upgradeUI.SetActive(!upgradeUI.activeSelf);
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
                upgradeUI.SetActive(false);
            }
        }
    }
}
