using TMPro;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public GameObject FieldBuilder;
    public PlantationScript _plantation;
    public bool isBuildingEnabled = false;
    public GameObject _uiController;
    public TextMeshProUGUI _FieldCount;

    void Start()
    {
        _plantation = Object.FindFirstObjectByType<PlantationScript>();
        _uiController = GameObject.Find("UI");
        _FieldCount = GameObject.Find("FieldCount").GetComponent<TextMeshProUGUI>();
        if (_FieldCount == null)
        {
            Debug.Log("Field Count text not found.");
        }
    }


    void Update()
    {
        UpdateFieldCount(); 
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
        _plantation.inBuildMenu = true;
        _plantation.HandleFieldPreview();
    }

    private void DisableBuildMenu()
    {
        FieldBuilder.SetActive(false);
        isBuildingEnabled = false;
        _plantation.inBuildMenu = false;
        _uiController.SetActive(false);
        _plantation.inBuildMenu = false;
    }

    private void UpdateFieldCount()
    {
        if (_plantation == null || _FieldCount == null)
            return;

        var FieldCount = _plantation._fields.Length;
        var MaxFieldCount = _plantation.maxFieldCount;

        _FieldCount.text = $"{FieldCount}/{MaxFieldCount}";
    }
}
