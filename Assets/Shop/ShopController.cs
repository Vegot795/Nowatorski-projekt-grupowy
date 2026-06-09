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
        //TODO:
        //RefreshShopDisplay
        //RefreshPlayerInventoryDisplay
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
    }

}
