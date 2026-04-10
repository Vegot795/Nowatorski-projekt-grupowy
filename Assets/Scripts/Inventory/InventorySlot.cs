using System;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [field: SerializeField] public bool IsOccupied { get; private set; }
    [field: SerializeField] public ItemSO ItemInSlot { get; private set; }
    [field: SerializeField] public ushort ItemAmount { get; private set; }

    private void ChangeOccupancy(bool occupancyValue)
    {
        IsOccupied = occupancyValue;
    }

}
