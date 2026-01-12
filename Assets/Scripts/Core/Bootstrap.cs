using System.Collections.Generic;
using MiningGame.Core.Interfaces;
using MiningGame.WorldGeneration;
using MiningGame.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiningGame.Core
{
    [DefaultExecutionOrder(-1)]
    public class Bootstrap : MonoBehaviour
    {
        [Header("Minerals Service")]
        [SerializeField] private List<Mineral> mineralsDatabase;
        [Header("Audio Service")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ServiceLocator.Register<IMineralsService>(new MineralsService(mineralsDatabase));
            ServiceLocator.Register<IAudioService>(new AudioService(sfxSource, musicSource));
            ServiceLocator.Register<ISceneService>(new SceneService());
            ServiceLocator.Register<IMinimapService>(new MinimapService());

            Debug.Log("[Bootstrap] All services registered!");

            SceneManager.LoadScene("Main menu", LoadSceneMode.Single);
        }
    }
}