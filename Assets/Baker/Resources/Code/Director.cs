using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject player;
    public float waitTime = 5f;
    public Transform enemiesParent;
    public List<GameObject> enemies = new List<GameObject>();
    public OverworldData overworldData;

    public int minEnemies = 2;
    public int maxEnemies = 4;

    private LocationData locationData;
    private Vector3Int playerGridPosition;
    private List<Vector3Int> enemyGridPositions = new List<Vector3Int>();
    public bool enemiesInitialized = false; // Flag to indicate enemy initialization

    void Start()
    {
        overworldData = Resources.Load<OverworldData>("OverworldData");
        UnityEngine.Debug.Log("Director Start method called.");
        locationData = Resources.Load<LocationData>("AllLocationInformation");
        if (overworldData.furthestInstance == overworldData.playerPosition) 
        {
            SceneManager.LoadScene("Win Scene");
        }
        while (tilemap == null)
        {

        }
        
        StartCoroutine(PositionCharacterAfterGeneration());
              

        foreach (Transform enemy in enemiesParent)
        {
            enemies.Add(enemy.gameObject);
        }
    }

    IEnumerator PositionCharacterAfterGeneration()
    {
        UnityEngine.Debug.Log("Waiting for tilemap generation.");
        yield return new WaitForSeconds(waitTime);

        if (tilemap == null)
        {
            UnityEngine.Debug.LogError("Tilemap not assigned.");
            yield break;
        }

        if (RoomGenerator.Instance != null)
        {
            int[,] grid = RoomGenerator.Instance.baseGrid;

            List<Vector3Int> floorPositions = new List<Vector3Int>();
            List<int> weights = new List<int>();

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    Debug.Log("LOOK HERE!!!: " + grid[row, col]);
                    if (grid[row, col] == 2)
                    {
                        Vector3Int position = new Vector3Int(col, row, 0);
                        floorPositions.Add(position);

                        int weight = grid.GetLength(1) - col;
                        weights.Add(weight);
                    }
                }
            }

            if (floorPositions.Count == 0)
            {
                UnityEngine.Debug.LogError("No floor positions found.");
                yield break;
            }

            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int baseGridOffset = new Vector3Int(bounds.xMin, bounds.yMin, 0);

            Vector3Int randomFloorPosition = GetWeightedRandomPosition(floorPositions, weights) + baseGridOffset;

            playerGridPosition = randomFloorPosition;

            PlayerSpawn playerSpawn = player.GetComponent<PlayerSpawn>();
            if (playerSpawn != null)
            {
                playerSpawn.SetSpawnPosition(playerGridPosition);
            }
            else
            {
                UnityEngine.Debug.LogError("PlayerSpawn component not found on player object.");
            }

            DetermineEnemyPositions(floorPositions, baseGridOffset);
            enemiesInitialized = true; // Set the flag to indicate enemy positions are initialized
        }
        else
        {
            UnityEngine.Debug.LogError("RoomGenerator instance not found.");
        }
    }

    public Vector3Int GetPlayerGridPosition()
    {
        return playerGridPosition;
    }

    public void UpdatePlayerGridPosition(Vector3Int newPosition)
    {
        playerGridPosition = newPosition;
        UnityEngine.Debug.Log("Director updated with new player grid position: " + playerGridPosition);
    }

    public List<Vector3Int> GetEnemyGridPositions()
    {
        UnityEngine.Debug.Log("Returning enemy grid positions: " + enemyGridPositions.Count);
        return enemyGridPositions;
    }

    public void UpdateEnemyGridPosition(int index, Vector3Int newPosition)
    {
        if (index >= 0 && index < enemyGridPositions.Count)
        {
            enemyGridPositions[index] = newPosition;
            UnityEngine.Debug.Log("Updated enemy grid position at index " + index + ": " + newPosition);
        }
    }

    private void DetermineEnemyPositions(List<Vector3Int> floorPositions, Vector3Int baseGridOffset)
    {
        int minDistanceFromPlayer = 2;

        int numEnemies = UnityEngine.Random.Range(minEnemies, maxEnemies + 1);
        UnityEngine.Debug.Log("Spawning " + numEnemies + " enemies.");

        for (int i = 0; i < numEnemies; i++)
        {
            Vector3Int randomFloorPosition;
            do
            {
                randomFloorPosition = floorPositions[UnityEngine.Random.Range(0, floorPositions.Count)] + baseGridOffset;
                UnityEngine.Debug.Log("Checking enemy position: " + randomFloorPosition);
            } while (Vector3.Distance(tilemap.CellToWorld(randomFloorPosition), tilemap.CellToWorld(playerGridPosition)) < minDistanceFromPlayer);

            enemyGridPositions.Add(randomFloorPosition);
            UnityEngine.Debug.Log("Enemy position added: " + randomFloorPosition);
        }
    }

    Vector3Int GetWeightedRandomPosition(List<Vector3Int> positions, List<int> weights)
    {
        int totalWeight = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            totalWeight += weights[i];
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < positions.Count; i++)
        {
            if (randomValue < weights[i])
            {
                return positions[i];
            }
            randomValue -= weights[i];
        }

        return positions[0];
    }
}