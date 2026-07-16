using System.Linq;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject FieldBuilder;
    public PlantationScript _plantation;
    public GameObject _uiController;
    public TextMeshProUGUI _FieldCount;
    public TextMeshProUGUI _BuildingMode;
    public GameObject Tooltip;
    public GameObject Deadscreen;
    public GameObject MoneyText;
    public GameObject WaterText;
    public bool isBuildingEnabled = false;
    public bool isTooltipOpen = false;
    public bool isDeadscreenEnabled = false;
    public bool isAnyTabOpen = false;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
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

        if(Deadscreen == null)
        {
            Deadscreen = GameObject.Find("Deadscreen");
        }

        if (Tooltip == null)
        {
            Tooltip = GameObject.Find("Tooltip");
        }

        isTooltipOpen = false;
        Tooltip.SetActive(false);

        isDeadscreenEnabled = false;
        Deadscreen.SetActive(false);
    }


    void Update()
    {
        UpdateFieldCount();
        IndicateBuildingMode();
    }

    

    private void EnableBuildMenu()
    {
        FieldBuilder.SetActive(true);
        isBuildingEnabled = true;
        _plantation.inBuildMenu = true;
        _plantation.HandleFieldPreview();
        _plantation.BuildingMode = _plantation.BuildingModeList[0];
    }

    private void DisableBuildMenu()
    {
        FieldBuilder.SetActive(false);
        isBuildingEnabled = false;
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

    public void OnSave()
    {
        if (_plantation != null)
        {
            _plantation.SaveFarm();
        }

        CharacterBasics playerBasics = GetPlayerCharacterBasics();
        if (playerBasics != null)
        {
            SaveSystem.SavePlayer(playerBasics);
        }

        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
        }
    }

    public void OnLoad()
    {
        if (_plantation != null)
        {
            _plantation.LoadFarm();
        }

        CharacterBasics playerBasics = GetPlayerCharacterBasics();
        if (playerBasics != null)
        {
            playerBasics.LoadPlayer();
        }

        if (inventoryManager != null)
        {
            inventoryManager.LoadInventory();
        }
    }

    private CharacterBasics GetPlayerCharacterBasics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Cannot save/load player: no GameObject with tag Player found.");
            return null;
        }

        CharacterBasics characterBasics = player.GetComponent<CharacterBasics>();

        if (characterBasics == null)
        {
            Debug.LogError("Cannot save/load player: Player does not have CharacterBasics.");
        }

        return characterBasics;
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

    public void OnThrowOutOfEquipment()
    {
        inventoryManager.ThrowOutOfEquipment(inventoryManager.currentHeldSlot.ItemInSlot, 1);
    }

    #region ----- UI Screen Toggles -----
    public void ToggleTooltip()
    {
        if (Tooltip != null)
        {
            if (isTooltipOpen)
            {
                Tooltip.SetActive(false);
                isTooltipOpen = false;
            }
            else
            {
                Tooltip.SetActive(true);
                isTooltipOpen = true;
            }
        }
    }

    public void ToggleDeadscreen(bool state)
    {
        if(Deadscreen != null) 
        {
            Deadscreen.SetActive(state);
            isDeadscreenEnabled = state;
        }
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
    #endregion
}
