using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public SeedSO seedData;

    [SerializeField] private int currentStage = 0;
    [SerializeField] private float dropRadius = 0.5f;
    private float currentGrowth;
    private float currentWater;
    private float baseTimeToWater;
    private float baseGrowthTime;
    private bool isHarvestable = false;
    private bool isWatered;
    private bool posToAdultMoved = false;

    public Sprite[] growthStages;
    public Sprite currentSprite;
    public FarmScript farmScript;
    public SpriteRenderer sr;
    public int dropCount = 2;



    public float dryTime = 50f;

    void Awake()
    {

        currentGrowth = seedData.currentTimeBetweenStages;
        currentWater = seedData.currentTimeToWater;
        baseGrowthTime = seedData.timeBetweenStages;
        baseTimeToWater = seedData.timeToWater;
        sr = gameObject.GetComponent<SpriteRenderer>();

        growthStages = new Sprite[seedData.babyStage.Length + seedData.adultStage.Length];
        seedData.babyStage.CopyTo(growthStages, 0);
        seedData.adultStage.CopyTo(growthStages, seedData.babyStage.Length);
        currentStage = seedData.currentGrowthStage;
        currentSprite = growthStages[currentStage];
        sr.sprite = currentSprite;

        isHarvestable = false;
        posToAdultMoved = false;



        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }
    void Start()
    {
        farmScript = transform.parent.GetComponent<FarmScript>();
        if (farmScript != null)
        {
            Debug.Log("Found parent");
        }
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
        float timeToDry = dryTime;
        int stagesCount = growthStages.Length;
        float timePerStage = baseGrowthTime / stagesCount;

        // If the plant has water, it continues to grow
        if (currentWater >= 0)
        {

            if (currentStage < stagesCount - 1)
            {
                currentGrowth -= Time.deltaTime;
                if (currentGrowth <= 0f)
                {
                    currentStage++;

                    if (currentStage >= seedData.babyStage.Length && !posToAdultMoved)
                    {
                        gameObject.transform.position += new Vector3(0, 0.2f, 0);
                        gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
                        posToAdultMoved = true;
                    }

                    if (currentStage < stagesCount)
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
            dryTime -= Time.deltaTime;
            if (dryTime <= 0)
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

    public void HarvestPlant()
    {
        if (isHarvestable)
        {
            farmScript.isOccupied = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Vector2 dropLocation = (Vector2)transform.position + Random.insideUnitCircle * dropRadius;
            GameObject SeedDrop = Instantiate(seedData.itemPickupPrefab, dropLocation, Quaternion.identity);
            var SeedDropIP = SeedDrop.GetComponent<ItemPickup>();
            SeedDropIP.item = seedData;
            SeedDropIP.count = dropCount;
            Destroy(gameObject);
        }
    }
}
