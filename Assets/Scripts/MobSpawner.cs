using NUnit.Framework;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] GameObject FlyingEyePrefab;
    [SerializeField] List<FlyingEyeController> currentSpawnedEnemies = new List<FlyingEyeController>();
    [SerializeField] int maxSpawnedEnemies = 5;
    [SerializeField] Tilemap spawnAreaTilemap;
    [SerializeField] int spawnChance = 1;
    [SerializeField] float timeGap = 10f;
    [SerializeField] float currentTime;


    void Start()
    {
        if (FlyingEyePrefab == null)
        {
            FlyingEyePrefab = Resources.Load<GameObject>("FlyingEye");
        }

        currentTime = timeGap;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = timeGap;
            if (currentSpawnedEnemies.Count < maxSpawnedEnemies)
            {
                int randomValue = Random.Range(0, spawnChance + 1);
                if (randomValue == spawnChance)
                {
                    GameObject enemy = SpawnEnemy(FlyingEyePrefab);
                    currentSpawnedEnemies.Add(enemy.GetComponent<FlyingEyeController>());
                }
            }
        }

    }
    void FixedUpdate()
    {
        RmoveDead();
    }
    void RmoveDead()
    {
        if (currentSpawnedEnemies != null)
        {
            for (int i = 0; i < currentSpawnedEnemies.Count; i++)
            {
                CharacterBasics enemiehp = currentSpawnedEnemies[i].gameObject.GetComponent<CharacterBasics>();
                if (enemiehp.CurrentHP <= 0)
                {
                    Destroy(currentSpawnedEnemies[i].gameObject);
                    currentSpawnedEnemies.RemoveAt(i);

                }
            }
        }

    }

    private GameObject SpawnEnemy(GameObject enemy)
    {
        Vector2 randomSpawnPoint = GetRandomSpawnPoint();
        GameObject spawnedEnemy = Instantiate(enemy, randomSpawnPoint, Quaternion.identity);
        spawnedEnemy.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        SpriteRenderer sr = spawnedEnemy.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Player";
        sr.sortingOrder = 0;

        return spawnedEnemy;
    }

    private Vector2 GetRandomSpawnPoint()
    {
        if (spawnAreaTilemap == null)
        {
            return Vector2.zero;
        }
        BoundsInt bounds = spawnAreaTilemap.cellBounds;
        Vector3Int randomCell = new Vector3Int(
            Random.Range(bounds.xMin, bounds.xMax),
            Random.Range(bounds.yMin, bounds.yMax),
            0
        );
        Vector3 worldPosition = spawnAreaTilemap.CellToWorld(randomCell);
        return worldPosition;
    }

    public void RemoveEnemyFromList(FlyingEyeController enemy)
    {
        Debug.Log("Remove enem");
        currentSpawnedEnemies.Remove(enemy);
    }

}