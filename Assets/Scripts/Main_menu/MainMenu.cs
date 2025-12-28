using UnityEngine;
using UnityEngine.SceneManagement;
using MiningGame.SaveSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Merging Everything";

    /// <summary>
    /// Nowa gra - tworzy świeży save i ładuje scenę
    /// </summary>
    public void OnNewGameClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.NewGame();
        }
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Wczytaj poprzednią grę
    /// </summary>
    public void OnLoadGameClicked()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.SaveExists())
        {
            Debug.Log("MainMenu: Loading game...");
            
            // Ustaw flagę w SaveManager że trzeba wczytać save po załadowaniu sceny
            SaveManager.Instance.RequestLoadAfterSceneLoad();
            
            // Załaduj scenę - SaveManager sam wczyta save gdy scena będzie gotowa
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("MainMenu: No save file found!");
        }
    }

    /// <summary>
    /// Sprawdza czy istnieje save - użyj do aktywacji/deaktywacji przycisku Load
    /// </summary>
    public bool HasSaveFile()
    {
        return SaveManager.Instance != null && SaveManager.Instance.SaveExists();
    }

    // Stara metoda dla kompatybilności
    public void OnPlayClicked()
    {
        OnNewGameClicked();
    }

    
    public void OnOptionsClicked()
    {
        
        Debug.Log("Opcje jeszcze nie sa zrobione");

        
    }

    
    public void OnExitClicked()
    {
        Debug.Log("Wyjscie z gry");

        Application.Quit(); 

#if UNITY_EDITOR
        // �eby w edytorze tez 'wychodzilo' z gry:
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
