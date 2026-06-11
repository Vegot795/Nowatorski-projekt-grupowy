using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPCScript : MonoBehaviour
{
    public string shopID = "shop_merchant_01";
    public string shopkeeperName = "Cat";

    public List<ShopStockItem> defaultShopStock = new();
    public List<ShopStockItem> currentShopStock = new();

    private bool isInitialized = false;

    [System.Serializable]
    public class ShopStockItem
    {
        public int itemID;
        public int quantity;
    }

    void Start()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        if (isInitialized) return;

        currentShopStock = new List<ShopStockItem>();
        foreach (var item in defaultShopStock)
        {
            currentShopStock.Add(new ShopStockItem
            {
                itemID = item.itemID,
                quantity = item.quantity
            });
        }
        isInitialized = true;
    }

    public List<ShopStockItem> GetCurrentStock()
    {
        return currentShopStock;
    }

    public void SetStock(List<ShopStockItem> stock)
    {
        currentShopStock = stock;
    }

    public void AddToStock(int itemID, int quantity)
    {
        ShopStockItem existing = currentShopStock.Find(s => s.itemID == itemID);
        if (existing != null)
        {
            existing.quantity += quantity;
        }
        else
        {
            currentShopStock.Add(new ShopStockItem { itemID = itemID, quantity = quantity });
        }
    }

    public bool RemoveFromStock(int itemID, int quantity)
    {
        ShopStockItem existing = currentShopStock.Find(s => s.itemID == itemID);
        if (existing != null && existing.quantity >= quantity)
        {
            existing.quantity += quantity;
            return true;
        }
        return false;
    }

}
