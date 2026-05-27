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
    private bool isWatered;

    public Sprite[] growthStages;
    public Sprite currentSprite;
    public FarmScript farmScript;
    public SpriteRenderer sr;


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
    }

    void Update()
    {
        if (farmScript != null)
        {
            isWatered = farmScript.isWatered;
        }

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

            if(currentStage < stagesCount - 1)
            {
                currentGrowth -= Time.deltaTime;
                if (currentGrowth <= 0f)
                {
                    currentStage++;
                    plantData.currentGrowthStage = currentStage;

                    if(currentStage < stagesCount)
                    {
                        currentSprite = growthStages[currentStage];

                        if(sr != null)
                        {
                            sr.sprite = currentSprite;
                        }

                        currentGrowth = timePerStage;
                    }
                }
            }
            else
            {
                isHarvestable = true;
            }
        }
        else
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
