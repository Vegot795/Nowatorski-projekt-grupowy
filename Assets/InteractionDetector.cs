using TMPro;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{

    [SerializeField] private ShopController shopController;
    [SerializeField] private Money money;

    [SerializeField] private GameObject currentPlant;
    [SerializeField] private GameObject currentField;

    [SerializeField] private bool inShop = false;
    [SerializeField] private bool inPlant = false;
    [SerializeField] private bool inFarmField = false;

    void Start()
    {
        //interactionText.gameObject.SetActive(false);
        shopController = FindAnyObjectByType<ShopController>();
        money = FindAnyObjectByType<Money>();
    }

    void Update()
    {
        HandleShopInput();
        HandlePlantInput();
    }

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
    private void HandleFieldInput()
    {
        if (!inFarmField) return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentPlant == null) return;

            FarmScript field = currentField.GetComponent<FarmScript>();

            if (field != null)
            {
                field.WaterTheField();
            }
        }
    }

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
        if (collision.CompareTag("FarmField"))
        {
            currentField = collision.gameObject;
            inShop = true;
            //ShowText("Press T to open shop");
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
        if (collision.CompareTag("FarmField"))
        {

            inShop = false;
            currentField = null;
            //ShowText("Press T to open shop");
        }


    }


}