using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopInventoryGrid, playerInventoryGrid;
    public GameObject shopSlotPrefab;
    public TMP_Text playerMoneyText;

    private ShopNPCScript currentShop;
    [SerializeField] private InventoryManager inventoryManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        shopPanel.SetActive(false);
        if (CurrencyController.Instance != null)
        {
            CurrencyController.Instance.OnGoldChanged += UpdateMoneyDisplay;
            UpdateMoneyDisplay(CurrencyController.Instance.GetGold());
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if (playerMoneyText != null)
            playerMoneyText.text = amount.ToString();
    }

    public void OpenShop(ShopNPCScript shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        RefreshShopDisplay();
        RefreshPlayerInventoryDisplay();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
    }

    public void RefreshShopDisplay()
    {
        if (currentShop == null)
        {
            return;
        }

        foreach (Transform child in shopInventoryGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (var stockItem in currentShop.GetCurrentStock())
        {
            if (stockItem.quantity <= 0) continue;
            GameObject slot = Instantiate(shopSlotPrefab, shopInventoryGrid);

            TMP_Text text = slot.GetComponentInChildren<TMP_Text>();

            text.text = stockItem.itemID + " x" + stockItem.quantity;
        }

    }

    public void RefreshPlayerInventoryDisplay()
    {
        if (inventoryManager == null)
        {
            return;
        }

        foreach (Transform child in playerInventoryGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot inventorySlot in inventoryManager.allSlots)
        {
            if (!inventorySlot.IsOccupied || inventorySlot.ItemInSlot == null)
            {
                GameObject slot = Instantiate(shopSlotPrefab, playerInventoryGrid);

                TMP_Text text = slot.GetComponentInChildren<TMP_Text>();

                ItemSO item = inventorySlot.ItemInSlot;
                int amount = inventorySlot.ItemAmount;

                text.text = item.name + " x" + amount;
            }
        }
    }

}


