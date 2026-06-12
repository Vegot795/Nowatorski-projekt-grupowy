using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
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
    }

    public void OpenShop(ShopNPCScript shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
    }

}


