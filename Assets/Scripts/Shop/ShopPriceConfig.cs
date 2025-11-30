using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiningGame.Shop
{
    [CreateAssetMenu(fileName = "ShopPriceConfig", menuName = "Scriptable Objects/Shop")]
    public class ShopPriceConfig : ScriptableObject
    {
        public List<ShopItemPrice> itemPrices = new List<ShopItemPrice>
        {
            new ShopItemPrice { itemType = ShopItemType.Tool, price = 50 },
            new ShopItemPrice { itemType = ShopItemType.Armor, price = 50 },
            new ShopItemPrice { itemType = ShopItemType.Misc, price = 15 },
            new ShopItemPrice { itemType = ShopItemType.Component, price = 25 }
        };
        
        public int GetPrice(ShopItemType itemType)
        {
            var priceData = itemPrices.Find(x => x.itemType == itemType);
            return priceData != null ? priceData.price : 0;
        }
        
        public Sprite GetIcon(ShopItemType itemType)
        {
            var priceData = itemPrices.Find(x => x.itemType == itemType);
            return priceData != null ? priceData.icon : null;
        }
        
        public void SetPrice(ShopItemType itemType, int newPrice)
        {
            var priceData = itemPrices.Find(x => x.itemType == itemType);
            if (priceData != null)
            {
                priceData.price = newPrice;
            }
            else
            {
                itemPrices.Add(new ShopItemPrice { itemType = itemType, price = newPrice });
            }
        }
    }
    
    [Serializable]
    public class ShopItemPrice
    {
        public ShopItemType itemType;
        public int price = 10;
        public Sprite icon;
    }
}

