using UnityEngine;
using MiningGame.Core.Interfaces;

namespace MiningGame.Services
{
    public class MinimapService : IMinimapService
    {
        private float initialZoom = 15f;
        public float CurrentZoom { get; private set; }
        public float MinZoom => 10f;
        public float MaxZoom => 20f;

        public MinimapService()
        {
            CurrentZoom = initialZoom;
        }

        public void ZoomIn(float amount)
        {
            CurrentZoom = Mathf.Max(CurrentZoom - amount, MinZoom);
        }

        public void ZoomOut(float amount)
        {
            CurrentZoom = Mathf.Min(CurrentZoom + amount, MaxZoom);
        }
    }
}