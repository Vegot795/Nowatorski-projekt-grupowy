using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public GameObject FieldBuilder;
    public PlantationScript _plantation;
    public bool isBuildingEnabled = false;

    private void Start()
    {
        _plantation = Object.FindFirstObjectByType<PlantationScript>();
    }

    private void OnBuild()
    {
        if (!isBuildingEnabled)
        {
            FieldBuilder.SetActive(true);
            isBuildingEnabled = true;
            _plantation.inBuildMenu = true;
        }
        else
        {
            FieldBuilder.SetActive(false);
            isBuildingEnabled = false;
            _plantation.inBuildMenu = false;
        }
    }


}
