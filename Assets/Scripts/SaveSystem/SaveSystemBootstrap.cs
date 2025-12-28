using UnityEngine;

namespace MiningGame.SaveSystem
{
    /// <summary>
    /// Automatycznie tworzy SaveManager i WorldChangeTracker jeśli nie istnieją.
    /// Dodaj ten skrypt do DOWOLNEGO obiektu w scenie gry (np. do gracza lub GameManager).
    /// </summary>
    public class SaveSystemBootstrap : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool autoCreateSaveManager = true;
        [SerializeField] private bool autoCreateWorldTracker = true;

        private void Awake()
        {
            // SaveManager i WorldChangeTracker używają DontDestroyOnLoad
            // Więc jeśli Instance już istnieje (przeniesione z innej sceny), NIE TWÓRZ NOWEGO
            
            if (autoCreateSaveManager && SaveManager.Instance == null)
            {
                CreateSaveManager();
            }
            else if (SaveManager.Instance != null)
            {
                Debug.Log("SaveSystemBootstrap: SaveManager already exists (DontDestroyOnLoad). Skipping creation.");
            }

            if (autoCreateWorldTracker && WorldChangeTracker.Instance == null)
            {
                CreateWorldChangeTracker();
            }
            else if (WorldChangeTracker.Instance != null)
            {
                Debug.Log("SaveSystemBootstrap: WorldChangeTracker already exists (DontDestroyOnLoad). Skipping creation.");
            }
        }

        private void CreateSaveManager()
        {
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();
            Debug.Log("SaveSystemBootstrap: Created SaveManager automatically.");
        }

        private void CreateWorldChangeTracker()
        {
            GameObject trackerObj = new GameObject("WorldChangeTracker");
            trackerObj.AddComponent<WorldChangeTracker>();
            Debug.Log("SaveSystemBootstrap: Created WorldChangeTracker automatically.");
        }
    }
}

