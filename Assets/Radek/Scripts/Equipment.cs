using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiningGame
{
    public class Equipment : MonoBehaviour
    {
        public int chosenSlot;
        public float playerRubble;
        public float rubbleMax=5;
        [SerializeField] private TextMeshProUGUI rubble;
        [SerializeField] private GameObject[] slots;
        [SerializeField] private Sprite[] activeSlots;
        [SerializeField] private Sprite[] inactiveSlots;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            chosenSlot = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                chosenSlot = 1;
                SlotSwitch(chosenSlot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                chosenSlot = 2;
                SlotSwitch(chosenSlot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                chosenSlot = 3;
                SlotSwitch(chosenSlot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                chosenSlot=4;
                SlotSwitch(chosenSlot);
            }
        }

        void SlotSwitch(int chosenSlot)
        {
            switch(chosenSlot)
            {
                case 1:
                    slots[0].gameObject.GetComponent<Image>().sprite = activeSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = inactiveSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = inactiveSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = inactiveSlots[3];
                    break;

                case 2:
                    slots[0].gameObject.GetComponent<Image>().sprite = inactiveSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = activeSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = inactiveSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = inactiveSlots[3];
                    break;

                case 3:
                    slots[0].gameObject.GetComponent<Image>().sprite = inactiveSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = inactiveSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = activeSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = inactiveSlots[3];
                    break;

                case 4:
                    slots[0].gameObject.GetComponent<Image>().sprite = inactiveSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = inactiveSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = inactiveSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = activeSlots[3];
                    break;
            }
        }

        public void updateRubble()
        {
            if(playerRubble>=5) playerRubble=5;
            rubble.text = playerRubble.ToString();
        }
    }
}
