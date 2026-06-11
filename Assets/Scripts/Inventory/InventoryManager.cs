using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] public List<InventorySlot> inventorySlots;
    [SerializeField] public List<InventorySlot> toolbarSlots;
    [SerializeField] private List<ItemSO> allItems;
    public ItemSO testItem;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject ToolbarUI;
    [SerializeField] public InventorySlot currentHeldSlot;
    private bool isInvOpen = true;
    private Grid grid;
    private GameObject seedPreviewInstance;
    public Color baseColor = new Color(255, 255, 255, 100);
    public bool isSeed = false;
    List<string> itemTypes = new List<string> { "Seed", "Item" };
    private Vector3 spawnPos;

    public void Start()
    {
        grid = GameObject.FindWithTag("FarmGrid").GetComponent<Grid>();
        if (inventorySlots != null)
        {
            inventoryUI = GameObject.Find("InventoryUI");
        }
        inventoryUI.SetActive(false);
        isInvOpen = false;

        if (toolbarSlots != null)
        {
            ToolbarUI = GameObject.Find("ToolbarUI");
        }

        foreach (var toolbarSlot in ToolbarUI.GetComponentsInChildren<InventorySlot>())
        {
            toolbarSlots.Add(toolbarSlot);
            toolbarSlots.OrderBy(x => x.name);
        }
        currentHeldSlot = inventorySlots[0];
        SlotSetToBeCurrentHeld(currentHeldSlot);

        foreach (var inventorySlot in inventoryUI.GetComponentsInChildren<InventorySlot>())
        {
            inventorySlots.Add(inventorySlot);
            inventorySlots.OrderBy(x => x.name);
        }
    }

    void Update()
    {
        Vector3 spawnPos = GetPotentialSpawnPos();

        if (currentHeldSlot.ItemInSlot is SeedSO && ValidateConditions())
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
    void removeItemFromInv(int slotIndex)
    {

        Debug.Log("Slot to remove found");
        //inventorySlots[slotIndex].removeItemSlot.RemoveItem();

    }
    void addItemToInv(ItemSO item, int amount)
    {
        List<InventorySlot> thisItemSlots = inventorySlots.FindAll(x => x.ItemInSlot == item);
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
    #endregion


    public Vector3 GetPotentialSpawnPos()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
        return spawnPosition;
    }

    #region // -------------------------------- Seed Preview Code -------------------------------
    public void PlantHeldSeeds()
    {
        if (currentHeldSlot.ItemInSlot is SeedSO seed)
        {

            if (ValidateConditions())
            {
                GameObject plantInstance = Instantiate(seed.plantPreview, spawnPos, Quaternion.identity, transform);
                plantInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Plants";
                plantInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                currentHeldSlot.RemoveItemAmount(1);
            }
        }

    }
    public void HandleSeedPreview()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (currentHeldSlot?.ItemInSlot is SeedSO seed)
        {
            if (seedPreviewInstance == null)
            {
                seedPreviewInstance = Instantiate(seed.plantPreview);
                seedPreviewInstance.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 50);
                seedPreviewInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Preview";
                seedPreviewInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }

            mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);

            seedPreviewInstance.transform.position = spawnPosition;
            seedPreviewInstance.transform.rotation = Quaternion.identity;
        }
        else
        {
            DestroySeedPrefabPreview();
        }
    }

    private bool ValidateConditions()
    {
        if (currentHeldSlot == null || currentHeldSlot.ItemInSlot == null)
        {
            Debug.LogWarning("No item is currently held.");
            return false;
        }

        if (!(currentHeldSlot.ItemInSlot is SeedSO seed))
        {
            Debug.LogWarning("Currently held item is not a seed.");
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapPointAll(spawnPos);

        foreach (var col in colliders)
        {
            if (col.CompareTag("FarmTile"))
            {
                FarmScript fs = col.GetComponent<FarmScript>();
                if (fs != null & !fs.isOccupied)
                {
                    continue;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
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
    //save
    public void SaveInventory()
    {
        SaveSystem.SaveInventory(inventorySlots);
    }
    public void LoadInventory()
    {
        InventorySaveData data = SaveSystem.LoadInventory();

        if (data == null)
            return;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];

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
