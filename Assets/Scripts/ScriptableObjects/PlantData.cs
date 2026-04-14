using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "SO/PlantData")]

public class PlantData : ScriptableObject
{
    public string plantName;
    public float timeBetweenStages;
    public float timeToWater;
    public Sprite[] babyStage;
    public Sprite[] adultStage;
   
    [NonSerialized]
    public float currentTimeBetweenStages;
    [NonSerialized]
    public float currentTimeToWater;
    [NonSerialized]
    public int currentGrowthStage;



    public void SavePlantTimers()
    {
        string growKey = $"Plant_{plantName}_timeBetweenStages";
        string waterKey = $"Plant_{plantName}_timeToWater";
        string stageKey = $"Plant_{plantName}_growthStage";
        PlayerPrefs.SetFloat(growKey, timeBetweenStages);
        PlayerPrefs.SetFloat(waterKey, timeToWater);
        PlayerPrefs.SetInt(stageKey, currentGrowthStage);
        PlayerPrefs.Save();
    }

    public void LoadPlantTimers()
    {
        string growKey = $"Plant_{plantName}_timeBetweenStages";
        string waterKey = $"Plant_{plantName}_timeToWater";
        string stageKey = $"Plant_{plantName}_growthStage";

        if (PlayerPrefs.HasKey(growKey)) 
        {
            currentTimeBetweenStages = PlayerPrefs.GetFloat(growKey);
        }

        if(PlayerPrefs.HasKey(waterKey))
        {
            currentTimeToWater = PlayerPrefs.GetFloat(waterKey);
        }

        if(PlayerPrefs.HasKey(stageKey))
        {
            currentGrowthStage = PlayerPrefs.GetInt(stageKey);
        }

    }
}
