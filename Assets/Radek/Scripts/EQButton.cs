using System.Runtime.CompilerServices;
using UnityEngine;

namespace MiningGame
{
    public class EQButton : MonoBehaviour
    {
        [SerializeField] private GameObject frame;
        public Equipment equipment;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void Clicked()
        {
            frame.SetActive(true);
            equipment.slotsChange = false;
        }
    }
}

    
