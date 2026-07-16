using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlantationScript : MonoBehaviour
{
    public static PlantationScript instance;
    public int level = 1;
    public int maxFieldCount;
    public FarmScript[] _fields;
    public GameObject fieldPrefabGO;

    public FarmScript fieldPrefab;
    public bool inBuildMenu = false;
    public List<string> BuildingModeList = new List<string> { "Build", "Destroy" };
    public string BuildingMode;
    public FarmScript FieldPrefabInstance { get; private set; }
    public GameObject destroyIndicatorPrefab;

    private Grid grid;
    private bool canBuild = false;
    private GameObject destroyPreviewInstance;
    private GameObject fieldPreviewInstance;
    private PlayerController _playerController;
    private GameObject hitObject = null;


    [SerializeField] private SeedSO[] seeds;

    void Start()
    {
        _fields = GetComponentsInChildren<FarmScript>();
        fieldPrefab = fieldPrefabGO.GetComponent<FarmScript>();
        maxFieldCount = 2 + (level * 2);
        grid = GameObject.FindWithTag("FarmGrid").GetComponent<Grid>();

    }

    private void Update()
    {
        if (_fields.Length < maxFieldCount)
        {
            canBuild = true;
        }

        if (BuildingMode == "Build")
        {
            HandleFieldPreview();
        }
        else if (BuildingMode == "Destroy")
        {
            ShowDestroyIndicator();
            DestroyFieldPrefabPreview();
        }
    }

    #region // ------------- BUILDING FIELDS -------------------
    public void HandleFieldPreview()
    {
        if (BuildingMode == "Build" && inBuildMenu && canBuild)
        {
            if (fieldPreviewInstance == null)
            {
                fieldPreviewInstance = Instantiate(fieldPrefab.gameObject);
                SetLayerRecursively(fieldPreviewInstance, LayerMask.NameToLayer("Preview"));
                SetPreviewMaterial(fieldPreviewInstance, 0.5f);
                fieldPreviewInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Preview";
                fieldPreviewInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);

            fieldPreviewInstance.transform.position = spawnPosition;
            fieldPreviewInstance.transform.rotation = Quaternion.identity;
        }
        else
        {
            DestroyFieldPrefabPreview();
        }
    }

    public void BuildNewField()
    {
        if (BuildingMode == "Build" && canBuild && inBuildMenu)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);

            if (ValidateConditions())
            {
                FieldPrefabInstance = Instantiate(fieldPrefab, spawnPosition, Quaternion.identity, transform);
                FieldPrefabInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
                FieldPrefabInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                _fields = GetComponentsInChildren<FarmScript>();
            }
        }
    }
    private bool ValidateConditions()
    {
        if (_fields.Length >= maxFieldCount)
        {
            return false;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);

        if (IsFieldAtCell(cellPosition))
        {
            return false;
        }

        int groundLayer = LayerMask.NameToLayer("Ground");
        int previewLayer = LayerMask.NameToLayer("Preview");
        Collider2D[] colliders = Physics2D.OverlapPointAll(spawnPosition);

        foreach (var col in colliders)
        {
            if (col.gameObject.layer == previewLayer)
                continue;
            if (col.gameObject.layer != groundLayer)
            {
                Debug.Log("Cannot build here, object in the way: " + col.gameObject.name);
                return false;
            }
        }

        return true;
    }

    private bool IsFieldAtCell(Vector3Int cellPosition)
    {
        Vector3 cellWorldPos = grid.GetCellCenterWorld(cellPosition);
        foreach (var field in _fields)
        {
            if (field == null) continue;
            if (field.transform.position == cellWorldPos)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region//---------------- DESTROY FIELDS -----------------

    private (bool, GameObject) IsObjectUnderCursorAFarmField()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        Debug.Log($"OverlapPoint hit: {hit?.name ?? "None"} at position {worldPosition}");

        if (hit != null && hit.CompareTag("FarmField"))
        {
            return (true, hit.gameObject);
        }
        return (false, null);
    }

    private void ShowDestroyIndicator()
    {
        if (BuildingMode == "Destroy" && hitObject != null && inBuildMenu)
        {
            if (destroyPreviewInstance == null)
            {
                destroyPreviewInstance = Instantiate(destroyIndicatorPrefab);
                destroyPreviewInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
                destroyPreviewInstance.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }

            GameObject hitObject = IsObjectUnderCursorAFarmField().Item2;
            Vector3 SpawnPos = hitObject.transform.position;

            destroyPreviewInstance.transform.position = SpawnPos;
            destroyPreviewInstance.transform.rotation = Quaternion.identity;
        }
        else
        {
            if (destroyPreviewInstance != null)
            {
                Destroy(destroyPreviewInstance);
            }
        }
    }

    public void RemoveField()
    {
        (bool isFieldUnderCursor, GameObject field) = IsObjectUnderCursorAFarmField();

        if (isFieldUnderCursor && field != null)
        {
            FarmScript fieldToRemove = _fields.FirstOrDefault(f => f.gameObject == field);

            if (fieldToRemove != null)
            {
                _fields = _fields
                    .Where(f => f != fieldToRemove)
                    .ToArray();

                Destroy(fieldToRemove.gameObject);
            }
        }
    }
    #endregion

    #region// ---------------- SAVE/LOAD --------------

    public void SaveFarm()
    {
        _fields = GetComponentsInChildren<FarmScript>();
        SaveSystem.SaveFarm(_fields.ToList());
    }

    public void LoadFarm()
    {
        FarmSaveDataList dataList = SaveSystem.LoadFarm();

        if (dataList == null)
        {
            return;
        }

        foreach (FarmScript field in _fields)
        {
            if (field != null)
            {
                Destroy(field.gameObject);
            }
        }

        foreach (FarmSaveData farmData in dataList.farms)
        {
            LoadField(farmData);
        }

        _fields = GetComponentsInChildren<FarmScript>();
    }

    public void LoadField(FarmSaveData data)
    {
        Vector3 position = new Vector3(
            data.DataFieldPosition[0],
            data.DataFieldPosition[1],
            data.DataFieldPosition[2]
        );

        FarmScript newField = Instantiate(fieldPrefab, position, Quaternion.identity, transform);

        SpriteRenderer spriteRenderer = newField.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Ground";
            spriteRenderer.sortingOrder = 1;
        }

        newField.isWatered = data.DataIsWatered;
        newField.isOccupied = data.DataIsOccupied;
        newField.waterTimer = data.DataWaterTimer;

        if (data.DataPlant != null)
        {
            LoadPlant(data.DataPlant, newField);
        }
    }

    public void LoadPlant(PlantSaveData saveData, FarmScript farmField)
    {
        SeedSO seed = FindSeedByName(saveData.DataSeedName);

        if (seed == null)
        {
            Debug.LogError($"Could not load plant. Seed not found: {saveData.DataSeedName}");
            return;
        }

        GameObject loadedPlantInstance = Instantiate(
            seed.plantPrefab,
            farmField.transform.position,
            Quaternion.identity,
            farmField.transform
        );

        PlantScript plantScript = loadedPlantInstance.GetComponent<PlantScript>();

        if (plantScript == null)
        {
            Debug.LogError($"Loaded plant prefab does not have PlantScript: {seed.plantPrefab.name}");
            return;
        }

        plantScript.RestoreGrowthState(
            saveData.DataCurrentGrowthStage,
            saveData.DataCurrentGrowth,
            saveData.DataIsHarvestable
        );

        farmField.isOccupied = true;
    }

    private SeedSO FindSeedByName(string seedName)
    {
        foreach (SeedSO seed in seeds)
        {
            if (seed != null && seed.name == seedName)
            {
                return seed;
            }
        }

        return null;
    }

    #endregion

    public void DestroyFieldPrefabPreview()
    {
        if (fieldPreviewInstance != null)
        {
            Destroy(fieldPreviewInstance);
            fieldPreviewInstance = null;
        }
    }

    public void UpgradePlantation()
    {
        level++;
    }

    private void SetPreviewMaterial(GameObject previewObj, float alpha)
    {
        var renderers = previewObj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
