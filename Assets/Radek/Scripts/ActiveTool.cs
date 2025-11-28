using UnityEngine;

namespace MiningGame
{
    public class ActiveTool : MonoBehaviour
    {
        public Equipment eq;

        [Header("Tool Images")]
        [SerializeField] private Sprite crudePickaxe;
        [SerializeField] private GameObject diggingManager;

        public void EnableToolScripts()
        {
            if (eq.activeTool == crudePickaxe && eq.enabled)
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
