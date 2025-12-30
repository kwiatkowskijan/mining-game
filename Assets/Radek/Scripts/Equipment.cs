using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace MiningGame
{
    public class Equipment : MonoBehaviour
    {
        public int chosenSlot;
        public Sprite activeTool;

        public float playerRubble;
        public float rubbleMax=5;
        [SerializeField] private int playerMinerals;
        [SerializeField] private TextMeshProUGUI rubble;
        [SerializeField] private TextMeshProUGUI minerals;

        [Header("Arrays")]
        [SerializeField] private GameObject[] slots;
        [SerializeField] private GameObject[] toolsImages;
        [SerializeField] private Sprite[] activeSlots;
        [SerializeField] private Sprite[] inactiveSlots;

        [SerializeField] private ActiveTool activeToolScript;

        public bool slotsChange=true;
        
        // Lista sprite'ów w każdym slotcie dla save/load
        public Sprite[] slotSprites = new Sprite[4];
        
        void Start()
        {
            chosenSlot = 0;
            // Inicjalizacja - zbierz bieżące sprite'y z toolsImages
            for (int i = 0; i < toolsImages.Length && i < 4; i++)
            {
                var img = toolsImages[i].GetComponent<Image>();
                if (img != null)
                {
                    slotSprites[i] = img.sprite;
                }
            }
        }

        void Update()
        {
            if (slotsChange)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    chosenSlot = 0;
                    SlotSwitch(chosenSlot);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    chosenSlot = 1;
                    SlotSwitch(chosenSlot);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    chosenSlot = 2;
                    SlotSwitch(chosenSlot);
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    chosenSlot = 3;
                    SlotSwitch(chosenSlot);
                }
            }
        }

        public void SlotSwitch(int chosenSlot)
        {
            switch(chosenSlot)
            {
                case 0:
                    slots[0].gameObject.GetComponent<Image>().sprite = activeSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = inactiveSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = inactiveSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = inactiveSlots[3];
                    break;

                case 1:
                    slots[0].gameObject.GetComponent<Image>().sprite = inactiveSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = activeSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = inactiveSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = inactiveSlots[3];
                    break;

                case 2:
                    slots[0].gameObject.GetComponent<Image>().sprite = inactiveSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = inactiveSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = activeSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = inactiveSlots[3];
                    break;

                case 3:
                    slots[0].gameObject.GetComponent<Image>().sprite = inactiveSlots[0];
                    slots[1].gameObject.GetComponent<Image>().sprite = inactiveSlots[1];
                    slots[2].gameObject.GetComponent<Image>().sprite = inactiveSlots[2];
                    slots[3].gameObject.GetComponent<Image>().sprite = activeSlots[3];
                    break;
            }

            activeTool = toolsImages[chosenSlot].GetComponent<Image>().sprite;
            CheckChosenSlot();
        }

        public void CheckChosenSlot()
        {
            activeToolScript.EnableToolScripts();
        }

        // === METODY DO SAVE/LOAD ===
        public void SaveSlotSprites()
        {
            for (int i = 0; i < toolsImages.Length && i < 4; i++)
            {
                var img = toolsImages[i].GetComponent<Image>();
                if (img != null)
                {
                    slotSprites[i] = img.sprite;
                }
            }
            Debug.Log($"Equipment: Saved slot sprites - Count: {slotSprites.Length}");
        }

        public void LoadSlotSprites()
        {
            for (int i = 0; i < toolsImages.Length && i < 4; i++)
            {
                var img = toolsImages[i].GetComponent<Image>();
                if (img != null && slotSprites[i] != null)
                {
                    img.sprite = slotSprites[i];
                    toolsImages[i].SetActive(true);
                    Debug.Log($"Equipment: Loaded sprite to slot {i}: {slotSprites[i].name}");
                }
                else if (img != null && slotSprites[i] == null)
                {
                    img.sprite = null;
                    toolsImages[i].SetActive(false);
                }
            }
        }
        
        public GameObject[] GetToolsImages() => toolsImages;
    }
}
