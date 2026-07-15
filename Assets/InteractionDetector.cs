using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class InteractionDetector : MonoBehaviour
{

    [SerializeField] private ShopController shopController;
    [SerializeField] private Money money;

    [SerializeField] private GameObject currentPlant;
    [SerializeField] private GameObject currentField;

    [SerializeField] private bool inShop = false;
    [SerializeField] private bool inPlant = false;
    [SerializeField] private bool inFarmField = false;

    public List<GameObject> collisions = new List<GameObject>();


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
        HandleFieldInput();
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
            if (currentField == null) return;
            if (money == null || money.currentWater < 10) return;

            FarmScript field = currentField.GetComponent<FarmScript>();

            if (field != null)
            {
                field.WaterTheField();
                money.currentWater -= 10;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisions.Add(collision.gameObject);
        Debug.Log($"Collided with {collision.gameObject.name}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collisions.Remove(collision.gameObject);
        Debug.Log($"Exited collision with {collision.gameObject.name}");
        
        if(collision.gameObject.GetComponent<ShopNPCScript>() != null)
        {
            ShopController.Instance.CloseShop();
        }
    }


}