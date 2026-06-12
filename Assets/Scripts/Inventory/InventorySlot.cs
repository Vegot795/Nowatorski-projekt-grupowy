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

    public bool isCurrentHeldSlot = false;
    void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        if (ItemInSlot != null)
        {
            slotImage.sprite = ItemInSlot.Icon;
            slotImage.color = new Color(255, 255, 255, 100);
        }
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

        if (ItemAmount <= 0)
        {
            RemoveItem();
            return;
        }

        if (!CheckOccupencyOfSlot())
        {
            RemoveItem();
        }
    }
    public void AddItem(ItemSO item, int amount)
    {
        slotAmountText.enabled = true;
        IsOccupied = true;
        ItemInSlot = item;
        slotImage.sprite = item.Icon;
        slotImage.color = new Color(255, 255, 255, 100);
        ItemAmount += amount;
        UpdateItemAmountText();
        imageDrag.enabled = true;
    }
    public void RemoveItem()
    {
        slotAmountText.enabled = false;
        IsOccupied = false;
        ItemInSlot = null;
        slotImage.sprite = null;
        slotImage.color = new Color(255, 255, 255, 0);
        ItemAmount = 0;
        slotAmountText.text = null;
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(SetDrag(false));
        }

    }
    void UpdateItemAmountText()
    {
        slotAmountText.text = ItemAmount.ToString();
    }
    bool CheckOccupencyOfSlot()
    {
        if (ItemInSlot == null)
            return false;

        return ItemAmount <= ItemInSlot.MaxStackAmount;
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
