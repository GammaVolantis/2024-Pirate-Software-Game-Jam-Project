using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSpawn : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject characterPrefab;

    private Vector3Int spawnPosition;
    private Director director;

    void Start()
    {
        director = FindObjectOfType<Director>();
    }

    public void SetSpawnPosition(Vector3Int position)
    {
        spawnPosition = position;
        PositionCharacter();
    }

    private void PositionCharacter()
    {
        // Convert the tile position to world position using the Tilemap's method
        Vector3 worldPosition = tilemap.CellToWorld(spawnPosition);

        // Center the character on the tile by offsetting by half of the tile size
        Vector3 offset = new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
        worldPosition += offset;

        // Instantiate the character at the world position
        transform.position = worldPosition;

        // Update the Director with the player's grid position
        if (director != null)
        {
            director.UpdatePlayerGridPosition(spawnPosition);
        }
        else
        {
            Debug.LogError("Director not found.");
        }
    }
}
