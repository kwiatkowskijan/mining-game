using System;
using UnityEngine;

namespace MiningGame
{
    public class ShopTrigger : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject tooltipE;
        
        private bool playerInRange = false;
        private bool shopOpen = false;
        
        private void Start()
        {
            if (shopUI != null)
                shopUI.SetActive(false);
                
            if (tooltipE != null)
                tooltipE.SetActive(false);
        }
        
        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E) && !shopOpen)
            {
                OpenShop();
                Debug.Log("Shop opened");
            }
            
            if (shopOpen && !playerInRange || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseShop();
                Debug.Log("Shop closed");
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                if (tooltipE != null && !shopOpen)
                    tooltipE.SetActive(true);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                if (tooltipE != null)
                    tooltipE.SetActive(false);
            }
        }
        
        private void OpenShop()
        {
            shopOpen = true;
            if (shopUI != null)
                shopUI.SetActive(true);
                
            if (tooltipE != null)
                tooltipE.SetActive(false);
        }
        
        private void CloseShop()
        {
            shopOpen = false;
            if (shopUI != null)
                shopUI.SetActive(false);
                
            if (tooltipE != null && playerInRange)
                tooltipE.SetActive(true);
        }
    }
}


