using TMPro;
using UnityEngine;

public class ShopSlotScript : MonoBehaviour
{
    public GameObject currentItem;
    public int itemPrice;
    public TMP_Text priceText;
    public bool isShopSlot = true;

    private void Awake()
    {
        if (!priceText)
        {
            priceText = transform.Find("TextPrice").GetComponent<TMP_Text>();
        }
    }

    public void UpdatedPriceDisplay()
    {
        if(priceText && currentItem)
        {
            priceText.text = itemPrice.ToString();
        }
    }

    public void SetItem(GameObject item, int price)
    {
        currentItem = item;
        itemPrice = price;
        UpdatedPriceDisplay();
    }

}
