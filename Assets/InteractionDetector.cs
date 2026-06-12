using TMPro;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Header("UI")]
    //public TextMeshProUGUI interactionText;

    [Header("References")]
    [SerializeField] private ShopController shopController;

    [SerializeField] private GameObject currentPlant;

    [SerializeField] private bool inShop = false;
    [SerializeField] private bool inPlant = false;

    void Start()
    {
        //interactionText.gameObject.SetActive(false);
        shopController = FindAnyObjectByType<ShopController>();
    }

    void Update()
    {
        HandleShopInput();
        HandlePlantInput();
    }

    // ---------------- SHOP ----------------
    private void HandleShopInput()
    {
        if (!inShop) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (shopController.isShopOpen)
                shopController.CloseShop();
            else
                shopController.OpenShop();
        }
    }

    // ---------------- PLANT ----------------
    private void HandlePlantInput()
    {
        if (!inPlant) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentPlant == null) return;

            PlantScript plant = currentPlant.GetComponent<PlantScript>();

            if (plant != null && plant.isHarvestable)
            {
                plant.HarvestPlant();
            }
        }
    }

    // ---------------- TRIGGERS ----------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shop"))
        {
            inShop = true;
            //ShowText("Press T to open shop");
        }

        if (collision.CompareTag("Plant"))
        {
            PlantScript plant = collision.GetComponent<PlantScript>();

            if (plant != null && plant.isHarvestable)
            {
                currentPlant = collision.gameObject;
                inPlant = true;

                //ShowText("Press P to harvest");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Shop"))
        {
            inShop = false;
        }

        if (collision.CompareTag("Plant"))
        {
            inPlant = false;
            currentPlant = null;
        }

        if (!inShop && !inPlant)
        {
            //HideText();
        }
    }

    // ---------------- UI HELPERS ----------------
    /*private void ShowText(string message)
    {
        interactionText.gameObject.SetActive(true);
        interactionText.text = message;
    }

    private void HideText()
    {
        interactionText.gameObject.SetActive(false);
    }*/
}