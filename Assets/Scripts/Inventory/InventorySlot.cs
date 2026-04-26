using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [field: SerializeField] public bool IsOccupied { get; private set; }
    [field: SerializeField] public ItemSO ItemInSlot { get; private set; }
    [field: SerializeField] public int ItemAmount { get; private set; }
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI slotAmountText;
    void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        slotAmountText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateItemAmountText();
        slotAmountText.enabled = false;
    }

    public void AddItemAmount(int amount)
    {
        ItemAmount += amount;
        UpdateItemAmountText();
    }
    public void RemoveItemAmount(int amount)
    {
        ItemAmount -= amount;
        UpdateItemAmountText();
        if (!CheckOccupencyOfSlot()) RemoveItem();
    }
    public void AddItem(ItemSO item, int amount)
    {
        slotAmountText.enabled = true;
        IsOccupied = true;
        ItemInSlot = item;
        slotImage.sprite = item.Icon;
        ItemAmount += amount;
        UpdateItemAmountText();
    }
    public void RemoveItem()
    {
        slotAmountText.enabled = false;
        IsOccupied = false;
        ItemInSlot = null;
        slotImage.sprite = null;
        ItemAmount = 0;
        slotAmountText.text = null;
    }
    void UpdateItemAmountText()
    {
        slotAmountText.text = ItemAmount.ToString();
    }
    bool CheckOccupencyOfSlot()
    {
        if (ItemAmount <= ItemInSlot.MaxStackAmount)
        {
            return true;
        }
        return false;
    }

}
