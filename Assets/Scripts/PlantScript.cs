using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public PlantData plantData;

    private float currentGrowth;
    private float currentWater;
    private float baseTimeToWater;
    private float baseGrowthTime;
    private bool isHarvestable = false;
    private int currentStage = 0;

    public Sprite[] growthStages;
    public Sprite currentSprite;

    public float dryTime = 50f;

    void Start() 
    {
        // Initialize plant timers and growth stages
        currentGrowth = plantData.currentTimeBetweenStages;
        currentWater = plantData.currentTimeToWater;
        baseGrowthTime = plantData.timeBetweenStages;
        baseTimeToWater = plantData.timeToWater;

        // Combine baby and adult stages into a single array for easier management
        growthStages = new Sprite[plantData.babyStage.Length + plantData.adultStage.Length];
        plantData.babyStage.CopyTo(growthStages, 0);
        plantData.adultStage.CopyTo(growthStages, plantData.babyStage.Length);
        currentStage = plantData.currentGrowthStage;

        isHarvestable = false;
        currentSprite = gameObject.GetComponent<Sprite>();
        currentSprite = growthStages[currentStage];
    }

    void Update()
    {
        if (currentWater > 0)
        {
            currentWater -= Time.deltaTime;            
        }

        PlantGrowth();
    }

    private void PlantGrowth()
    {
        float timeToDry =  dryTime;
        int stagesCount = growthStages.Length;
        float timePerStage = baseGrowthTime / stagesCount;

        // If the plant has water, it continues to grow
        if (currentWater >= 0)
        {
            if (currentGrowth > 0 && currentStage < stagesCount)
            {
                currentGrowth = timePerStage;
                currentGrowth -= Time.deltaTime;

            }
            else if (currentGrowth == 0 && currentStage < stagesCount)
            {
                currentStage++;
                plantData.currentGrowthStage = currentStage;
                currentSprite = growthStages[plantData.currentGrowthStage];
            }

            if (currentGrowth == 0 && currentStage == stagesCount)
            {
                isHarvestable = true;
            }
        }
        else // If the plant has run out of water, it starts drying out
        {
            while (timeToDry > 0)
            {
                timeToDry -= Time.deltaTime;
            }

            if(timeToDry <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void WaterThePlant()
    {
        if (currentWater == 0)
        {
            currentWater = baseTimeToWater;
        }
    }
}
