using System.Runtime.CompilerServices;
using UnityEngine;

namespace MiningGame
{
    public class EQButton : MonoBehaviour
    {
        [SerializeField] private GameObject frame;
        public Equipment equipment;

        public void Clicked()
        {
            frame.SetActive(true);
            equipment.slotsChange = false;
        }
    }
}

    
