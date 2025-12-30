using System;
using System.Collections.Generic;
using MiningGame.WorldGeneration;
using UnityEngine;

namespace MiningGame.Shop
{
    [CreateAssetMenu(fileName = "MineralSellPrices", menuName = "Mining Game/Shop/Mineral Sell Prices")]
    public class MineralSellPrices : ScriptableObject
    {
        [Serializable]
        public class MineralPrice
        {
            [Tooltip("Referencja do minerału")]
            public Mineral mineral;
            
            [Tooltip("Cena sprzedaży za 1 jednostkę minerału")]
            public float sellPrice;
        }

        [Header("Ceny sprzedaży minerałów")]
        [Tooltip("Lista cen dla każdego minerału")]
        public List<MineralPrice> mineralPrices = new List<MineralPrice>();
        
        public float GetSellPrice(Mineral mineral)
        {
            if (mineral == null)
            {
                Debug.LogWarning("MineralSellPrices: Mineral is null!");
                return 0f;
            }

            var priceData = mineralPrices.Find(p => p.mineral == mineral);
            if (priceData != null)
            {
                return priceData.sellPrice;
            }

            Debug.LogWarning($"MineralSellPrices: No price found for mineral '{mineral.blockName}'. Returning 0.");
            return 0f;
        }
        
        public float CalculateTotalValue(Mineral mineral, int amount)
        {
            return GetSellPrice(mineral) * amount;
        }

        public void SetSellPrice(Mineral mineral, float newPrice)
        {
            if (mineral == null) return;

            var priceData = mineralPrices.Find(p => p.mineral == mineral);
            if (priceData != null)
            {
                priceData.sellPrice = newPrice;
            }
            else
            {
                mineralPrices.Add(new MineralPrice
                {
                    mineral = mineral,
                    sellPrice = newPrice
                });
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Auto-Fill Missing Minerals")]
        private void AutoFillMissingMinerals()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Mineral");
            
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Mineral mineral = UnityEditor.AssetDatabase.LoadAssetAtPath<Mineral>(path);
                
                if (mineral != null && !mineralPrices.Exists(p => p.mineral == mineral))
                {
                    float price = GetDefaultPriceForMineral(mineral.blockName);
                    
                    mineralPrices.Add(new MineralPrice
                    {
                        mineral = mineral,
                        sellPrice = price
                    });
                    Debug.Log($"Added mineral: {mineral.blockName} with price {price}");
                }
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"MineralSellPrices: Auto-filled. Total minerals: {mineralPrices.Count}");
        }
        
        private float GetDefaultPriceForMineral(string mineralName)
        {
            switch (mineralName.ToLower())
            {
                // Podstawowe minerały (niskie ceny)
                case "iron":
                    return 20f;
                case "tin":
                    return 25f;
                case "sulfur":
                    return 40f;
                
                // Średnio rzadkie (średnie ceny)
                case "quartz":
                    return 35f;
                case "lapis":
                    return 50f;
                case "flourite":
                    return 60f;
                case "malachite":
                    return 65f;
                case "silver":
                    return 70f;
                
                // Rzadkie (wysokie ceny)
                case "topaz":
                    return 100f;
                case "amethyst":
                    return 120f;
                case "obsidian":
                    return 130f;
                case "gold":
                    return 150f;
                case "sapphire":
                    return 180f;
                
                // Bardzo rzadkie (najwyższe ceny)
                case "uranium":
                    return 250f;
                case "diamond":
                    return 300f;
                case "pink diamond":
                case "pinkdiamond":
                    return 600f;
                
                // Domyślna cena dla nieznanych minerałów
                default:
                    Debug.LogWarning($"Unknown mineral '{mineralName}', using default price 50");
                    return 50f;
            }
        }

        [ContextMenu("Sort by Mineral Name")]
        private void SortByName()
        {
            mineralPrices.Sort((a, b) => 
            {
                if (a.mineral == null || b.mineral == null) return 0;
                return string.Compare(a.mineral.blockName, b.mineral.blockName, StringComparison.Ordinal);
            });
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("MineralSellPrices: Sorted by name");
        }
#endif
    }
}

