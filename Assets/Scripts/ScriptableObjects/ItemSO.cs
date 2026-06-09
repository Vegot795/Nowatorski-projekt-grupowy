using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Item", menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    public Sprite Icon;
    public string Description;
    public ushort MaxStackAmount;
    public int buyPrice = 10;
    [Range(0, 1)]
    public float sellPriceMultiplier = 0.5f;

    public int GetSellPrice()
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
    }
}

