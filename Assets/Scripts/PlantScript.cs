using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public PlantTimer_ScriptableObject _plantTimer;

    private float currentGrowth;
    private float currentWater;
    private float baseTimeToWater;
    private float baseGrowth;
    private bool isHarvestable = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        currentGrowth = _plantTimer.currentTimeBetweenStages;
        currentWater = _plantTimer.currentTimeToWater;
        baseGrowth = _plantTimer.timeBetweenStages;
        baseTimeToWater = _plantTimer.timeToWater;

    }

    // Update is called once per frame
    void Update()
    {
        currentWater -= Time.deltaTime;

        PlantGrowth(currentWater);
    }

    private void PlantGrowth(float currentWater)
    {
        if (currentWater <= 0)
        {
            currentGrowth -= Time.deltaTime;
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
