using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    public Sprite Icon;
    public string Description;
    public int MaxStackAmount;
}
