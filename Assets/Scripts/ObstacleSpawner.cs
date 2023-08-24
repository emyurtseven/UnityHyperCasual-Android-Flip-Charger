using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages spawning of obstacles.
/// IMPORTANT: This script needs to be loaded after default time.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    GameObject[] obstaclePrefabs;


    float minSpawnInterval;
    float maxSpawnInterval;
    float obstacleMoveSpeed;


    int obstacleCount;

    // Start is called before the first frame update
    void Start()
    {
        obstaclePrefabs = Resources.LoadAll<GameObject>("Prefabs/Obstacles");
        obstacleCount = obstaclePrefabs.Length;

        EventManager.AddNoArgumentListener(OnDifficultyChangedListener, EventType.DifficultyChanged);

        obstacleMoveSpeed = GameManager.Instance.GameSpeed;
        (minSpawnInterval, maxSpawnInterval) = GameManager.Instance.SpawnIntervals;

        StartCoroutine(SpawnCoroutine());
    }

    private void OnDifficultyChangedListener()
    {
        obstacleMoveSpeed = GameManager.Instance.GameSpeed;
        (minSpawnInterval, maxSpawnInterval) = GameManager.Instance.SpawnIntervals;
    }

    private IEnumerator SpawnCoroutine()
    {
        Array pooledObjectTypes = Enum.GetValues(typeof(PooledObjectType));

        if (pooledObjectTypes.Length != obstaclePrefabs.Length)
        {
            Debug.LogError("Some obstacles missing from PooledObjectType enum values.");
            yield break;
        }

        int totalSign = 0;

        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, obstacleCount);

            PooledObjectType obstacleType = (PooledObjectType)pooledObjectTypes.GetValue(randomIndex);
            
            GameObject newObstacleObj = ObjectPool.GetPooledObject(obstacleType);

            if (!newObstacleObj.TryGetComponent<Obstacle>(out Obstacle newObstacle))
            {
                Debug.LogWarning($"Object {newObstacleObj.name} is missing Obstacle script. Spawning another one");
                continue;
            }

            // set speed
            newObstacleObj.GetComponent<Obstacle>().MoveSpeed = obstacleMoveSpeed;

            int sign = UnityEngine.Random.Range(0, 2) * 2 - 1;

            totalSign += sign;

            if (Mathf.Abs(totalSign) > 2)
            {
                sign = -sign;
                totalSign = 0;
            }
            
            float spawnPosY = sign * obstaclePrefabs[randomIndex].transform.position.y;

            Vector2 spawnPos = new Vector2(transform.position.x, spawnPosY);

            newObstacleObj.transform.position = spawnPos;
            newObstacleObj.transform.localScale = new Vector2(1, sign);
            newObstacleObj.SetActive(true);

            float interval = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);
        }
    }
}
