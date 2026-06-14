using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeSpawner : MonoBehaviour
{
    public Node nodePrefab;
    public Grid grid;
    public GameObject NodeParent;

    void Start()
    {
        HashSet<Vector3Int> spawnedCells = new HashSet<Vector3Int>();
        Tilemap[] tilemaps = grid.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Vector3Int cellPosition in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(cellPosition) || !spawnedCells.Add(cellPosition))
                {
                    continue;
                }

                Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
                Node node = Instantiate(nodePrefab, spawnPosition, Quaternion.identity, NodeParent.transform);
            }
        }
    }
}
