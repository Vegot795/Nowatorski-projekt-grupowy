using UnityEngine;

[CreateAssetMenu(fileName = "SeedItem", menuName = "SO/SeedItemSO")]
public class SeedSO : ItemSO
{
    public PlantData plantData;
    public GameObject plantPreview;
    public float growthTime;
}
