using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public GameObject FieldBuilder;
    public PlantationScript _plantation;
    public bool isBuildingEnabled = false;
    public GameObject _uiController;

    private void Start()
    {
        _plantation = Object.FindFirstObjectByType<PlantationScript>();
        _uiController = GameObject.Find("UI");
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
}
