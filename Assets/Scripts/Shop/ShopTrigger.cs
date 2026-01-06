using UnityEngine;
using MiningGame.Shop;

namespace MiningGame
{
    public class ShopTrigger : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject tooltipE;

        [Header("Shop UI")]
        [SerializeField] private ShopUIController shopUIController;

        private bool _playerInRange;
        private bool _shopOpen;

        private void Start()
        {
            Debug.Log("ShopTrigger: Start called on " + gameObject.name);

            if (shopUI != null)
                shopUI.SetActive(false);
            else
                Debug.LogWarning("ShopTrigger: shopUI not assigned on " + gameObject.name);

            if (tooltipE != null)
                tooltipE.SetActive(false);
            else
                Debug.LogWarning("ShopTrigger: tooltipE not assigned on " + gameObject.name);

            if (shopUIController == null)
                Debug.LogWarning("ShopTrigger: shopUIController not assigned on " + gameObject.name + ". Assign to display minerals when opening shop.");
            
            var col2d = GetComponent<Collider2D>();
            var col3d = GetComponent<Collider>();
            if (col2d == null && col3d == null)
            {
                Debug.LogWarning("ShopTrigger: No collider found on " + gameObject.name + ". Add a Collider2D (IsTrigger=true) or Collider (IsTrigger=true) to detect player.");
            }
        }

        private void Update()
        {
            if (_playerInRange && Input.GetKeyDown(KeyCode.E) && !_shopOpen)
            {
                OpenShop();
            }

            if (_shopOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseShop();
            }
            
            if (_shopOpen && !_playerInRange)
            {
                CloseShop();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                Debug.Log("ShopTrigger: Player entered 2D trigger");
                if (tooltipE != null && !_shopOpen) tooltipE.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                Debug.Log("ShopTrigger: Player exited 2D trigger");
                if (tooltipE != null) tooltipE.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                Debug.Log("ShopTrigger: Player entered 3D trigger");
                if (tooltipE != null && !_shopOpen) tooltipE.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                Debug.Log("ShopTrigger: Player exited 3D trigger");
                if (tooltipE != null) tooltipE.SetActive(false);
            }
        }

        private void OpenShop()
        {
            _shopOpen = true;

            if (shopUI != null) shopUI.SetActive(true);

            if (shopUIController != null)
            {
                // Nowy system - pokazuje minerały gracza
                shopUIController.ShowPlayerMinerals();
            }

            if (tooltipE != null) tooltipE.SetActive(false);
            
            Debug.Log("ShopTrigger: Shop opened - showing player minerals");
        }

        private void CloseShop()
        {
            _shopOpen = false;

            if (shopUI != null) shopUI.SetActive(false);

            if (shopUIController != null) shopUIController.Clear();

            if (tooltipE != null && _playerInRange) tooltipE.SetActive(true);

            Debug.Log("ShopTrigger: Shop closed");
        }
    }
}
