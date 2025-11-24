using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Map Generation";

    
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Map Generation");
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
        // ¿eby w edytorze tez 'wychodzilo' z gry:
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
