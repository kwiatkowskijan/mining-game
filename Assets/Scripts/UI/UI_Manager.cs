using UnityEngine;
using UnityEngine.UI;

namespace MiningGame.UI
{
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        private int health;

        private void Start()
        {
            health = StatsManager.Instance.Health;
        }

        private void Update()
        {
            healthBar.fillAmount = health;
        }
    }
}
