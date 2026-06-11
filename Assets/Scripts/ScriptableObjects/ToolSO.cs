using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolItem", menuName = "SO/ToolItemSO")]
public class ToolSO : ItemSO
{
    public enum ToolType
    {
        Axe,
        Hoe,
        Sword
    }

    public ToolType toolType;
    public Sprite toolSprite;
    public float baseSpeed;
    public string itemName;

    [NonSerialized]
    public float durability;

    public void UseTool()
    {
        durability -= 1;
        Debug.Log($"Used {itemName}, durability is now {durability}");
        if (durability <= 0)
        {
            Debug.Log($"{itemName} broke!");
            // Handle tool breaking logic here (e.g., remove from inventory)
        }
    }
}
