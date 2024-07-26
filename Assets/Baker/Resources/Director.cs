using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Director : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject player;
    public float waitTime = 1f;
    public Transform enemiesParent; 
    public List<GameObject> enemies = new List<GameObject>();

    private Vector3Int playerGridPosition;
    private List<Vector3Int> enemyGridPositions = new List<Vector3Int>();

    void Start()
    {
        StartCoroutine(PositionCharacterAfterGeneration());
        foreach (Transform enemy in enemiesParent)
        {
            enemies.Add(enemy.gameObject);
        }
    }

    IEnumerator PositionCharacterAfterGeneration()
    {
        // Wait for the Tilemap to be generated
        yield return new WaitForSeconds(waitTime);

        // Ensure the Tilemap is assigned
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned.");
            yield break;
        }

        // Look for and pull the baseGrid variable from the RoomGenerator script
        if (RoomGenerator.Instance != null)
        {
            int[,] grid = RoomGenerator.Instance.baseGrid;
            List<Vector3Int> floorPositions = new List<Vector3Int>();
            List<int> weights = new List<int>();

            // Store positions of all floor tiles (elements stored as "2") into floorPositions
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (grid[row, col] == 2)
                    {
                        Vector3Int position = new Vector3Int(col, row, 0);
                        floorPositions.Add(position);

                        // Higher weight for positions on the left side
                        int weight = grid.GetLength(1) - col; // Higher weight for lower column indices
                        weights.Add(weight);
                    }
                }
            }

            // Ensure there are floor positions to choose from
            if (floorPositions.Count == 0)
            {
                Debug.LogError("No floor positions found.");
                yield break;
            }

            // Calculate offset to align baseGrid with tilemap
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int baseGridOffset = new Vector3Int(bounds.xMin, bounds.yMin, 0);

            // Get a weighted random position from the list of floor positions and apply offset
            Vector3Int randomFloorPosition = GetWeightedRandomPosition(floorPositions, weights) + baseGridOffset;

            // Store the player's grid position
            playerGridPosition = randomFloorPosition;

            // Send the position to the PlayerSpawn script
            PlayerSpawn playerSpawn = player.GetComponent<PlayerSpawn>();
            if (playerSpawn != null)
            {
                playerSpawn.SetSpawnPosition(playerGridPosition);
            }
            else
            {
                Debug.LogError("PlayerSpawn component not found on player object.");
            }

            // Determine and store enemy positions
            DetermineEnemyPositions(floorPositions, baseGridOffset);
        }
    }

    public Vector3Int GetPlayerGridPosition()
    {
        return playerGridPosition;
    }

    public void UpdatePlayerGridPosition(Vector3Int newPosition)
    {
        playerGridPosition = newPosition;
        Debug.Log("Director updated with new player grid position: " + playerGridPosition);
    }

    public List<Vector3Int> GetEnemyGridPositions()
    {
        return enemyGridPositions;
    }

    public void UpdateEnemyGridPosition(int index, Vector3Int newPosition)
    {
        if (index >= 0 && index < enemyGridPositions.Count)
        {
            enemyGridPositions[index] = newPosition;
            Debug.Log("Updated enemy grid position at index " + index + ": " + newPosition);
        }
    }

    private void DetermineEnemyPositions(List<Vector3Int> floorPositions, Vector3Int baseGridOffset)
    {
        int minEnemies = 2; // Minimum number of enemies to spawn
        int maxEnemies = 4; // Maximum number of enemies to spawn
        int minDistanceFromPlayer = 2; // Minimum distance from the player

        int numEnemies = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Vector3Int randomFloorPosition;
            do
            {
                randomFloorPosition = floorPositions[Random.Range(0, floorPositions.Count)] + baseGridOffset;
            } while (Vector3.Distance(tilemap.CellToWorld(randomFloorPosition), tilemap.CellToWorld(playerGridPosition)) < minDistanceFromPlayer);

            enemyGridPositions.Add(randomFloorPosition);
        }
    }

    Vector3Int GetWeightedRandomPosition(List<Vector3Int> positions, List<int> weights)
    {
        int totalWeight = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            totalWeight += weights[i];
        }

        int randomValue = Random.Range(0, totalWeight);
        for (int i = 0; i < positions.Count; i++)
        {
            if (randomValue < weights[i])
            {
                return positions[i];
            }
            randomValue -= weights[i];
        }

        // Fallback in case something goes wrong
        return positions[0];
    }
}
