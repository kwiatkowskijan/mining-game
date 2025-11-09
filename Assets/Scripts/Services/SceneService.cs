using UnityEngine;
using UnityEngine.SceneManagement;
using MiningGame.Core.Interfaces;

namespace MiningGame.Services
{
    public class SceneService : ISceneService
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}