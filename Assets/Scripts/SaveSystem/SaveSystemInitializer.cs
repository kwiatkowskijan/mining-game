using UnityEngine;

namespace MiningGame.SaveSystem
{
    /// <summary>
    /// Dodaj ten skrypt do MAIN MENU.
    /// Stworzy SaveManager i WorldChangeTracker które przetrwają między scenami.
    /// </summary>
    public class SaveSystemInitializer : MonoBehaviour
    {
        private void Awake()
        {
            if (SaveManager.Instance == null)
            {
                GameObject saveManagerObj = new GameObject("SaveManager");
                saveManagerObj.AddComponent<SaveManager>();
                Debug.Log("SaveSystemInitializer: Created SaveManager in Main Menu.");
            }
            else
            {
                Debug.Log("SaveSystemInitializer: SaveManager already exists, skipping creation.");
            }
            
            if (WorldChangeTracker.Instance == null)
            {
                GameObject trackerObj = new GameObject("WorldChangeTracker");
                trackerObj.AddComponent<WorldChangeTracker>();
                Debug.Log("SaveSystemInitializer: Created WorldChangeTracker in Main Menu.");
            }
            else
            {
                Debug.Log("SaveSystemInitializer: WorldChangeTracker already exists, skipping creation.");
            }
        }
    }
}

