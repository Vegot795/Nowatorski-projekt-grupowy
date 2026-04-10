using System;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [field: SerializeField] public bool IsOccupied { get; private set; }
    [field: SerializeField] public ItemSO ItemInSlot { get; private set; }
    [field: SerializeField] public ushort ItemAmount { get; private set; }

    private void changeOccupancy(bool occupancyValue)
    {
        IsOccupied = occupancyValue;
    }
    public void AddItemAmount(ushort amount)
    {
        ItemAmount += amount;
    }
    public void RemoveItemAmount(ushort amount)
    {
        ItemInSlot = null;
        ItemAmount -= amount;
    }
    public void AddItem(ItemSO item, ushort amount)
    {
        ItemInSlot = item;
        ItemAmount += amount;
    }
    public void RemoveItem()
    {
        ItemInSlot = null;
        ItemAmount = 0;
    }

}
