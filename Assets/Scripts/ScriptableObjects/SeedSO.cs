using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedItem", menuName = "SO/SeedItemSO")]
public class SeedSO : ItemSO
{
    [Header("Seed/Plant Visual")]
    public GameObject plantPrefab;
    public Sprite[] babyStage;
    public Sprite[] adultStage;

    [Header("Growth Timers")]
    public float timeBetweenStages;
    public float timeToWater;

    [Header("Instance Data (Runtime)")]
    [NonSerialized] public int currentGrowthStage = 0;
    [NonSerialized] public float currentTimeBetweenStages;
    [NonSerialized] public float currentTimeToWater;

    public void ResetForNewPlant()
    {
        currentGrowthStage = 0;
        currentTimeBetweenStages = timeBetweenStages;
        currentTimeToWater = timeToWater;
    }

    public void SavePlantTimers()
    {
        string growKey = $"Plant_{name}_timeBetweenStages";
        string waterKey = $"Plant_{name}_timeToWater";
        string stageKey = $"Plant_{name}_growthStage";
        PlayerPrefs.SetFloat(growKey, timeBetweenStages);
        PlayerPrefs.SetFloat(waterKey, timeToWater);
        PlayerPrefs.SetInt(stageKey, currentGrowthStage);
        PlayerPrefs.Save();
    }

    public void LoadPlantTimers()
    {
        string growKey = $"Plant_{name}_timeBetweenStages";
        string waterKey = $"Plant_{name}_timeToWater";
        string stageKey = $"Plant_{name}_growthStage";

        if (PlayerPrefs.HasKey(growKey))
            currentTimeBetweenStages = PlayerPrefs.GetFloat(growKey);

        if(PlayerPrefs.HasKey(waterKey))
            currentTimeToWater = PlayerPrefs.GetFloat(waterKey);

        if(PlayerPrefs.HasKey(stageKey))
            currentGrowthStage = PlayerPrefs.GetInt(stageKey);
    }
}
