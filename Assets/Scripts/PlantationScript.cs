using Unity.VisualScripting;
using UnityEngine;

public class PlantationScript : MonoBehaviour
{
    public int level = 1;
    public int maxFieldCount;
    public FarmScript[] _fields;
    public FarmScript fieldPrefab;
    public bool inBuildMenu = false;
    public FarmScript FieldPrefabInstance { get; private set; }

    private Grid grid;
    private bool canBuild = false;
    private GameObject fieldPreviewInstance;
    private PlayerController _playerController;


    void Start()
    {
        _fields = GetComponentsInChildren<FarmScript>();
        maxFieldCount = 4 + level * 2; 
        grid = GameObject.FindWithTag("FarmGrid").GetComponent<Grid>(); 
    }

    private void Update()
    {
        if (_fields.Length < maxFieldCount)
        {
            canBuild = true;
        }

        HandleFieldPreview();
    }

    public void HandleFieldPreview()
    {
        if (inBuildMenu && canBuild)
        {
            if (fieldPreviewInstance == null)
            {
                fieldPreviewInstance = Instantiate(fieldPrefab.gameObject);
                SetPreviewMaterial(fieldPreviewInstance, 0.5f);
                fieldPreviewInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
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
            if(fieldPreviewInstance != null)
            {
                Destroy(fieldPreviewInstance);
                fieldPreviewInstance = null;
            }
        }
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

    public void UpgradePlantation()
    {
        level++;
    }

    public void BuildNewField()
    {
        if (canBuild && inBuildMenu)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);

            FieldPrefabInstance = Instantiate(fieldPrefab, spawnPosition, Quaternion.identity, transform);
            FieldPrefabInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
            FieldPrefabInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;
            _fields = GetComponentsInChildren<FarmScript>();          
        }
    }

    public void OnMouseLeftButtonClick()
    {
        BuildNewField();
        Debug.Log("ButtonPressed");
    }

}
