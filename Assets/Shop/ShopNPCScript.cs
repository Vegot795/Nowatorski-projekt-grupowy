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

        ShopController.Instance.ToggleShop();
        Debug.Log("Interacted with Shop NPC");
    }
}
