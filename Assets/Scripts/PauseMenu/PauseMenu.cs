using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void SaveAndExit()
    {
        SaveGame();

        Time.timeScale = 1f;
        IsPaused = false;

        SceneManager.LoadScene("Main Menu"); 
    }

    private void SaveGame()
    {
        Debug.Log("Game Saved (placeholder)");
    }
}
