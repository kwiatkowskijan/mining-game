using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MiningGame.Shop
{
    public class ShopUIController : MonoBehaviour
    {
        [Header("TextMeshPro fields")]
        [SerializeField] private TMP_Text[] itemNameTexts = new TMP_Text[3];
        [SerializeField] private TMP_Text[] itemPriceTexts = new TMP_Text[3];
        
        [Header("Buy Buttons")]
        [SerializeField] private Button[] buyButtons = new Button[3];
        
        private ShopItemData[] _currentItems;
        public event Action<int> OnBuyButtonClicked;
        
        private void Awake()
        {
            for (int i = 0; i < buyButtons.Length; i++)
            {
                if (buyButtons[i] != null)
                {
                    int index = i;
                    buyButtons[i].onClick.AddListener(() => HandleBuyButton(index));
                }
            }
        }
        
        public void ShowItems(ShopItemData[] items)
        {
            gameObject.SetActive(true);
            if (items == null) return;
            
            _currentItems = items;
            Clear();

            int count = Mathf.Min(items.Length, itemNameTexts.Length);
            for (int i = 0; i < count; i++)
            {
                if (itemNameTexts[i] != null)
                    itemNameTexts[i].text = items[i].itemType.ToString();
                
                if (itemPriceTexts[i] != null)
                    itemPriceTexts[i].text = $"${items[i].price}";
                
                if (buyButtons[i] != null)
                    buyButtons[i].gameObject.SetActive(true);
            }
        }
        
        private void HandleBuyButton(int index)
        {
            if (_currentItems != null && index < _currentItems.Length)
            {
                OnBuyButtonClicked?.Invoke(index);
            }
        }
        
        public ShopItemData GetItemAtIndex(int index)
        {
            if (_currentItems != null && index >= 0 && index < _currentItems.Length)
                return _currentItems[index];
            return null;
        }
        
        public void Clear()
        {
            if (itemNameTexts != null)
            {
                for (int i = 0; i < itemNameTexts.Length; i++)
                {
                    if (itemNameTexts[i] != null) 
                        itemNameTexts[i].text = string.Empty;
                }
            }
            
            if (itemPriceTexts != null)
            {
                for (int i = 0; i < itemPriceTexts.Length; i++)
                {
                    if (itemPriceTexts[i] != null) 
                        itemPriceTexts[i].text = string.Empty;
                }
            }
            
            if (buyButtons != null)
            {
                for (int i = 0; i < buyButtons.Length; i++)
                {
                    if (buyButtons[i] != null)
                        buyButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
