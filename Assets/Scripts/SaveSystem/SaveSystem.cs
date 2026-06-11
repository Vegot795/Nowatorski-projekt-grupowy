using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SavePlayer(CharacterBasics characterBasics)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(characterBasics);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                PlayerData data = (PlayerData)formatter.Deserialize(stream);
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file not found");
            return null;

        }
    }

    public static void SaveInventory(List<InventorySlot> inventorySlots)
    {
        InventorySaveData saveData = new InventorySaveData();

        foreach (InventorySlot slot in inventorySlots)
        {
            saveData.slots.Add(new InventoryData(slot));
        }

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/inventory.data";

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, saveData);
        }
    }
    public static InventorySaveData LoadInventory()
    {
        string path = Application.persistentDataPath + "/inventory.data";

        if (!File.Exists(path))
            return null;

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return (InventorySaveData)formatter.Deserialize(stream);
        }
    }
}
