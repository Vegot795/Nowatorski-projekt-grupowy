using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    void Start()
    {
        inventorySlots = new(); //initializing list with slots
    }

    void removeItemToInv(ItemSO item)
    {

    }
    void addItemToInv(ItemSO item, int amount)
    {
        List<InventorySlot> thisItemSlots = inventorySlots.FindAll(x => x.ItemInSlot == item);
        if (thisItemSlots != null)
        {
            //item w inv
            int rest = amount;
            for (int i = 0; i < thisItemSlots.Count; i++)
            {
                if (thisItemSlots[i].ItemAmount + rest == thisItemSlots[i].ItemInSlot.MaxStackAmount)
                {
                    thisItemSlots[i].AddItemAmount(rest);
                }
                else
                {
                    int diff = thisItemSlots[i].ItemInSlot.MaxStackAmount - thisItemSlots[i].ItemAmount;
                    rest = rest - diff;
                    thisItemSlots[i].AddItemAmount(diff);
                }

            }

        }
        else
        {
            //nie ma takiego itemu w inv
            //dodac do nastepnego wolnego
            NextEmptySlot();
        }
        //jesli juz jest tego typu item dodaje do stacku
        /*inventorySlots.Find(x => x.ItemInSlot == item && x.ItemAmount < x.ItemInSlot.MaxStackAmount);
        inventorySlots[k].ItemAmount += amount;
        if (inventorySlots[k].ItemAmount > inventorySlots[k].ItemInSlot.MaxStackAmount)
        {
            NextEmptySlot();
            inventorySlots[i].ItemInSlot = item;
            ushort newAmount = inventorySlots[k].ItemAmount - inventorySlots[k].ItemInSlot.MaxStackAmount;
            inventorySlots[i].ItemAmount += newAmount;
        }


        //jesli nie ma tego typu itemu lub pełny stack typu itemu szuka pustego slotu i tam dodaje item i amount
        NextEmptySlot()
        inventorySlots[NextEmptySlot()].
        inventorySlots[NextEmptySlot()].


    }
    int FindSameItemInInv(ItemSO item)
    {

    }*/
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
