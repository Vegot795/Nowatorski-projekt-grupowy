using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    public ItemSO testItem;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            addItemToInv(testItem, 3);
        }
    }

    void removeItemToInv(ItemSO item)
    {

    }
    void addItemToInv(ItemSO item, int amount)
    {
        List<InventorySlot> thisItemSlots = inventorySlots.FindAll(x => x.ItemInSlot == item);
        if (thisItemSlots.Count > 0)
        {
            //item w inv
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
                inventorySlots[NextEmptySlot()].AddItem(item, newAmount);
            }


        }
        else
        {
            if (NextEmptySlot() != -1)
            {
                inventorySlots[NextEmptySlot()].AddItem(item, amount);
            }

        }


    }
    int FindSameItemInInv(ItemSO item)
    {
        return 0;
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
    void OpenInventory()
    {

    }
    void CloseInventory()
    {

    }
    void showInUI()
    {

    }
}
