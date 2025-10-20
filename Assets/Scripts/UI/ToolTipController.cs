using UnityEngine;
using TMPro;

namespace MiningGame
{
    public class ToolTipController : MonoBehaviour
    {
        public static ToolTipController I;

        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            I = this;
            Hide();
        }

        public void Show(string msg, Vector2 screenPos)
        {
            if (panel == null || text == null) return;

            panel.gameObject.SetActive(true);
            text.text = msg;
            panel.position = screenPos + new Vector2(50f, -50f);
        }

        public void Hide()
        {
            if (panel != null)
                panel.gameObject.SetActive(false);
        }
    }
}
