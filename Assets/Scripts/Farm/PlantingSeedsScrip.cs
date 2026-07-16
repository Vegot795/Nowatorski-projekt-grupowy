using UnityEngine;

public class PlantingSeedsScript : MonoBehaviour
{
    public SeedSO Seed;
    public SeedSO[] Seeds;

    private GameObject seedPreviewInstance;
    private Grid grid;

    void Start()
    {
        EnsureGrid();
    }

    public void PlantHeldSeeds(InventorySlot currentHeldSlot)
    {
        if (currentHeldSlot == null || !(currentHeldSlot.ItemInSlot is SeedSO seed))
        {
            return;
        }

        GameObject seedParent = FindSeedParent();

        if (!ValidateConditions(currentHeldSlot, seedParent))
        {
            Debug.Log("ValidateConditions() failed in PlantHeldSeeds()");
            return;
        }

        FarmScript farmScript = seedParent.GetComponent<FarmScript>();

        if (farmScript == null || farmScript.isOccupied)
        {
            return;
        }

        Vector3 spawnPosition = GetPotentialSpawnPos();
        Debug.Log($"Planting seed at {spawnPosition}");
        GameObject plantInstance = Instantiate(seed.plantPrefab, spawnPosition, Quaternion.identity, seedParent.transform);
        SpriteRenderer sr = plantInstance.GetComponent<SpriteRenderer>();
        farmScript.isOccupied = true;
        plantInstance.transform.localScale = new Vector3(1f, 1f, 1f);

        if (sr != null)
        {
            sr.sortingLayerName = "Plants";
            sr.sortingOrder = 1;
        }

        currentHeldSlot.RemoveItemAmount(1);
    }

   

    public void HandleSeedPreview(InventorySlot currentHeldSlot, GameObject plantPreview)
    {
        GameObject seedParent = FindSeedParent();

        if (!ValidateConditions(currentHeldSlot, seedParent))
        {
            DestroySeedPrefabPreview();
            return;
        }

        if (currentHeldSlot.ItemInSlot is SeedSO seed)
        {
            if (seedPreviewInstance == null)
            {
                seedPreviewInstance = Instantiate(plantPreview);
                SpriteRenderer sr = seedPreviewInstance.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    sr.sprite = seed.adultStage[1];
                    sr.color = new Color(1f, 1f, 1f, 0.5f);
                    sr.sortingLayerName = "Preview";
                    sr.sortingOrder = 1;
                }

                seedPreviewInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            seedPreviewInstance.transform.position = GetPotentialSpawnPos();
            seedPreviewInstance.transform.rotation = Quaternion.identity;
        }
    }

    public void DestroySeedPrefabPreview()
    {
        if (seedPreviewInstance != null)
        {
            Destroy(seedPreviewInstance);
            seedPreviewInstance = null;
        }
    }

    private bool ValidateConditions(InventorySlot currentHeldSlot, GameObject farmField)
    {
        if (!EnsureGrid() || Camera.main == null)
        {
            return false;
        }

        if (currentHeldSlot == null || currentHeldSlot.ItemInSlot == null)
        {
            return false;
        }

        if (!(currentHeldSlot.ItemInSlot is SeedSO))
        {
            return false;
        }

        if (farmField == null)
        {
            return false;
        }

        FarmScript fs = farmField.GetComponent<FarmScript>();

        if (fs != null && fs.isOccupied)
        {
            Debug.LogWarning("Farm field is already occupied.");
            return false;
        }

        return true;
    }

    private Vector3 GetPotentialSpawnPos()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        return grid.GetCellCenterWorld(cellPosition);
    }

    private GameObject FindSeedParent()
    {
        if (Camera.main == null)
        {
            return null;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.CompareTag("FarmField"))
            {
                return hit.gameObject;
            }
        }

        return null;
    }

    private bool EnsureGrid()
    {
        if (grid != null)
        {
            return true;
        }

        GameObject farmGrid = GameObject.FindWithTag("FarmGrid");
        grid = farmGrid != null ? farmGrid.GetComponent<Grid>() : null;

        return grid != null;
    }
}
