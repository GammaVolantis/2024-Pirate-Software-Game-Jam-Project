using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject characterPrefab;
    public float waitTime = 1f;

    void Start()
    {
        StartCoroutine(PositionCharacterAfterGeneration());
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

            // Convert the tile position to world position using the Tilemap's method
            Vector3 worldPosition = tilemap.CellToWorld(randomFloorPosition);

            // Center the character on the tile by offsetting by half of the tile size
            Vector3 offset = new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            worldPosition += offset;

            // Instantiate the character at the world position
            Instantiate(characterPrefab, worldPosition, Quaternion.identity);
            // transform.position = worldPosition;
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
