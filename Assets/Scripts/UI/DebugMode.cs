using UnityEngine;
using TMPro;

namespace MiningGame
{
    public class DebugMode : MonoBehaviour
    {
        private TextMeshProUGUI _debugText;
        private Transform _player;
        void Start()
        {
            _debugText = GetComponentInChildren<TextMeshProUGUI>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            Vector3 playerPos = _player.position;
            _debugText.text = $"X: {playerPos.x:F2}\nY: {playerPos.y:F2}";
        }
    }
}
