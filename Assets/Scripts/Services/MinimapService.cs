using UnityEngine;
using MiningGame.Core.Interfaces;

namespace MiningGame.Services
{
    public class MinimapService : IMinimapService
    {
        public Camera minimapCamera;
        private int _maxZoom = 20;
        private int _minZoom = 10;


        public MinimapService(Camera camera)
        {
            minimapCamera = camera;
        }

        public void zoomIn(float amount)
        {
            if (minimapCamera.orthographicSize > _minZoom)
                minimapCamera.orthographicSize -= amount;
            else
                return;
        }

        public void zoomOut(float amount)
        {
            if (minimapCamera.orthographicSize < _maxZoom)
                minimapCamera.orthographicSize += amount;
            else
                return;
        }
    
    }
}