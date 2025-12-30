using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiningGame.WorldGeneration;

namespace MiningGame.Shop
{
    /// <summary>
    /// Reprezentuje pojedynczy wpis minerału w sklepie (tekst + przycisk sell)
    /// </summary>
    public class MineralShopEntry : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text mineralInfoText; // "{Nazwa} x{ilość} - ${cena}"
        [SerializeField] private Button sellButton;

        private Mineral _mineral;
        private int _amount;
        private float _sellPrice;
        private System.Action<Mineral, int> _onSellCallback;

        public void Setup(Mineral mineral, int amount, float sellPrice, System.Action<Mineral, int> onSellCallback)
        {
            _mineral = mineral;
            _amount = amount;
            _sellPrice = sellPrice;
            _onSellCallback = onSellCallback;

            if (mineralInfoText != null)
            {
                float totalValue = sellPrice * amount;
                mineralInfoText.text = $"{mineral.blockName} x{amount} - ${totalValue:F0}";
            }

            if (sellButton != null)
            {
                sellButton.onClick.RemoveAllListeners();
                sellButton.onClick.AddListener(OnSellClicked);
            }

            gameObject.SetActive(true);
        }

        private void OnSellClicked()
        {
            if (_mineral != null && _amount > 0)
            {
                _onSellCallback?.Invoke(_mineral, _amount);
            }
        }

        public void Clear()
        {
            _mineral = null;
            _amount = 0;
            _sellPrice = 0f;
            _onSellCallback = null;

            if (mineralInfoText != null) mineralInfoText.text = "";
            if (sellButton != null) sellButton.onClick.RemoveAllListeners();

            gameObject.SetActive(false);
        }
    }
}

