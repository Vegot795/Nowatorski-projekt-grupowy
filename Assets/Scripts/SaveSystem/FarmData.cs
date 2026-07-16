using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FarmSaveData 
{
    public bool DataIsWatered;
    public bool DataIsOccupied;
    public float[] DataFieldPosition;
    public float DataWaterTimer;
    public PlantSaveData DataPlant;

    public FarmSaveData(FarmScript farmScript)
    {
        DataIsWatered = farmScript.isWatered;
        DataIsOccupied = farmScript.isOccupied;
        DataWaterTimer = farmScript.waterTimer;

        DataFieldPosition = new float[3];
        DataFieldPosition[0] = farmScript.transform.position.x;
        DataFieldPosition[1] = farmScript.transform.position.y;
        DataFieldPosition[2] = farmScript.transform.position.z;

        PlantScript plantScript = farmScript.GetComponentInChildren<PlantScript>();

        if (farmScript.isOccupied && plantScript != null)
        {
            DataPlant = new PlantSaveData(plantScript);
        }
    }
}

[System.Serializable]
public class PlantSaveData
{
    public string DataSeedName;
    public int DataCurrentGrowthStage;
    public float DataCurrentGrowth;
    public bool DataIsHarvestable;

    public PlantSaveData(PlantScript plantScript)
    {
        DataSeedName = plantScript.seedData.name;
        DataCurrentGrowthStage = plantScript.currentStage;
        DataCurrentGrowth = plantScript.currentGrowth;
        DataIsHarvestable = plantScript.isHarvestable;
    }
}

[System.Serializable]
public class FarmSaveDataList
{
    public List<FarmSaveData> farms = new();
}
