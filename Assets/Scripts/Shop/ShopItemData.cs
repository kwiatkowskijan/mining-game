using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiningGame.Shop
{
    [Serializable]
    public class ShopItemData
    {
        public ShopItemType itemType;
        public int price;
        public Sprite icon;
        
        public ShopItemData(ShopItemType type, int price, Sprite icon = null)
        {
            this.itemType = type;
            this.price = price;
            this.icon = icon;
        }
    }
}

