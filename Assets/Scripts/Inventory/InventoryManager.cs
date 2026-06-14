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

    private PlantingSeedsScript plantingSeedsScript;

    public void Start()
    {
        uicontroller = GetComponent<UI_Controller>();
        plantingSeedsScript = GetComponent<PlantingSeedsScript>();

        if (plantingSeedsScript == null)
        {
            plantingSeedsScript = gameObject.AddComponent<PlantingSeedsScript>();
        }

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
        plantingSeedsScript.PlantHeldSeeds(currentHeldSlot);
    }

    public void HandleSeedPreview()
    {
        plantingSeedsScript.HandleSeedPreview(currentHeldSlot, plantPreview);
    }

    public void DestroySeedPrefabPreview()
    {
        plantingSeedsScript.DestroySeedPrefabPreview();
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
