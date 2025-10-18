using UnityEngine;

namespace MiningGame
{
    public class BuildCategories : MonoBehaviour
    {
        [SerializeField] private GameObject[] categoryPanels;
        public int current = -1;

        private void OnEnable()
        {
            if (categoryPanels != null && categoryPanels.Length > 0)
            {
                ShowCategory(0);
            }
        }

        public void ShowCategory(int index)
        {
            if (categoryPanels == null || index < 0 || index >= categoryPanels.Length) return;
            for (int i = 0; i < categoryPanels.Length; i++) 
                categoryPanels[i].SetActive(i == index);
            current = index;
        }
    }
}
