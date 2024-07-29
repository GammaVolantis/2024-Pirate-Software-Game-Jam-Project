using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawn : MonoBehaviour
{
    public Tilemap tilemap;
    public List<GameObject> characterPrefabs; // List of all possible character prefabs
    public List<int> eligiblePrefabIndices; // Indices of eligible prefabs in the characterPrefabs list
    private Director director;
    private LocationData locationData;
    private int iter = 0;

    void Start()
    {
        UnityEngine.Debug.Log("EnemySpawn Start method called.");
        locationData = Resources.Load<LocationData>("AllLocationInformation");
        //locationData.ResetEnemiesLocationData();
        director = FindObjectOfType<Director>();
        if (director == null)
        {
            UnityEngine.Debug.LogError("Director instance not found!");
            return;
        }

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        UnityEngine.Debug.Log("Waiting for tilemap generation.");
        yield return new WaitForSeconds(director.waitTime);

        // Wait until the Director has initialized enemy positions
        while (!director.enemiesInitialized)
        {
            yield return null; // Wait for the next frame
        }

        // Ensure the Tilemap is assigned
        if (tilemap == null)
        {
            UnityEngine.Debug.LogError("Tilemap not assigned.");
            yield break;
        }

        // Ensure the Enemies parent is assigned
        if (director.enemiesParent == null)
        {
            UnityEngine.Debug.LogError("Enemies parent not assigned in Director.");
            yield break;
        }

        // Get enemy positions from the Director
        List<Vector3Int> enemyPositions = director.GetEnemyGridPositions();
        if (enemyPositions.Count == 0)
        {
            UnityEngine.Debug.LogError("No enemy positions found.");
            yield break;
        }

        foreach (Vector3Int gridPosition in enemyPositions)
        {
            // Ensure there are eligible prefabs to choose from
            if (eligiblePrefabIndices.Count == 0 || characterPrefabs.Count == 0)
            {
                UnityEngine.Debug.LogError("No eligible prefabs or character prefabs assigned.");
                yield break;
            }

            int randomPrefabIndex = UnityEngine.Random.Range(0, eligiblePrefabIndices.Count);
            if (randomPrefabIndex < 0 || randomPrefabIndex >= characterPrefabs.Count)
            {
                UnityEngine.Debug.LogError("Random prefab index out of bounds.");
                yield break;
            }

            GameObject characterPrefab = characterPrefabs[eligiblePrefabIndices[randomPrefabIndex]];
            if (characterPrefab == null)
            {
                UnityEngine.Debug.LogError("Character prefab is null.");
                yield break;
            }

            Vector3 worldPosition = tilemap.CellToWorld(gridPosition) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            UnityEngine.Debug.Log($"Instantiating enemy at: {worldPosition} (Grid Position: {gridPosition})");
            GameObject enemy = Instantiate(characterPrefab, worldPosition, Quaternion.identity, director.enemiesParent);
            Debug.Log("EnemyObj = " + enemy);
            locationData.AddEnemyLocations(gridPosition, worldPosition, enemy);
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            enemyMovement.SetEnemyIndexVal(iter);
            iter += 1;
            if (enemy == null)
            {
                UnityEngine.Debug.LogError("Failed to instantiate enemy.");
                yield break;
            }
            else 
            { 
                UnityEngine.Debug.Log("Spawned enemy successfully."); 
            }
        }
    }
}
