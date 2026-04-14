using System;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [field: SerializeField] public bool IsOccupied { get; private set; }
    [field: SerializeField] public ItemSO ItemInSlot { get; private set; }
    [field: SerializeField] public int ItemAmount { get; private set; }

    private void changeOccupancy(bool occupancyValue)
    {
        IsOccupied = occupancyValue;
    }
    public void AddItemAmount(int amount)
    {
        ItemAmount += amount;
    }
    public void RemoveItemAmount(int amount)
    {
        ItemInSlot = null;
        ItemAmount -= amount;
    }
    public void AddItem(ItemSO item, int amount)
    {
        IsOccupied = true;
        ItemInSlot = item;
        ItemAmount += amount;
    }
    public void RemoveItem()
    {
        IsOccupied = true;
        ItemInSlot = null;
        ItemAmount = 0;
    }

}
