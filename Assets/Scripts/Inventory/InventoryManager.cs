using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<InventorySlot> inventorySlots;
    public List<InventorySlot> toolbarSlots;
    public List<InventorySlot> allSlots;
    [SerializeField] private List<ItemSO> allItems;
    public InventorySlot currentHeldSlot;
    public UI_Controller uicontroller;
    public GameObject itemPickupPrefab;
    public GameObject plantPreview;


    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject ToolbarUI;

    public ItemSO testItem;
    public Color baseColor = new Color(255, 255, 255, 100);
    public bool isSeed = false;

    List<string> itemTypes = new List<string> { "Seed", "Item" };

    private bool isInvOpen = true;

    private Grid grid;
    private GameObject seedPreviewInstance;
    private Vector3 spawnPos;

    public void Start()
    {
        uicontroller = GetComponent<UI_Controller>();
        grid = GameObject.FindWithTag("FarmGrid").GetComponent<Grid>();
        if (inventorySlots == null)
        {
            inventoryUI = GameObject.Find("InventoryUI");
        }

        if (toolbarSlots == null)
        {
            ToolbarUI = GameObject.Find("ToolbarUI");
        }


        foreach (var toolbarSlot in ToolbarUI.GetComponentsInChildren<InventorySlot>())
        {
            toolbarSlots.Add(toolbarSlot);
            toolbarSlots.OrderBy(x => x.name);
        }
        currentHeldSlot = toolbarSlots[0];
        SlotSetToBeCurrentHeld(currentHeldSlot);

        foreach (var inventorySlot in inventoryUI.GetComponentsInChildren<InventorySlot>())
        {
            inventorySlots.Add(inventorySlot);
            inventorySlots.OrderBy(x => x.name);
        }

        allSlots = toolbarSlots.Concat(inventorySlots).ToList();


        inventoryUI.SetActive(false);
        isInvOpen = false;

    }

    void Update()
    {
        spawnPos = GetPotentialSpawnPos();

        /*if (Input.GetKeyDown(KeyCode.O))
        {
            addItemToInv(testItem, 30);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            removeItemFromInv(3);
        }*/

        if (!uicontroller.isBuildingEnabled)
        {
            HandleSeedPreview();
        }
        else
        {
            DestroySeedPrefabPreview();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            addItemToInv(testItem, 30);
        }
    }

    #region // -------------------------------- Item Management Code -------------------------------
    void removeItemFromInv(int slotIndex, int amount)
    {

        Debug.Log("Slot to remove found");
        allSlots[slotIndex].RemoveItemAmount(amount);

    }
    public void addItemToInv(ItemSO item, int amount)
    {
        List<InventorySlot> thisItemSlots = allSlots.FindAll(x => x.ItemInSlot == item);
        if (thisItemSlots.Count > 0)
        {
            Debug.Log("Pl have slots with this item in inv");
            int newAmount = amount;

            for (int i = 0; i < thisItemSlots.Count; i++)
            {
                int difference = thisItemSlots[i].ItemInSlot.MaxStackAmount - thisItemSlots[i].ItemAmount;
                int toAdd = Mathf.Min(difference, newAmount);

                thisItemSlots[i].AddItemAmount(toAdd);

                newAmount -= toAdd;
                if (newAmount == 0) break;
            }

            if (newAmount > 0 && NextEmptySlot() != -1)
            {
                Debug.Log("Pl have slots with this item in inv, but all taken");
                inventorySlots[NextEmptySlot()].AddItem(item, newAmount);
            }
        }
        else
        {
            if (NextEmptySlot() != -1)
            {
                Debug.Log("Pl doesn't have slots with this item in inv");
                inventorySlots[NextEmptySlot()].AddItem(item, amount);
            }
        }
    }

    int NextEmptySlot()
    {
        if (inventorySlots == null || inventorySlots.Count == 0)
        {
            Debug.Log("not found slots ");
            return -1; //there is no slot with this pos, this means no slots in list
        }
        else
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsOccupied == false)
                {
                    Debug.Log("found next empty slot: " + i);
                    return i;
                }
            }
            Debug.Log("not found next empty slot, inv is full ");
            return -1; //there is no slot with this pos, this means inv is full

        }


    }
    public void ToggleInventory()
    {
        Debug.Log("inv visibility change");
        isInvOpen = !isInvOpen;
        inventoryUI.SetActive(isInvOpen);
    }

    private void SlotSetToBeCurrentHeld(InventorySlot toolbarSlot)
    {
        Image thisImage = currentHeldSlot.GetComponentInChildren<Image>();
        int currentIndex = toolbarSlots.IndexOf(toolbarSlot);

        thisImage.color = Color.red;
        currentHeldSlot = toolbarSlots[currentIndex];
        currentHeldSlot.isCurrentHeldSlot = true;
    }

    private void SlotSetToBeFree(InventorySlot toolbarSlot)
    {
        Image thisImage = currentHeldSlot.GetComponent<Image>();
        thisImage.color = baseColor;
        toolbarSlot.isCurrentHeldSlot = false;
    }

    public void MoveCurrentSlot(int direction)
    {
        int currentIndex = toolbarSlots.IndexOf(currentHeldSlot);
        int newIndex = (currentIndex + direction) % toolbarSlots.Count;
        if (newIndex < 0) newIndex += toolbarSlots.Count;
        SlotSetToBeFree(currentHeldSlot);
        currentHeldSlot = toolbarSlots[newIndex];
        SlotSetToBeCurrentHeld(currentHeldSlot);
    }

    public void ThrowOutOfEquipment(ItemSO item, int amount)
    {
        if(currentHeldSlot == null || currentHeldSlot.ItemInSlot != item)
        {
            Debug.LogWarning("Current held slot is null or does not contain the specified item.");
            return;
        }

        GameObject PickupItem = Instantiate(itemPickupPrefab, gameObject.transform.position, Quaternion.identity);
        PickupItem.GetComponent<Collider2D>().enabled = false;
        ItemPickup itemPickup = PickupItem.GetComponent<ItemPickup>();
        PickupItem.GetComponentInChildren<SpriteRenderer>().sprite = item.Icon;
        itemPickup.item = item;
        itemPickup.count = amount;

        Rigidbody2D rb = PickupItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 throwDirection = gameObject.GetComponent<PlayerController>().facingDirection switch
            {
                PlayerController.Direction.Top => Vector2.up,
                PlayerController.Direction.Bottom => Vector2.down,
                PlayerController.Direction.Left => Vector2.left,
                PlayerController.Direction.Right => Vector2.right,
                _ => Vector2.zero
            };
            float throwForce = 2f;
            rb.linearVelocity = throwDirection * throwForce;
            StartCoroutine(StopThrownItem(PickupItem, 1f));
        }
        int slotIndex = allSlots.IndexOf(currentHeldSlot);
        if (slotIndex >= 0)
        {
            removeItemFromInv(slotIndex, amount);
            Debug.Log($"Threw out {amount} of {item.name} from slot {slotIndex}");
        }
        else
        {
            Debug.LogWarning($"Current held slot {slotIndex} not found in inventory slots.");

        }
    }

    private IEnumerator StopThrownItem(GameObject IP, float stopAfterSeconds)
    {
        Rigidbody2D rb = IP.GetComponent<Rigidbody2D>();
        Collider2D col = IP.GetComponent<Collider2D>();
        yield return new WaitForSeconds(stopAfterSeconds);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (col != null)
        {
            col.enabled = true;
        }
    }

    #endregion

    #region // -------------------------------- Seed Plant/Preview Code -------------------------------
    public void PlantHeldSeeds()
    {
        if (currentHeldSlot.ItemInSlot is SeedSO seed)
        {
            spawnPos = GetPotentialSpawnPos();

            if (ValidateConditions())
            {
                if (FindSeedParent().GetComponent<FarmScript>().isOccupied == false)
                {
                    Debug.Log($"Planting seed at {spawnPos}");
                    GameObject plantInstance = Instantiate(seed.plantPrefab, spawnPos, Quaternion.identity, transform);
                    SpriteRenderer sr = plantInstance.GetComponent<SpriteRenderer>();
                    if (FindSeedParent() != null)
                    {
                        plantInstance.transform.SetParent(FindSeedParent().transform);
                        FindSeedParent().GetComponent<FarmScript>().isOccupied = true;
                    }
                    plantInstance.transform.localScale = new Vector3(1f, 1f, 1f);
                    sr.sortingLayerName = "Plants";
                    sr.sortingOrder = 1;

                    currentHeldSlot.RemoveItemAmount(1);
                }

            }
            else
            {
                Debug.Log("ValidateConditions() failed in PlantHeldSeeds()");
            }
        }

    }
    public void HandleSeedPreview()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (ValidateConditions())
        {
            if (currentHeldSlot.ItemInSlot is SeedSO seed && currentHeldSlot != null)
            {
                if (seedPreviewInstance == null)
                {
                    seedPreviewInstance = Instantiate(plantPreview);
                    seedPreviewInstance.GetComponent<SpriteRenderer>().sprite = seed.adultStage[1];
                    seedPreviewInstance.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 50);
                    seedPreviewInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Preview";
                    seedPreviewInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    seedPreviewInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }

                spawnPos = GetPotentialSpawnPos();

                seedPreviewInstance.transform.position = spawnPos;
                seedPreviewInstance.transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            DestroySeedPrefabPreview();
        }
    }

    private bool ValidateConditions()
    {
        if (uicontroller == null)
        {
            //Debug.LogWarning("UI Controller reference is null.");
            return false;
        }

        if (currentHeldSlot == null || currentHeldSlot.ItemInSlot == null)
        {
            //Debug.LogWarning("No item is currently held.");
            return false;
        }

        if (!(currentHeldSlot.ItemInSlot is SeedSO seed))
        {
            //Debug.LogWarning("Currently held item is not a seed.");
            return false;
        }

        var (isOverFarmField, farmField) = IsObjectUnderCursorAFarmField();
        //Debug.Log($"Is over farm field: {isOverFarmField}, Farm field object: {farmField?.name ?? "None"}");

        if (!isOverFarmField)
        {
            //Debug.LogWarning("Not over a farm field.");
            return false;
        }

        if (farmField != null)
        {
            FarmScript fs = farmField.GetComponent<FarmScript>();
            if (fs != null && fs.isOccupied)
            {
                Debug.LogWarning("Farm field is already occupied.");
                return false;
            }
        }
        return true;
    }

    private (bool, GameObject) IsObjectUnderCursorAFarmField()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        //Debug.Log($"OverlapPoint hit: {hit?.name ?? "None"} at position {worldPosition}");

        if (hit != null && hit.CompareTag("FarmField"))
        {
            return (true, hit.gameObject);
        }
        return (false, null);
    }


    public void DestroySeedPrefabPreview()
    {
        if (seedPreviewInstance != null)
        {
            Destroy(seedPreviewInstance);
            seedPreviewInstance = null;
        }
    }

    #endregion

    #region // -------------------------------- Helpers -----------------------------------------------
    public Vector3 GetPotentialSpawnPos()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
        //spawnPosition.y += 0.2f;
        return spawnPosition;
    }
    private GameObject FindSeedParent()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit != null && hit.CompareTag("FarmField"))
        {
            return hit.gameObject;
        }

        return null;
    }

    #endregion
    //save
    public void SaveInventory()
    {
        SaveSystem.SaveInventory(allSlots);
    }
    public void LoadInventory()
    {
        InventorySaveData data = SaveSystem.LoadInventory();

        if (data == null)
            return;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = allSlots[i];

            slot.RemoveItem();

            if (data.slots[i].DataIsOccupied)
            {
                ItemSO item = FindItemByName(data.slots[i].DataItemName);

                slot.AddItem(
                    item,
                    data.slots[i].DataItemAmount
                );
            }
        }
    }
    private ItemSO FindItemByName(string itemName)
    {
        foreach (ItemSO item in allItems)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }

        return null;
    }
}
