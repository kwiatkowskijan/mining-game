using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiningGame.SaveSystem
{
    /// <summary>
    /// Prosty kontroler UI do przycisków save/load/new game.
    /// Podepnij pod przyciski w menu głównym lub pause menu.
    /// </summary>
    public class SaveLoadUI : MonoBehaviour
    {
        [Header("Buttons (Optional - can use OnClick events)")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button saveGameButton;

        [Header("Scene Settings")]
        [SerializeField] private string gameSceneName = "Map Generation";

        [Header("UI Feedback")]
        [SerializeField] private GameObject noSaveMessage; // Opcjonalny tekst "Brak zapisu"

        private void Awake()
        {
            // Podepnij eventy do przycisków jeśli przypisane
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

        /// <summary>
        /// Aktualizuje stan przycisku Load (nieaktywny jeśli brak save'a)
        /// </summary>
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

        /// <summary>
        /// Nowa gra - tworzy nowy save i ładuje scenę gry
        /// </summary>
        public void OnNewGameClicked()
        {
            Debug.Log("SaveLoadUI: Starting new game...");

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.NewGame();
            }

            // Załaduj scenę gry
            SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// Wczytaj grę - ładuje poprzedni save
        /// </summary>
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
            
            // Najpierw załaduj scenę, potem wczytaj save
            SceneManager.LoadScene(gameSceneName);
            
            // Subskrybuj event załadowania sceny
            SceneManager.sceneLoaded += OnSceneLoadedForLoad;
        }

        private void OnSceneLoadedForLoad(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedForLoad;

            // Poczekaj chwilę i wczytaj save
            StartCoroutine(LoadAfterDelay());
        }

        private System.Collections.IEnumerator LoadAfterDelay()
        {
            // Poczekaj 1 klatkę aż wszystko się zainicjalizuje
            yield return null;
            yield return null;

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }
        }

        /// <summary>
        /// Zapisz grę - do użycia w pause menu
        /// </summary>
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

