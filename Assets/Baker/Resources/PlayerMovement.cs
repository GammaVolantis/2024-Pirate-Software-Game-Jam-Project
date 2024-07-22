using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    private GameObject selectedCharacter;
    public Tilemap tilemap;
    public Tilemap overlayTilemap; // Secondary Tilemap for overlays
    public RuleTile overlayTile; // RuleTile to use for overlay
    public int maxMovementDistance = 2; // Maximum movement distance setting

    void Update()
    {
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
        if (dx <= maxMovementDistance && dy <= maxMovementDistance && (dx + dy <= maxMovementDistance))
        {
            return true;
        }
        return false;
    }

    void MoveCharacterTo(Vector3Int cellPosition)
    {
        if (selectedCharacter != null)
        {
            Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
            Vector3 offset = new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            worldPosition += offset;
            selectedCharacter.transform.position = worldPosition;
            Debug.Log("Moved character to: " + worldPosition);

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
}
