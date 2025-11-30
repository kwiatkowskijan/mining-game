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

        public bool slotsChange=true;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            chosenSlot = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (slotsChange)
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
                    chosenSlot = 4;
                    SlotSwitch(chosenSlot);
                }
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

            activeTool = toolsImages[chosenSlot - 1].GetComponent<Image>().sprite;
        }

        public void updateRubble()
        {
            if(playerRubble>=5) playerRubble=5;
            rubble.text = playerRubble.ToString();
        }

        public void updateMineral()
        {
            playerMinerals++;
            minerals.text=playerMinerals.ToString();
        }
        public void toolAssignment()
        {
            Debug.Log("Działa");
        }
        
        public bool AddItem(ShopItemType itemType, Sprite itemSprite = null)
        {
            Debug.Log($"Equipment.AddItem called: itemType={itemType}, itemSprite={(itemSprite != null ? itemSprite.name : "NULL")}");
            Debug.Log($"Equipment: toolsImages array length = {toolsImages.Length}");
            
            for (int i = 0; i < toolsImages.Length; i++)
            {
                if (toolsImages[i] == null)
                {
                    Debug.LogWarning($"Equipment: toolsImages[{i}] is NULL!");
                    continue;
                }
                
                Image slotImage = toolsImages[i].GetComponent<Image>();
                if (slotImage == null)
                {
                    Debug.LogWarning($"Equipment: toolsImages[{i}] has no Image component!");
                    continue;
                }
                
                Debug.Log($"Equipment: Checking slot {i}: sprite={(slotImage.sprite != null ? slotImage.sprite.name : "NULL")}, alpha={slotImage.color.a}");
                
                if (slotImage.sprite == null || slotImage.color.a < 0.1f)
                {
                    Debug.Log($"Equipment: Found empty slot at index {i}");
                    
                    if (itemSprite != null)
                    {
                        slotImage.sprite = itemSprite;
                        slotImage.color = Color.white;
                        Debug.Log($"Equipment: Set sprite to '{itemSprite.name}' and color to white");
                    }
                    else
                    {
                        Debug.LogWarning($"Equipment: itemSprite is NULL, cannot set sprite!");
                    }
                    
                    Debug.Log($"Equipment: Added {itemType} to slot {i + 1}");
                    return true;
                }
            }
            
            Debug.Log("Equipment: All slots are full!");
            return false;
        }
        
        public bool HasEmptySlot()
        {
            for (int i = 0; i < toolsImages.Length; i++)
            {
                Image slotImage = toolsImages[i].GetComponent<Image>();
                if (slotImage != null && (slotImage.sprite == null || slotImage.color.a < 0.1f))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
