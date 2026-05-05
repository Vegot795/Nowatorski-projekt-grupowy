using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [field: SerializeField] public bool IsOccupied { get; private set; }
    [field: SerializeField] public ItemSO ItemInSlot { get; private set; }
    [field: SerializeField] public int ItemAmount { get; private set; }
    [SerializeField] private Image slotImage;
    private DragDrop imageDrag;
    [SerializeField] private TextMeshProUGUI slotAmountText;
    void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        slotAmountText = GetComponentInChildren<TextMeshProUGUI>();
        IsOccupied = false;
        UpdateItemAmountText();
        slotAmountText.enabled = false;
        imageDrag = slotImage.GetComponent<DragDrop>();
        StartCoroutine(SetDrag(false));
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
        StartCoroutine(SetDrag(true));
    }
    public void RemoveItem()
    {
        slotAmountText.enabled = false;
        IsOccupied = false;
        ItemInSlot = null;
        slotImage.sprite = null;
        ItemAmount = 0;
        slotAmountText.text = null;
        StartCoroutine(SetDrag(false));

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
    IEnumerator SetDrag(bool state)
    {

        yield return new WaitForSeconds(0.1f);
        imageDrag.enabled = state;
    }
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponentInParent<InventorySlot>();
        //Debug.Log("OnDrop");
        if (eventData.pointerDrag != null && IsOccupied == false)
        {
            AddItem(draggedSlot.ItemInSlot, draggedSlot.ItemAmount);
            draggedSlot.RemoveItem();

        }
        if (eventData.pointerDrag != null && IsOccupied == true && eventData.pointerDrag.GetComponentInParent<InventorySlot>().ItemInSlot == ItemInSlot)
        {
            int difference = ItemInSlot.MaxStackAmount - ItemAmount;
            int toAdd = Mathf.Min(difference, draggedSlot.ItemAmount);

            AddItemAmount(toAdd);
            draggedSlot.RemoveItemAmount(toAdd);

            if (draggedSlot.ItemAmount <= 0)
            {
                draggedSlot.RemoveItem();
            }


        }

    }
}
