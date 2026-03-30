using Unity.VisualScripting;
using UnityEngine;

public class PlantationScript : MonoBehaviour
{
    public int level = 1;
    public int maxFieldCount;
    public FarmScript[] _fields;
    public FarmScript fieldPrefab;
    public bool inBuildMenu = false;

    private Grid grid;
    private bool canBuild = false;


    void Start()
    {
        _fields = GetComponentsInChildren<FarmScript>();
        maxFieldCount = 4 + level * 2; 
        grid = Object.FindFirstObjectByType<Grid>();
    }

    private void Update()
    {
        if (_fields.Length < maxFieldCount)
        {
            canBuild = true;
        }

    }

    public void UpgradePlantation()
    {
        level++;

    }

    private void BuildNewField()
    {
        if (canBuild)
        {
            Vector3 mousePosition = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector3Int cellPosition = grid.WorldToCell(mousePosition);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);

            Instantiate(fieldPrefab, spawnPosition, Quaternion.identity, transform);
            _fields = GetComponentsInChildren<FarmScript>();
        }
    }

    
}
