using System.Linq;
using UnityEngine;
using MiningGame.Shop;

namespace MiningGame
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Shop Settings")]
        private const int ItemsToGenerate = 3;
        
        [Header("Price Configuration")]
        [SerializeField] private ShopPriceConfig priceConfig;
        
        private ShopItemType[] _generatedItems = new ShopItemType[0];

        void Start()
        {
            GenerateShopItems();
        }

        public void GenerateShopItems()
        {
            var values = System.Enum.GetValues(typeof(ShopItemType)).Cast<ShopItemType>().ToList();
            int available = values.Count;

            _generatedItems = new ShopItemType[ItemsToGenerate];

            if (available == 0)
            {
                Debug.LogWarning("ShopManager: No ShopItemType values available.");
                return;
            }

            if (available >= ItemsToGenerate)
            {
                for (int i = values.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    var tmp = values[i];
                    values[i] = values[j];
                    values[j] = tmp;
                }

                for (int i = 0; i < ItemsToGenerate; i++)
                {
                    _generatedItems[i] = values[i];
                }
            }
            else
            {
                for (int i = 0; i < available; i++)
                {
                    _generatedItems[i] = values[i];
                }

                for (int i = available; i < ItemsToGenerate; i++)
                {
                    _generatedItems[i] = values[Random.Range(0, available)];
                }
            }
        }
        
        public ShopItemType[] GetGeneratedItems() => _generatedItems;
        
        public int GetItemPrice(ShopItemType itemType)
        {
            if (priceConfig == null)
            {
                Debug.LogWarning("ShopManager: Price config is not assigned!");
                return 0;
            }
            return priceConfig.GetPrice(itemType);
        }
        
        public ShopItemData GetItemData(ShopItemType itemType)
        {
            int price = GetItemPrice(itemType);
            Sprite icon = priceConfig != null ? priceConfig.GetIcon(itemType) : null;
            return new ShopItemData(itemType, price, icon);
        }
    }
}
