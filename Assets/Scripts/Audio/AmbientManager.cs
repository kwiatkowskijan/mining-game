using MiningGame.Core;
using MiningGame.Core.Interfaces;
using UnityEngine;

namespace MiningGame.AmbientManager
{
    public class AmbientManager : MonoBehaviour
    {
        private IAudioService _audioService;
        [SerializeField] private AudioClip ambientSound;

        private void Awake()
        {
            _audioService = ServiceLocator.Get<IAudioService>();
        }

        private void Start()
        {
            _audioService.PlayMusic(ambientSound);
        }
    }
}