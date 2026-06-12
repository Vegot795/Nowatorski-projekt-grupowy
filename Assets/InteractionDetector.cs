using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteraction interactableInRange = null;
    public GameObject interactionText;
    private bool canInteract = false;
    private ShopController shopController;
    void Start()
    {
        interactionText.SetActive(false);
        shopController = FindAnyObjectByType<ShopController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) & canInteract)
        {
            Debug.Log("t + int");
            if (shopController.isShopOpen)
            {
                shopController.CloseShop();
            }
            else
            {
                shopController.OpenShop();
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shop"))
        {
            //Debug.Log("kolizja");
            canInteract = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Shop"))
        {
            //Debug.Log("end kolizja");
            canInteract = false;
        }
    }
}
