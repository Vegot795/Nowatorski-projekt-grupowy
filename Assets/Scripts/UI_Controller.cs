using TMPro;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public GameObject FieldBuilder;
    public PlantationScript _plantation;
    public bool isBuildingEnabled = false;
    public GameObject _uiController;
    public TextMeshProUGUI _FieldCount;
    public TextMeshProUGUI _BuildingMode;

    void Start()
    {
        _plantation = Object.FindFirstObjectByType<PlantationScript>();
        _uiController = GameObject.Find("UI");
        if (_FieldCount == null)
        {
            var fieldCount = GameObject.Find("FieldCount");
            if (fieldCount != null)
            {
                _FieldCount = fieldCount.GetComponent<TextMeshProUGUI>();
            }
        }

        if (_BuildingMode == null)
        {
            var buildingMode = GameObject.Find("BuildMode");
            if (buildingMode != null)
            {
                _BuildingMode = buildingMode.GetComponent<TextMeshProUGUI>();
            }
        }
    }


    void Update()
    {
        UpdateFieldCount();
        IndicateBuildingMode();
    }

    public void ToggleBuildMenu()
    {
        if (!isBuildingEnabled)
        {
            EnableBuildMenu();
        }
        else
        {
            DisableBuildMenu();
        }
    }

    private void EnableBuildMenu()
    {
        FieldBuilder.SetActive(true);
        isBuildingEnabled = true;
        _plantation.inBuildMenu = true;
        _uiController.SetActive(true);
        _plantation.HandleFieldPreview();
        _plantation.BuildingMode = _plantation.BuildingModeList[0];
    }

    private void DisableBuildMenu()
    {
        FieldBuilder.SetActive(false);
        isBuildingEnabled = false;
        _plantation.inBuildMenu = false;
        _uiController.SetActive(false);
    }

    private void UpdateFieldCount()
    {
        if (_plantation == null || _FieldCount == null)
            return;

        var FieldCount = _plantation._fields.Length;
        var MaxFieldCount = _plantation.maxFieldCount;

        _FieldCount.text = $"{FieldCount}/{MaxFieldCount}";
    }

    public void OnSave()
    {
        _plantation.Save();
    }

    public void OnLoad()
    {
        _plantation.Load();
    }

    private void IndicateBuildingMode()
    {
        if (_plantation.BuildingMode == "Build")
        {
            _BuildingMode.text = "Build";


        }
        else if (_plantation.BuildingMode == "Destroy")
        {
            _BuildingMode.text = "Destroy";
        }
    }
}
