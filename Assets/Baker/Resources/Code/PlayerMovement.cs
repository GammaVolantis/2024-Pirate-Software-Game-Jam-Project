using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    private GameObject selectedCharacter;
    public Tilemap tilemap;
    public Tilemap overlayTilemap; // Secondary Tilemap for overlays
    public RuleTile overlayTile; // RuleTile to use for overlay
    public int maxMovementDistance = 2; // Maximum movement distance setting
    private Vector3Int currentCellPosition; // Store the player's current cell position
    private LocationData locationData;

    private Director director;

    void Awake()
    {
        locationData = Resources.Load<LocationData>("AllLocationInformation");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionally make this persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate instance
        }
    }

    void Start()
    {
        // Initialize currentCellPosition
        currentCellPosition = tilemap.WorldToCell(transform.position);
        Debug.Log("Initial Player current cell position: " + currentCellPosition);

        director = FindObjectOfType<Director>();
        if (director != null)
        {
            director.UpdatePlayerGridPosition(currentCellPosition);
        }
    }

    void Update()
    {
        // Always update player cell position for debugging
        currentCellPosition = tilemap.WorldToCell(transform.position);
        // Debug.Log("Updated Player current cell position: " + currentCellPosition);

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // Priority to Player Selection
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePos2D);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    selectedCharacter = hitCollider.gameObject;
                    Debug.Log("Selected character: " + selectedCharacter.name);
                    ShowMovementRange();
                    return; // Exit early to prioritize player selection
                }
            }

            // Check for tilemap click if no player was selected
            if (selectedCharacter != null)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
                if (IsFloorTile(cellPosition) && IsWithinMovementRange(cellPosition))
                {
                    MoveCharacterTo(cellPosition);
                    ClearMovementRange();
                }
            }
        }
    }

    bool IsFloorTile(Vector3Int cellPosition)
    {
        if (RoomGenerator.Instance != null)
        {
            int[,] grid = RoomGenerator.Instance.baseGrid;
            int row = cellPosition.y - tilemap.cellBounds.yMin;
            int col = cellPosition.x - tilemap.cellBounds.xMin;

            foreach (var enemyPos in director.GetEnemyGridPositions())
            {
                if (enemyPos == cellPosition)
                {
                    return false;
                }
            }

            if (row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1))
            {
                return grid[row, col] == 2; // Check if the tile is a floor tile
            }
        }
        return false;
    }

    bool IsWithinMovementRange(Vector3Int cellPosition)
    {
        Vector3Int characterPosition = tilemap.WorldToCell(selectedCharacter.transform.position);

        int dx = Mathf.Abs(characterPosition.x - cellPosition.x);
        int dy = Mathf.Abs(characterPosition.y - cellPosition.y);

        // Check movement restrictions
        return dx <= maxMovementDistance && dy <= maxMovementDistance && (dx + dy <= maxMovementDistance);
    }

    void MoveCharacterTo(Vector3Int cellPosition)
    {
        if (selectedCharacter != null)
        {
            Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
            locationData.UpdatePlayerPosition(cellPosition, worldPosition);
            Vector3 offset = new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            worldPosition += offset;
            selectedCharacter.transform.position = worldPosition;
            Debug.Log("Moved character to: " + worldPosition + " (Cell Position: " + cellPosition + ")");

            // Update current cell position
            currentCellPosition = cellPosition;
            Debug.Log("Updated Player current cell position after move: " + currentCellPosition);

            // Notify the Director of the player's new position
            if (director != null)
            {
                director.UpdatePlayerGridPosition(currentCellPosition);
            }

            // Deselect character after moving
            selectedCharacter = null;
        }
    }

    void ShowMovementRange()
    {
        overlayTilemap.ClearAllTiles();
        Vector3Int characterPosition = tilemap.WorldToCell(selectedCharacter.transform.position);

        for (int x = -maxMovementDistance; x <= maxMovementDistance; x++)
        {
            for (int y = -maxMovementDistance; y <= maxMovementDistance; y++)
            {
                Vector3Int cellPosition = new Vector3Int(characterPosition.x + x, characterPosition.y + y, 0);

                if (IsFloorTile(cellPosition) && IsWithinMovementRange(cellPosition))
                {
                    overlayTilemap.SetTile(cellPosition, overlayTile);
                }
            }
        }
    }

    void ClearMovementRange()
    {
        overlayTilemap.ClearAllTiles();
    }

    // Expose current cell position
    public Vector3Int GetCurrentCellPosition()
    {
        // Debug.Log("Exposing Player current cell position: " + currentCellPosition);
        return currentCellPosition;
    }
}