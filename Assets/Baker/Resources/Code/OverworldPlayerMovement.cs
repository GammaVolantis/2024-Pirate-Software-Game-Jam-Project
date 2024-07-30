using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using static RoomGenerator;

public class OverworldPlayerMovement : MonoBehaviour
{
    private OverworldGenerator overworldGenerator;
    private Tilemap overworldTilemap;
    private Vector3Int currentPosition;
    private GameObject particleEffect;

    void Start()
    {
        // Find the OverworldGenerator and Tilemap in the scene
        overworldGenerator = FindObjectOfType<OverworldGenerator>();
        if (overworldGenerator == null)
        {
            Debug.LogError("OverworldGenerator not found!");
            return;
        }

        overworldTilemap = overworldGenerator.OverworldTilemap;
        if (overworldTilemap == null)
        {
            Debug.LogError("OverworldTilemap not found!");
            return;
        }

        // Initialize player at the first encounter location on the left
        currentPosition = overworldGenerator.GetFurthestLeftEncounterPosition();
        transform.position = overworldTilemap.GetCellCenterWorld(currentPosition);
        Debug.Log($"Player initialized at position: {currentPosition}");

        // Ensure player is rendered on top
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 10; // Higher sorting order than the lines
        }

        

        // Set the camera follow target
        Camera.main.GetComponent<PlayerFollow>().player = transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickPosition = overworldTilemap.WorldToCell(mouseWorldPos);

            if (overworldGenerator.CanMoveTo(currentPosition, clickPosition))
            {
                MovePlayer(clickPosition);
            }
        }
    }

    void MovePlayer(Vector3Int newPosition)
    {
        Debug.Log($"Moving player from {currentPosition} to {newPosition}");

        // Destroy the line behind the player
        overworldGenerator.DestroyPath(currentPosition, newPosition);

        // Destroy paths to the left of the new position
        overworldGenerator.DestroyPathsToLeft(newPosition);

        // Move the player to the new position
        currentPosition = newPosition;
        transform.position = overworldTilemap.GetCellCenterWorld(currentPosition);

        // Determine the encounter type based on the position
        OverworldGenerator.EncounterType encounterType = overworldGenerator.GetEncounterTypeAtPosition(newPosition);

        // Save the environment type before loading the new scene
        EnvironmentType environmentType = ConvertEncounterTypeToEnvironmentType(encounterType);
        PlayerPrefs.SetInt("EnvironmentType", (int)environmentType);
        PlayerPrefs.Save();

        Debug.Log($"Setting EnvironmentType to: {environmentType}");

        // Load the encounter scene
        LoadEncounterScene();
    }

    void LoadEncounterScene()
    {
        // Optionally, you can save the player's state or the current game state here
        SceneManager.LoadScene("WorkingCAPrototype");
    }

    EnvironmentType ConvertEncounterTypeToEnvironmentType(OverworldGenerator.EncounterType encounterType)
    {
        switch (encounterType)
        {
            case OverworldGenerator.EncounterType.PlainsEncounter:
                return EnvironmentType.Dungeon;
            case OverworldGenerator.EncounterType.ForestEncounter:
                return EnvironmentType.Forest;
            case OverworldGenerator.EncounterType.MountainEncounter:
                return EnvironmentType.Cave;
            case OverworldGenerator.EncounterType.VolcanoEncounter:
                return EnvironmentType.Volcano;
            default:
                return EnvironmentType.Dungeon; // Default to Dungeon if not matched
        }
    }
}
