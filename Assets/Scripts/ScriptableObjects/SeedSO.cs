using UnityEngine;

[CreateAssetMenu(fileName = "SeedItem", menuName = "SO/SeedItemSO")]
public class SeedSO : ItemSO
{
    public PlantData plantData;
    public PlantData plantPreview;
    public float growthTime;
}
