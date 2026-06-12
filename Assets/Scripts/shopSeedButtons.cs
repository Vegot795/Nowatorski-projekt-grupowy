using UnityEngine;

public class shopSeedButtons : MonoBehaviour
{
    [SerializeField] private ItemSO shopItem;
    [SerializeField] private int price;
    private Money money;
    private InventoryManager im;
    void Start()
    {
        money = FindAnyObjectByType<Money>();
        im = FindAnyObjectByType<InventoryManager>();
    }
    bool CanPay => money.currentMoney >= price;
    public void Pay()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            im.addItemToInv(shopItem, 1);
        }
    }

}
