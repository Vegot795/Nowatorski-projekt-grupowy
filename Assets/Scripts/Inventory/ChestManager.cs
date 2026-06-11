using UnityEngine;
using System.Collections.Generic;

public class ChestManager : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> chestSlots;
    public ItemSO testItem;
    [SerializeField] private GameObject chestUI;
    private bool isChestOpen = false;


    void addItemToChest(ItemSO item, int amount)
    {
        List<InventorySlot> thisItemSlots = chestSlots.FindAll(x => x.ItemInSlot == item);
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
                chestSlots[NextEmptySlot()].AddItem(item, newAmount);
            }


        }
        else
        {
            if (NextEmptySlot() != -1)
            {
                Debug.Log("Pl doesn't have slots with this item in inv");
                chestSlots[NextEmptySlot()].AddItem(item, amount);
            }

        }


    }

    int NextEmptySlot()
    {
        if (chestSlots == null || chestSlots.Count == 0)
        {
            Debug.Log("not found slots ");
            return -1; //there is no slot with this pos, this means no slots in list
        }
        else
        {
            for (int i = 0; i < chestSlots.Count; i++)
            {
                if (chestSlots[i].IsOccupied == false)
                {
                    Debug.Log("found next empty slot: " + i);
                    return i;
                }
            }
            Debug.Log("not found next empty slot, inv is full ");
            return -1; //there is no slot with this pos, this means inv is full

        }


    }
    void ToggleChest()
    {
        Debug.Log("inv visibility change");
        isChestOpen = !isChestOpen;
        chestUI.SetActive(isChestOpen);
    }
}
