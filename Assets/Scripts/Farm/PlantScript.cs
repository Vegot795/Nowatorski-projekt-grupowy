using UnityEngine;
using UnityEngine.UI;

public class PlantScript : MonoBehaviour, IInteraction
{
    public SeedSO seedData;

    [SerializeField] public int currentStage = 0;
    [SerializeField] private float dropRadius = 0.5f;

    public float currentGrowth;
    private float baseGrowthTime;
    public bool isHarvestable = false;
    private bool posToAdultMoved = false;

    public Sprite[] growthStages;
    public Sprite currentSprite;
    public FarmScript farmScript;
    public SpriteRenderer sr;
    public int dropCount = 2;

    private Slider growSlider;

    void Awake()
    {
        baseGrowthTime = seedData.timeBetweenStages;

        sr = gameObject.GetComponent<SpriteRenderer>();

        growthStages = new Sprite[seedData.babyStage.Length + seedData.adultStage.Length];
        seedData.babyStage.CopyTo(growthStages, 0);
        seedData.adultStage.CopyTo(growthStages, seedData.babyStage.Length);

        currentStage = seedData.currentGrowthStage;
        currentSprite = growthStages[currentStage];
        sr.sprite = currentSprite;

        currentGrowth = baseGrowthTime / growthStages.Length;

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

            growSlider = farmScript.statsSliders[1];

            if (growSlider != null)
            {
                growSlider.gameObject.SetActive(true);
                growSlider.value = 0f;
            }
        }
    }

    void Update()
    {
        PlantGrowth();
    }

    private void PlantGrowth()
    {
        int stagesCount = growthStages.Length;
        float timePerStage = baseGrowthTime / stagesCount;


        if (farmScript == null || !farmScript.isWatered)
            return;

        if (currentStage < stagesCount - 1)
        {
            currentGrowth -= Time.deltaTime * farmScript.growSpeed;

            if (currentGrowth <= 0f)
            {
                currentStage++;

                if (currentStage >= seedData.babyStage.Length && !posToAdultMoved)
                {
                    transform.position += new Vector3(0, 0.2f, 0);
                    transform.localScale = new Vector3(2f, 2f, 2f);
                    posToAdultMoved = true;
                }

                currentSprite = growthStages[currentStage];
                sr.sprite = currentSprite;

                currentGrowth = timePerStage;
            }
        }
        else
        {
            isHarvestable = true;
        }
        if (growSlider != null)
        {
            growSlider.value = (float)currentStage / (growthStages.Length - 1);
        }
    }

    public void HarvestPlant()
    {
        if (isHarvestable)
        {
            farmScript.isOccupied = false;
            sr.enabled = false;
            Money money = FindAnyObjectByType<Money>();
            money.currentMoney += 10;
            growSlider.gameObject.SetActive(false);

            /*
            Vector2 dropLocation = (Vector2)transform.position + Random.insideUnitCircle * dropRadius;
            GameObject SeedDrop = Instantiate(seedData.itemPickupPrefab, dropLocation, Quaternion.identity);

            var SeedDropIP = SeedDrop.GetComponent<ItemPickup>();
            SeedDropIP.item = seedData;
            SeedDropIP.count = dropCount;*/

            Destroy(gameObject);
        }
    }
    public void Interact()
    {
        HarvestPlant();
    }

    public void RestoreGrowthState(int savedStage, float savedGrowth, bool savedHarvestable)
    {
        currentStage = savedStage;
        currentGrowth = savedGrowth;
        isHarvestable = savedHarvestable;
        if (currentStage >= growthStages.Length)
        {
            currentStage = growthStages.Length - 1;
        }
        currentSprite = growthStages[currentStage];
        sr.sprite = currentSprite;
        if (currentStage >= seedData.babyStage.Length && !posToAdultMoved)
        {
            transform.position += new Vector3(0, 0.2f, 0);
            transform.localScale = new Vector3(2f, 2f, 2f);
            posToAdultMoved = true;
        }
    }
}