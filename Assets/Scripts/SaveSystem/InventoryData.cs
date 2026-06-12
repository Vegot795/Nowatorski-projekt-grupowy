using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public bool DataIsOccupied;
    public int DataItemAmount;
    public string DataItemName;

    public InventoryData(InventorySlot slot)
    {
        DataIsOccupied = slot.IsOccupied;

        if (slot.IsOccupied)
        {
            DataItemAmount = slot.ItemAmount;
            DataItemName = slot.ItemInSlot.name;
        }
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryData> slots = new();

}