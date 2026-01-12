using MiningGame.Core.Interfaces;
using UnityEngine;

namespace MiningGame.Services
{
    public class AudioService : IAudioService
    {
        private readonly AudioSource _sfxSource;
        private readonly AudioSource _musicSource;

        public AudioService(AudioSource sfx, AudioSource music)
        {
            _sfxSource = sfx;
            _musicSource = music;
        }

        public void PlaySfx(AudioClip audioClip)
        {
            _sfxSource.PlayOneShot(audioClip);
        }

        public void PlayMusic(AudioClip audioClip)
        {
            _musicSource.clip = audioClip;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }
    }
}