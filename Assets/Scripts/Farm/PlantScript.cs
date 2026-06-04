using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public PlantData plantData;

    private float currentGrowth;
    private float currentWater;
    private float baseTimeToWater;
    private float baseGrowthTime;
    private bool isHarvestable = false;
    [SerializeField] private int currentStage = 0;
    private bool isWatered;

    public Sprite[] growthStages;
    public Sprite currentSprite;
    public FarmScript farmScript;
    public SpriteRenderer sr;


    public float dryTime = 50f;

    void Awake() 
    {

        currentGrowth = plantData.currentTimeBetweenStages;
        currentWater = plantData.currentTimeToWater;
        baseGrowthTime = plantData.timeBetweenStages;
        baseTimeToWater = plantData.timeToWater;
        sr = gameObject.GetComponent<SpriteRenderer>();

        growthStages = new Sprite[plantData.babyStage.Length + plantData.adultStage.Length];
        plantData.babyStage.CopyTo(growthStages, 0);
        plantData.adultStage.CopyTo(growthStages, plantData.babyStage.Length);
        currentStage = plantData.currentGrowthStage;
        currentSprite = growthStages[currentStage];
        sr.sprite = currentSprite;
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.2f , 0);
        gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        isHarvestable = false;
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
        bool posToAdultMoved = false;

        // If the plant has water, it continues to grow
        if (currentWater >= 0)
        {

            if(currentStage < stagesCount - 1)
            {
                currentGrowth -= Time.deltaTime;
                if (currentGrowth <= 0f)
                {
                    currentStage++;

                    if (currentStage >= plantData.babyStage.Length && !posToAdultMoved)
                    {
                        gameObject.transform.position += new Vector3(0, 0.2f, 0);
                        posToAdultMoved = true;
                    }

                    if(currentStage < stagesCount)
                    {
                        currentSprite = growthStages[currentStage];
                        sr.sprite = currentSprite;

                        if (sr != null)
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
