using MiningGame.Player;
using UnityEngine;

namespace MiningGame
{
    public class EquipmentSwaping : MonoBehaviour
    {
        [SerializeField] private GameObject interactionTooltip;
        private bool inTrigger=false;
        public Controller movement;
        [SerializeField] private GameObject EQMenu;
        public bool inMenu=false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (inTrigger && Input.GetKeyDown(KeyCode.E) && !inMenu)
            {
                EQMenu.SetActive(true);
                inMenu = true;
                movement.saveDefaults();
                movement.disableMovement();
            }
            else if(inTrigger && Input.GetKeyDown(KeyCode.E) && inMenu)
            {
                EQMenu.SetActive(false);
                inMenu = false;
                movement.enableMovement();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                interactionTooltip.SetActive(true);
                inTrigger = true;
                
            }
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                interactionTooltip.SetActive(false);
                inTrigger = false;
            }
        }
    }
}
