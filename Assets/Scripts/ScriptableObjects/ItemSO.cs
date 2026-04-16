using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Item", menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    public Sprite Icon;
    public string Description;
    public ushort MaxStackAmount;
}
