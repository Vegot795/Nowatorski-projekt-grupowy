using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FarmScript : MonoBehaviour
{
    public float baseWaterTimer = 30f;
    [SerializeField] private float waterTimer;
    public bool isWatered = false;
    public float growSpeed = 1f;
    public bool isOccupied = false;
    public List<Slider> statsSliders;
    private void Awake()
    {
        statsSliders = new List<Slider>(GetComponentsInChildren<Slider>(true));
        WaterTheField();
    }

    private void Update()
    {
        WateredToDry();
    }

    public void WateredToDry()
    {
        if (!isWatered)
            return;

        Slider waterSlider = statsSliders.Find(s => s.name == "Water");

        waterTimer -= Time.deltaTime;

        waterSlider.value = waterTimer / baseWaterTimer;

        if (waterTimer <= 0)
        {
            waterTimer = 0;
            isWatered = false;
        }
    }

    public void WaterTheField()
    {
        isWatered = true;
        waterTimer = baseWaterTimer;
    }


}
