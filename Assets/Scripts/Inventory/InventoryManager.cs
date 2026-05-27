using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] public List<InventorySlot> inventorySlots;
    [SerializeField] public List<InventorySlot> toolbarSlots;
    public ItemSO testItem;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject ToolbarUI;
    [SerializeField] private InventorySlot currentHeldSlot;
    private bool isInvOpen = true;
    public Color baseColor = new Color(255, 255, 255, 100);

    public void Start()
    {
        inventoryUI = GameObject.Find("InventoryUI");
        inventoryUI.SetActive(false);
        isInvOpen = false;

        foreach (var toolbarSlot in ToolbarUI.GetComponentsInChildren<InventorySlot>())
        {
            toolbarSlots.Add(toolbarSlot);
        }

        currentHeldSlot = toolbarSlots[0];
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            addItemToInv(testItem, 30);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            removeItemFromInv(3);
        }
    }


    void removeItemFromInv(int slotIndex)
    {

        Debug.Log("Slot to remove found");
        //inventorySlots[slotIndex].removeItemSlot.RemoveItem();

    }
    void addItemToInv(ItemSO item, int amount)
    {
        List<InventorySlot> thisItemSlots = inventorySlots.FindAll(x => x.ItemInSlot == item);
        if (thisItemSlots.Count > 0)
        {
            Debug.Log("Pl have slots with this item in inv");
            int newAmount = amount;
            for (int i = 0; i < thisItemSlots.Count; i++)
            {
                int difference = thisItemSlots[i].ItemInSlot.MaxStackAmount - thisItemSlots[i].ItemAmount;
                int toAdd = Mathf.Min(difference, newAmount);

                thisItemSlots[i].AddItemAmount(toAdd);

                newAmount -= toAdd;
                if (newAmount == 0) break;
            }
            if (newAmount > 0 && NextEmptySlot() != -1)
            {
                Debug.Log("Pl have slots with this item in inv, but all taken");
                inventorySlots[NextEmptySlot()].AddItem(item, newAmount);
            }


        }
        else
        {
            if (NextEmptySlot() != -1)
            {
                Debug.Log("Pl doesn't have slots with this item in inv");
                inventorySlots[NextEmptySlot()].AddItem(item, amount);
            }

        }


    }

    int NextEmptySlot()
    {
        if (inventorySlots == null || inventorySlots.Count == 0)
        {
            Debug.Log("not found slots ");
            return -1; //there is no slot with this pos, this means no slots in list
        }
        else
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsOccupied == false)
                {
                    Debug.Log("found next empty slot: " + i);
                    return i;
                }
            }
            Debug.Log("not found next empty slot, inv is full ");
            return -1; //there is no slot with this pos, this means inv is full

        }


    }
    public void ToggleInventory()
    {
        Debug.Log("inv visibility change");
        isInvOpen = !isInvOpen;
        inventoryUI.SetActive(isInvOpen);
    }

    private void SlotSetToBeCurrentHeld(InventorySlot toolbarSlot)
    {
        Image thisImage = toolbarSlot.GetComponentInChildren<Image>();
        int currentIndex = toolbarSlots.IndexOf(toolbarSlot);
        
        thisImage.color = Color.red;
        currentHeldSlot = toolbarSlots[currentIndex];
        currentHeldSlot.isCurrentHeldSlot = true;
    }

    private void SlotSetToBeFree(InventorySlot toolbarSlot)
    {
        Image thisImage = toolbarSlot.GetComponentInChildren<Image>();
        thisImage.color = baseColor;
        toolbarSlot.isCurrentHeldSlot = false;
    }

    public void MoveCurrentSlot(int direction)
    {
        int currentIndex = toolbarSlots.IndexOf(currentHeldSlot);
        int newIndex = (currentIndex + direction) % toolbarSlots.Count;
        if (newIndex < 0) newIndex += toolbarSlots.Count;
        SlotSetToBeFree(currentHeldSlot);
        currentHeldSlot = toolbarSlots[newIndex];
        SlotSetToBeCurrentHeld(currentHeldSlot);
    }
     
}
