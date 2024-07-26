using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    public Tilemap tilemap;
    public List<GameObject> characterPrefabs; // List of all possible character prefabs
    public List<int> eligiblePrefabIndices; // Indices of eligible prefabs in the characterPrefabs list
    private Director director;

    void Start()
    {
        director = FindObjectOfType<Director>();
        if (director == null)
        {
            Debug.LogError("Director instance not found!");
        }

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // Wait for the Tilemap to be generated
        yield return new WaitForSeconds(director.waitTime);

        // Ensure the Tilemap is assigned
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned.");
            yield break;
        }

        // Ensure the Enemies parent is assigned
        if (director.enemiesParent == null)
        {
            Debug.LogError("Enemies parent not assigned in Director.");
            yield break;
        }

        // Get enemy positions from the Director
        List<Vector3Int> enemyPositions = director.GetEnemyGridPositions();

        foreach (Vector3Int gridPosition in enemyPositions)
        {
            // Select a random eligible prefab
            int randomPrefabIndex = eligiblePrefabIndices[Random.Range(0, eligiblePrefabIndices.Count)];
            GameObject characterPrefab = characterPrefabs[randomPrefabIndex];

            // Convert grid position to world position
            Vector3 worldPosition = tilemap.CellToWorld(gridPosition) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);

            // Instantiate the character at the world position as a child of the enemiesParent
            GameObject enemy = Instantiate(characterPrefab, worldPosition, Quaternion.identity, director.enemiesParent);
            Debug.Log("Spawned enemy at: " + worldPosition + " (Grid Position: " + gridPosition + ")");
        }
    }
}
