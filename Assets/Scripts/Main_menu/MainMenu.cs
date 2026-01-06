using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("MergeEverything");
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
        // Żeby w edytorze tez 'wychodzilo' z gry:
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
