using UnityEngine;
using TMPro;

namespace MiningGame.Shop
{
    public class ShopUIController : MonoBehaviour
    {
        [Header("TextMeshPro fields")]
        [SerializeField] private TMP_Text[] tmpTexts = new TMP_Text[3];
        
        public void ShowItems(ShopItemType[] items)
        {
            gameObject.SetActive(true);
            if (items == null || tmpTexts == null) return;
            
            Clear();

            int count = Mathf.Min(items.Length, tmpTexts.Length);
            for (int i = 0; i < count; i++)
            {
                tmpTexts[i].text = items[i].ToString();
            }
        }
        
        // clear all text fields
        public void Clear()
        {
            if (tmpTexts == null) return;
            for (int i = 0; i < tmpTexts.Length; i++)
                if (tmpTexts[i] != null) tmpTexts[i].text = string.Empty;
        }
    }
}
