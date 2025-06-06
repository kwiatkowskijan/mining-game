using UnityEngine;
using UnityEngine.UI;

namespace MiningGame
{
    public class ToolAllocation : MonoBehaviour
    {
        [SerializeField] private GameObject[] itemSlotsImages;
        [SerializeField] private GameObject tool;
        [SerializeField] private Sprite toolImage;
        public Equipment equipment;
        private int chosenSlot;

        private int i = 0;

        // Update is called once per frame
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
            itemSlotsImages[chosenSlot].GetComponent<Image>().sprite = toolImage;
            itemSlotsImages[chosenSlot].SetActive(true);
            //equipment.whatInSlots[chosenSlot] = tool;

            foreach (var itemSlot in itemSlotsImages) 
            {
                if (itemSlot != itemSlotsImages[chosenSlot])
                {
                    if(itemSlot.GetComponent<Image>().sprite == toolImage)
                    {
                        itemSlot.GetComponent<Image>().sprite = null;
                        //equipment.whatInSlots[i] = null;
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
