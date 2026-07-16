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

    }

    public void WaterPlant()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
        Debug.Log($"Hits: {hits.Length}");

        foreach (Collider2D hit in hits)
        {
            FarmScript field = hit.GetComponentInParent<FarmScript>();
            Debug.Log($"Hit: {hit.gameObject.name}, Field: {field?.gameObject.name}");

            if (field != null && collisions.Contains(field.gameObject))
            {
                if (money == null || money.currentWater < 10) return;

                field.WaterTheField();
                money.currentWater -= 10;
                Debug.Log($"Watered field: {field.gameObject.name}, Remaining water: {money.currentWater}");
                return;
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