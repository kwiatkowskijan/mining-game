using System.Collections.Generic;
using MiningGame.Core.Interfaces;
using MiningGame.WorldGeneration;
using MiningGame.Services;
using UnityEngine;

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
        }
    }
}