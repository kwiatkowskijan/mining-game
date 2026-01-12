using MiningGame.Player;
using System;
using UnityEngine;

namespace MiningGame
{
    public class EquipmentSwaping : MonoBehaviour
    {
        [SerializeField] private GameObject interactionTooltip;
        private bool inTrigger=false;
        private Controller movement;
        private BuildMode buildMode;
        [SerializeField] private GameObject EQMenu;
        public bool inEQMenu=false;
        public event Action OnEQMenuClosed;

        void Update()
        {
            if (inTrigger && Input.GetKeyDown(KeyCode.E) && !inEQMenu)
            {
                OpenEQMenu();
            }
            else if(inTrigger && Input.GetKeyDown(KeyCode.E) && inEQMenu)
            {
                CloseEQMenu();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                interactionTooltip.SetActive(true);
                inTrigger = true;
                movement = collision.gameObject.GetComponent<Controller>();
                buildMode = collision.gameObject.GetComponent<BuildMode>();
            }
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                interactionTooltip.SetActive(false);
                inTrigger = false;
                movement = null;
                buildMode = null;
            }
        }

        private void OpenEQMenu()
        {
            EQMenu.SetActive(true);
            inEQMenu = true;
            movement.saveDefaults();
            movement.disableMovement();
            if (buildMode.isInBuildMode) buildMode.HandleBuildModeToggle();
            buildMode.inOtherMenu = true;

            var allocations = EQMenu.GetComponentsInChildren <ToolAllocation>(true);
            foreach (var allocation in allocations)
            {
                allocation.Register(this);
            }
        }

        private void CloseEQMenu()
        {
            OnEQMenuClosed?.Invoke();
            
            inEQMenu = false;
            buildMode.inOtherMenu = false;
            movement.enableMovement();

            EQMenu.SetActive(false);
        }
    }
}
