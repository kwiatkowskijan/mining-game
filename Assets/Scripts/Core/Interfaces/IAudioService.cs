using System.Collections.Generic;
using UnityEngine;

namespace MiningGame.Core.Interfaces
{
    public interface IAudioService
    {
        void PlaySfx(AudioClip audioClip);
        void PlayMusic(AudioClip audioClip);
    }
}