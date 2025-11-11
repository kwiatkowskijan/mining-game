using UnityEngine;

namespace MiningGame
{
    public class ActiveTool : MonoBehaviour
    {
        public Equipment eq;

        [Header("Tool Images")]
        [SerializeField] private Sprite crudePickaxe;
        [SerializeField] private GameObject diggingManager;

        void Update()
        {
            if(eq.activeTool == crudePickaxe)
            {
                diggingManager.SetActive(true);
            }
            else
            {
                diggingManager.SetActive(false);
            }
        }
    }
}
