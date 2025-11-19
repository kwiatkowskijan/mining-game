using UnityEngine;
using UnityEngine.UI;

namespace MiningGame
{
    public class ToolAllocation : MonoBehaviour
    {
        [SerializeField] private GameObject[] toolImages;
        [SerializeField] private Sprite toolImageSprite;
        public Equipment equipment;
        private int chosenSlot;

        private int i = 0;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                chosenSlot = 0;
                allocatedSlot();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                chosenSlot = 1;
                allocatedSlot();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                chosenSlot = 2;
                allocatedSlot();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                chosenSlot = 3;
                allocatedSlot();
            }
        }
        void allocatedSlot()
        {
            toolImages[chosenSlot].GetComponent<Image>().sprite = toolImageSprite;
            toolImages[chosenSlot].SetActive(true);

            foreach (var itemSlot in toolImages) 
            {
                if (itemSlot != toolImages[chosenSlot])
                {
                    if(itemSlot.GetComponent<Image>().sprite == toolImageSprite)
                    {
                        itemSlot.GetComponent<Image>().sprite = null;
                        itemSlot.SetActive(false);
                    }
                }
                
                i++;
            }
            i = 0;
            equipment.slotsChange = true;
            gameObject.SetActive(false);
        }
    }
}
