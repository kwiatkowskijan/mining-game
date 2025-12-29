using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiningGame.SaveSystem
{

    public class SaveLoadUI : MonoBehaviour
    {
        [Header("Buttons (Optional - can use OnClick events)")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button saveGameButton;

        [Header("Scene Settings")]
        [SerializeField] private string gameSceneName = "Map Generation";

        [Header("UI Feedback")]
        [SerializeField] private GameObject noSaveMessage;

        private void Awake()
        {
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGameClicked);

            if (loadGameButton != null)
                loadGameButton.onClick.AddListener(OnLoadGameClicked);

            if (saveGameButton != null)
                saveGameButton.onClick.AddListener(OnSaveGameClicked);
        }

        private void Start()
        {
            UpdateLoadButtonState();
        }
        
        private void UpdateLoadButtonState()
        {
            bool saveExists = SaveManager.Instance != null && SaveManager.Instance.SaveExists();
            
            if (loadGameButton != null)
                loadGameButton.interactable = saveExists;

            if (noSaveMessage != null)
                noSaveMessage.SetActive(!saveExists);
        }

        // ==========================================
        // METODY DO PODPIĘCIA POD PRZYCISKI (OnClick)
        // ==========================================
        
        public void OnNewGameClicked()
        {
            Debug.Log("SaveLoadUI: Starting new game...");

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.NewGame();
            }
            
            SceneManager.LoadScene(gameSceneName);
        }
        
        public void OnLoadGameClicked()
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("SaveLoadUI: SaveManager not found!");
                return;
            }

            if (!SaveManager.Instance.SaveExists())
            {
                Debug.LogWarning("SaveLoadUI: No save file exists!");
                return;
            }

            Debug.Log("SaveLoadUI: Loading saved game...");
            
            SceneManager.LoadScene(gameSceneName);
            
            SceneManager.sceneLoaded += OnSceneLoadedForLoad;
        }

        private void OnSceneLoadedForLoad(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedForLoad;
            
            StartCoroutine(LoadAfterDelay());
        }

        private System.Collections.IEnumerator LoadAfterDelay()
        {
            yield return null;
            yield return null;

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }
        }
        
        public void OnSaveGameClicked()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
                Debug.Log("SaveLoadUI: Game saved!");
            }
            else
            {
                Debug.LogWarning("SaveLoadUI: SaveManager not found!");
            }
        }
    }
}

