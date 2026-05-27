using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlantingSeedsScript : MonoBehaviour
{
    public SeedSO Seed;
    public SeedSO[] Seeds;

    private GameObject plantPreview;
    private Grid grid;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid = GameObject.FindWithTag("FarmGrid").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandePlantPreview(SeedSO seed)
    {
        GameObject plantPrefab = seed.plantPreview;

        if (grid == null)
        {
            Debug.LogWarning("Grid not set in PlantingSeedsScrip.");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogWarning("No main camera found.");
            return;
        }

        // Ensure preview exists
        if (plantPreview == null)
        {
            plantPreview = Instantiate(plantPrefab);
            SetPreviewMaterial(plantPreview, 0.5f);
            var sr = plantPreview.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Ground";
                sr.sortingOrder = 1;
            }
        }

        // Convert mouse to world point and test the point for a FarmField collider
        Vector3 worldPoint3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2 = new Vector2(worldPoint3.x, worldPoint3.y);

        Collider2D hit = Physics2D.OverlapPoint(worldPoint2);
        if (hit != null && hit.CompareTag("FarmField"))
        {
            Vector3Int cellPosition = grid.WorldToCell(worldPoint3);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
            plantPreview.transform.position = spawnPosition;
            plantPreview.transform.rotation = Quaternion.identity;

            // Optional: update preview sprite/type using 'seed' if ItemSO contains sprite/reference
            var sr = plantPreview.GetComponent<SpriteRenderer>();
            if (sr != null && seed != null) sr.sprite = seed.plantPreview.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            // Mouse not over a farm field -> hide/destroy preview
            if (plantPreview != null)
            {
                Destroy(plantPreview);
                plantPreview = null;
            }
            Debug.Log("Mouse is not over a farm field.");
        }
    }

    private void SetPreviewMaterial(GameObject previewObj, float alpha)
    {
        var renderers = previewObj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}
