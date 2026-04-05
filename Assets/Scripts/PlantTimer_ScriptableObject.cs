using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "ScriptableObjects/PlantScript")]

public class PlantTimer_ScriptableObject : ScriptableObject
{
    public string plantName;
    public float timeBetweenStages;
    public float timeToWater;
    public Sprite[] growthStages;

    [NonSerialized]
    public float currentTimeBetweenStages;
    [NonSerialized]
    public float currentTimeToWater;
    [NonSerialized]
    public Sprite currentGrowthStage;

    public void SavePlantTimers()
    {
        string growKey = $"Plant_{plantName}_timeBetweenStages";
        string waterKey = $"Plant_{plantName}_timeToWater";
        string stageKey = $"Plant_{plantName}_growthStage";
        PlayerPrefs.SetFloat(growKey, timeBetweenStages);
        PlayerPrefs.SetFloat(waterKey, timeToWater);
        PlayerPrefs.SetInt(stageKey, Array.IndexOf(growthStages, currentGrowthStage));
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
            currentGrowthStage = growthStages[PlayerPrefs.GetInt(stageKey)];
        }

    }
}
