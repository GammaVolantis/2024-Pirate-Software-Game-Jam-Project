using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class CharacterPositioning : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject characterPrefab;
    public float waitTime = 1f; // Adjust this time as necessary

    void Start()
    {
        StartCoroutine(PositionCharacterAfterGeneration());
    }

    IEnumerator PositionCharacterAfterGeneration()
    {
        // Wait for the Tilemap to be generated
        yield return new WaitForSeconds(waitTime);

        // Ensure the Tilemap has tiles
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned.");
            yield break;
        }

        // Get the bounds of the Tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Log the bounds for debugging
        Debug.Log("Tilemap bounds: " + bounds);

        // Verify that the bounds are not empty
        if (bounds.size.x == 0 || bounds.size.y == 0)
        {
            Debug.LogError("Tilemap bounds are empty. Ensure tiles are painted on the Tilemap.");
            yield break;
        }

        // Generate random cell position within the Tilemap bounds
        Vector3Int cellPosition = new Vector3Int(
            Random.Range(bounds.xMin, bounds.xMax),
            Random.Range(bounds.yMin, bounds.yMax),
            0 // Assuming a 2D Tilemap
        );

        // Log the random cell position for debugging
        Debug.Log("Random cell position: " + cellPosition);

        // Convert cell position to world position
        Vector3 worldPosition = tilemap.CellToWorld(cellPosition);

        // Center the character on the tile
        Vector3 offset = tilemap.cellSize / 2;
        worldPosition += offset;

        // Instantiate the character at the world position
        Instantiate(characterPrefab, worldPosition, Quaternion.identity);
    }
}
