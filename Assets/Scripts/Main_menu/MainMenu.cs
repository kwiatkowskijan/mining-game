using UnityEngine;
using UnityEngine.SceneManagement;
using MiningGame.Core.Interfaces;
using MiningGame.Core;

public class MainMenu : MonoBehaviour
{
    private IMineralsService _mineralsService;

    private void Awake()
    {
        _mineralsService = ServiceLocator.Get<IMineralsService>();
    }
    public void OnPlayClicked()
    {
        SceneManager.LoadScene(2);
        _mineralsService.UndisoverAllMinerals();
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
