using UnityEngine;

public class shopSeedButtons : MonoBehaviour
{
    [SerializeField] private ItemSO shopItem;
    [SerializeField] private int price;
    private Money money;
    private InventoryManager im;
    private PlayerController contr;
    private CharacterBasics bas;
    private PlantationScript plantS;
    void Start()
    {
        money = FindAnyObjectByType<Money>();
        im = FindAnyObjectByType<InventoryManager>();
        contr = FindAnyObjectByType<PlayerController>();
        bas = FindAnyObjectByType<CharacterBasics>();
        plantS = FindAnyObjectByType<PlantationScript>();
    }
    bool CanPay => money.currentMoney >= price;
    public void BuySeeds()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            im.addItemToInv(shopItem, 1);
        }
    }
    public void BuyBetterSword()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            contr.Damage += 1;
        }
    }
    public void BuyRegHP()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            bas.CurrentHP = Mathf.Min(bas.CurrentHP + 10, bas.MaxHP);
        }
    }
    public void BuyMoreHP()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            bas.MaxHP += 10;
        }
    }
    public void BuyMoreField()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            plantS.maxFieldCount += 1;
        }
    }
    public void BuyWater()
    {
        if (CanPay)
        {
            money.currentMoney -= price;
            money.currentWater += 10;
        }
    }


}
