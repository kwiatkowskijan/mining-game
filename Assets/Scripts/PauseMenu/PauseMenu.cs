using UnityEngine;
using UnityEngine.SceneManagement;
using MiningGame.SaveSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;

    [Header("UI Panels")]
    public GameObject pauseMenuPanel; 

    private void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    
    public void OptionsPlaceholder()
    {
        Debug.Log("Options clicked (placeholder, nothing happens).");
    }

    /// <summary>
    /// Zapisz grę - podepnij pod przycisk "Save"
    /// </summary>
    public void OnSaveClicked()
    {
        SaveGame();
        Debug.Log("PauseMenu: Game saved!");
    }

    public void SaveAndExit()
    {
        SaveGame();

        Time.timeScale = 1f;
        IsPaused = false;

        SceneManager.LoadScene("Main Menu"); 
    }

    private void SaveGame()
    {
        Debug.Log($"PauseMenu: Attempting to save. SaveManager.Instance = {SaveManager.Instance}");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
        else
        {
            Debug.LogError("PauseMenu: SaveManager.Instance is NULL! Make sure SaveManager exists in the scene.");
            
            // Spróbuj znaleźć SaveManager w scenie
            var sm = FindFirstObjectByType<SaveManager>();
            if (sm != null)
            {
                Debug.Log("PauseMenu: Found SaveManager via FindFirstObjectByType, saving...");
                sm.SaveGame();
            }
            else
            {
                Debug.LogError("PauseMenu: No SaveManager found in scene at all!");
            }
        }
    }
}
