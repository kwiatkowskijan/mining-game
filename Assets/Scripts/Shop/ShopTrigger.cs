using UnityEngine;
using MiningGame.Shop;
using MiningGame.Player;

namespace MiningGame
{
    public class ShopTrigger : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopUI;
        [SerializeField] private GameObject tooltipE;

        [Header("Managers")]
        [SerializeField] private ShopManager shopManager;

        [Header("Shop UI")]
        [SerializeField] private ShopUIController shopUIController;
        
        [Header("Player References")]
        [SerializeField] private Stats playerStats;
        [SerializeField] private Equipment playerEquipment;

        private bool _playerInRange;
        private bool _shopOpen;
        private ShopItemData[] _currentShopItems;

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

            if (shopManager == null)
                Debug.LogWarning("ShopTrigger: shopManager not assigned on " + gameObject.name + ". Assign the ShopManager so items can be generated on open.");

            if (shopUIController == null)
                Debug.LogWarning("ShopTrigger: shopUIcontroller not assigned on " + gameObject.name + ". Assign to display items when opening shop.");
            else
                shopUIController.OnBuyButtonClicked += HandlePurchase;
            
            if (playerStats == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerStats = player.GetComponent<Stats>();
                
                if (playerStats == null)
                    Debug.LogWarning("ShopTrigger: Could not find Player Stats component!");
            }
            
            if (playerEquipment == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerEquipment = player.GetComponent<Equipment>();
                
                if (playerEquipment == null)
                    Debug.LogWarning("ShopTrigger: Could not find Player Equipment component!");
            }
            
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
            _currentShopItems = new ShopItemData[0];
            
            if (shopManager != null)
            {
                shopManager.GenerateShopItems();
                ShopItemType[] itemTypes = shopManager.GetGeneratedItems();
                
                
                _currentShopItems = new ShopItemData[itemTypes.Length];
                for (int i = 0; i < itemTypes.Length; i++)
                {
                    _currentShopItems[i] = shopManager.GetItemData(itemTypes[i]);
                }
                
                Debug.Log("ShopTrigger: Generated items: " + string.Join(", ", itemTypes));
            }

            _shopOpen = true;

            if (shopUI != null) shopUI.SetActive(true);

            if (shopUIController != null)
            {
                shopUIController.ShowItems(_currentShopItems);
            }

            if (tooltipE != null) tooltipE.SetActive(false);
        }

        private void CloseShop()
        {
            _shopOpen = false;

            if (shopUI != null) shopUI.SetActive(false);

            if (shopUIController != null) shopUIController.Clear();

            if (tooltipE != null && _playerInRange) tooltipE.SetActive(true);

            Debug.Log("ShopTrigger: Shop closed");
        }
        
        private void HandlePurchase(int itemIndex)
        {
            if (_currentShopItems == null || itemIndex >= _currentShopItems.Length)
            {
                Debug.LogError("ShopTrigger: Invalid item index!");
                return;
            }
            
            ShopItemData item = _currentShopItems[itemIndex];
            
            if (playerStats == null)
            {
                Debug.LogError("ShopTrigger: Player Stats not found! Cannot complete purchase.");
                return;
            }
            
            if (playerEquipment == null)
            {
                Debug.LogError("ShopTrigger: Player Equipment not found! Cannot complete purchase.");
                return;
            }
            
            if (!playerEquipment.HasEmptySlot())
            {
                Debug.Log("ShopTrigger: Equipment is full! Cannot purchase item.");
                return;
            }
            
            if (playerStats.CurrentMoney < item.price)
            {
                Debug.Log($"ShopTrigger: Not enough money! Need ${item.price}, have ${playerStats.CurrentMoney}");
                return;
            }
            
            playerStats.RemoveMoney(item.price);
            
            Debug.Log($"ShopTrigger: About to add item. Icon = {(item.icon != null ? item.icon.name : "NULL")}");
            
            bool added = playerEquipment.AddItem(item.itemType, item.icon);
            
            if (added)
            {
                Debug.Log($"ShopTrigger: Successfully purchased {item.itemType} for ${item.price}. Remaining money: ${playerStats.CurrentMoney}");
                
                Debug.Log("=== EQUIPMENT AFTER PURCHASE ===");
                var toolsImages = playerEquipment.GetType()
                    .GetField("toolsImages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(playerEquipment) as UnityEngine.GameObject[];
                
                if (toolsImages != null)
                {
                    for (int i = 0; i < toolsImages.Length; i++)
                    {
                        if (toolsImages[i] == null)
                        {
                            Debug.Log($"  Slot {i + 1}: GameObject = NULL");
                            continue;
                        }
                        
                        var img = toolsImages[i].GetComponent<UnityEngine.UI.Image>();
                        if (img == null)
                        {
                            Debug.Log($"  Slot {i + 1}: brak Image component");
                            continue;
                        }
                        
                        string spriteName = img.sprite != null ? img.sprite.name : "EMPTY";
                        Debug.Log($"  Slot {i + 1}: Sprite = '{spriteName}', Alpha = {img.color.a:F2}");
                    }
                }
                else
                {
                    Debug.LogWarning("  Could not access toolsImages array!");
                }
            }
            else
            {
                playerStats.AddMoney(item.price);
                Debug.LogError("ShopTrigger: Failed to add item to equipment. Money refunded.");
            }
        }
        
        private void OnDestroy()
        {
            if (shopUIController != null)
                shopUIController.OnBuyButtonClicked -= HandlePurchase;
        }
    }
}
