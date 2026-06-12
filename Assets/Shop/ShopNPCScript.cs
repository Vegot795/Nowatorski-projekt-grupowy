using UnityEngine;

public class ShopNPCScript : MonoBehaviour, IInteraction
{
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (ShopController.Instance == null)
        {
            return;
        }

        if (ShopController.Instance.shopPanel.activeSelf)
        {
            ShopController.Instance.CloseShop();
        }
        else
        {
            ShopController.Instance.OpenShop();
        }
    }
}
