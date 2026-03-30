using System.Threading;
using UnityEngine;

public class FarmScript : MonoBehaviour
{
    public float baseWaterTimer = 30f;
    private float waterTimer;
    public bool isWatered = false;
    public float growSpeed = 1f;

    private void FixedUpdate()
    {
        WateredToDry();

    }

    public void WateredToDry()
    {
        if (isWatered)
        {
            waterTimer = Time.time;
            if (waterTimer == 0)
            {
                isWatered = false;

            }
        }
    }

    public void WaterTheField()
    {
        isWatered = true;
        waterTimer = baseWaterTimer;
    }
}
