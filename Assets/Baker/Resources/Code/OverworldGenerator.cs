using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverworldGenerator : MonoBehaviour
{
    //public static OverworldGenerator Instance { get; private set; }

    public Tilemap OverworldTilemap;
    public GameObject PathPrefab; // Prefab with LineRenderer for drawing paths
    public GameObject PlayerPrefab; // Prefab for the player
    public OverworldData overworldData; // The overworldData scriptable object

    [System.Serializable]
    public class BiomeTile
    {
        public BiomeType biomeType;
        public RuleTile tile;
    }

    public enum BiomeType
    {
        Plains,
        Forest,
        //Mountain
    }

    public enum EncounterType
    {
        PlainsEncounter,
        ForestEncounter,
        //MountainEncounter
    }

    [System.Serializable]
    public class EncounterTile
    {
        public EncounterType encounterType;
        public TileBase tile;
    }

    [SerializeField]
    public List<BiomeTile> biomeTiles = new List<BiomeTile>();
    [SerializeField]
    public List<EncounterTile> encounterTiles = new List<EncounterTile>();

    public int mapWidth = 100;
    public int mapHeight = 20;
    public float noiseScale = 0.1f;
    public int minDistance = 3;

    private float offsetX;
    private float offsetY;

    // Data structures to store the biomes and encounters
    private BiomeType[,] biomeGrid;
    private List<Vector3Int> encounterPositions;
    private Dictionary<Vector3Int, List<Vector3Int>> encounterConnections;
    private Dictionary<(Vector3Int, Vector3Int), GameObject> pathObjects;

    //private void Awake()
    //{
        //if (Instance == null)
        //{
            //Instance = this;
            //DontDestroyOnLoad(gameObject); // Ensure the object isn't destroyed when loading new scenes
        //}
        //else
        //{
            //Debug.LogWarning("Another instance of OverworldGenerator detected and destroyed");
            //Destroy(gameObject);
            //return;
        //}
    //}

    void Start()
    {
        overworldData = Resources.Load<OverworldData>("OverWorldData");
        Debug.Log(overworldData.GetHasData());
        if (overworldData.GetHasData())
        {
            Debug.Log("Loading existing world state");
            LoadWorldState();

            // Spawn player at the furthest left encounter location
            Vector3 playerStartPos = overworldData.GetPlayerPosition();
            Debug.Log($"Spawning player at position: {playerStartPos}");
            GameObject player = Instantiate(PlayerPrefab, playerStartPos, Quaternion.identity);
            if (player == null)
            {
                Debug.LogError("Failed to instantiate player!");
            }

            // Ensure player is rendered on top
            SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.sortingOrder = 10; // Higher sorting order than the lines
            }
        }
        else
        {
            Debug.Log("Starting Overworld Generation");

            // Generate random offsets for Perlin noise
            offsetX = Random.Range(0f, 10000f);
            offsetY = Random.Range(0f, 10000f);

            // Initialize data structures
            biomeGrid = new BiomeType[mapHeight, mapWidth];
            encounterPositions = new List<Vector3Int>();
            encounterConnections = new Dictionary<Vector3Int, List<Vector3Int>>();
            pathObjects = new Dictionary<(Vector3Int, Vector3Int), GameObject>();

            GenerateOverworld();

            // Debug logs
            if (PlayerPrefab == null) Debug.LogError("PlayerPrefab is not assigned!");
            if (OverworldTilemap == null) Debug.LogError("OverworldTilemap is not assigned!");

            // Spawn player at the furthest left encounter location
            Vector3Int playerStartPos = GetFurthestLeftEncounterPosition();
            Debug.Log($"Spawning player at position: {playerStartPos}");
            GameObject player = Instantiate(PlayerPrefab, OverworldTilemap.GetCellCenterWorld(playerStartPos), Quaternion.identity);
            if (player == null)
            {
                Debug.LogError("Failed to instantiate player!");
            }

            // Ensure player is rendered on top
            SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.sortingOrder = 10; // Higher sorting order than the lines
            }
        }
    }

    void GenerateOverworld()
    {
        Debug.Log("Generating Overworld");

        GenerateBiomes();
        PlaceBiomeTiles();

        PlaceEncounterTiles(BiomeType.Plains, EncounterType.PlainsEncounter, 10);
        PlaceEncounterTiles(BiomeType.Forest, EncounterType.ForestEncounter, 10);
        //PlaceEncounterTiles(BiomeType.Mountain, EncounterType.MountainEncounter, 10);

        CreatePathways();

        OverworldTilemap.RefreshAllTiles(); // Refresh the Tilemap to ensure changes are applied
        Debug.Log("Overworld generation complete.");

        SaveWorldState();
    }

    void GenerateBiomes()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float xCoord = x * noiseScale + offsetX;
                float yCoord = y * noiseScale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                if (sample < 0.25f)
                {
                    biomeGrid[y, x] = BiomeType.Plains;
                }
                else //if (sample < 0.5f)
                {
                    biomeGrid[y, x] = BiomeType.Forest;
                }
                //else
                //{
                //    biomeGrid[y, x] = BiomeType.Mountain;
                //}
            }
        }
    }

    void PlaceBiomeTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x - mapWidth / 2, y - mapHeight / 2, 0);
                RuleTile tile = GetTileForBiome(biomeGrid[y, x]);
                if (tile == null)
                {
                    Debug.LogError($"Tile is null for biome at position {position} with biome type {biomeGrid[y, x]}");
                }
                OverworldTilemap.SetTile(position, tile);
                Debug.Log($"Setting tile at {position} with tile for biome {biomeGrid[y, x]}");
            }
        }
    }

    void PlaceEncounterTiles(BiomeType biomeType, EncounterType encounterType, int count)
    {
        List<int> xValues = new List<int>();
        for (int x = 4; x < mapWidth - 4; x++) // Values from 4 to mapWidth-4 to avoid the edges
        {
            xValues.Add(x);
        }

        TileBase encounterTile = encounterTiles.Find(e => e.encounterType == encounterType).tile;
        if (encounterTile == null)
        {
            Debug.LogError($"Encounter tile is null for encounter type {encounterType}");
            return;
        }

        while (count > 0 && xValues.Count > 0)
        {
            int randomIndex = Random.Range(0, xValues.Count);
            int x = xValues[randomIndex];
            int y = Random.Range(3, mapHeight - 3);

            Vector3Int position = new Vector3Int(x - mapWidth / 2, y - mapHeight / 2, 0);

            if (IsValidEncounterPosition(position))
            {
                OverworldTilemap.SetTile(position, encounterTile);
                AddEncounterPosition(position);
                xValues.RemoveAt(randomIndex);
                count--;
            }
        }
    }

    bool IsValidEncounterPosition(Vector3Int position)
    {
        foreach (Vector3Int placed in encounterPositions)
        {
            if (Vector3Int.Distance(position, placed) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    void AddEncounterPosition(Vector3Int position)
    {
        encounterPositions.Add(position);
        encounterConnections[position] = new List<Vector3Int>();
    }

    void CreatePathways()
    {
        foreach (Vector3Int encounter in encounterPositions)
        {
            int connections = Random.Range(1, 3); // 1 to 3 connections
            List<Vector3Int> potentialConnections = GetEncountersToRight(encounter);

            // Sort potential connections by distance (left to right)
            potentialConnections.Sort((a, b) => Vector3Int.Distance(encounter, a).CompareTo(Vector3Int.Distance(encounter, b)));

            int connectionCount = Mathf.Min(connections, potentialConnections.Count);
            for (int i = 0; i < connectionCount; i++)
            {
                Vector3Int targetEncounter = potentialConnections[i];

                if (IsValidPath(encounter, targetEncounter))
                {
                    encounterConnections[encounter].Add(targetEncounter);
                    DrawPath(encounter, targetEncounter);
                }
            }
        }
    }

    List<Vector3Int> GetEncountersToRight(Vector3Int encounter)
    {
        List<Vector3Int> encountersToRight = new List<Vector3Int>();

        foreach (Vector3Int pos in encounterPositions)
        {
            if (pos.x > encounter.x)
            {
                encountersToRight.Add(pos);
            }
        }

        return encountersToRight;
    }

    bool IsValidPath(Vector3Int start, Vector3Int end)
    {
        return true;
    }

    void DrawPath(Vector3Int start, Vector3Int end)
    {
        // Calculate the center of the start and end tiles
        Vector3 startWorldPos = OverworldTilemap.GetCellCenterWorld(start);
        Vector3 endWorldPos = OverworldTilemap.GetCellCenterWorld(end);

        // Instantiate the PathPrefab at the start position
        GameObject pathObject = Instantiate(PathPrefab, startWorldPos, Quaternion.identity);
        LineRenderer lineRenderer = pathObject.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startWorldPos);
            lineRenderer.SetPosition(1, endWorldPos);
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
            lineRenderer.sortingOrder = 5; // Ensure this is above the Tilemap's order
        }
        else
        {
            Debug.LogError("LineRenderer component is missing on PathPrefab");
        }

        // Store the path object for later removal
        pathObjects[(start, end)] = pathObject;
        //pathObjects[(end, start)] = pathObject; // Ensure we can find it regardless of direction
    }

    public void DestroyPath(Vector3Int start, Vector3Int end)
    {
        if (pathObjects.ContainsKey((start, end)))
        {
            Destroy(pathObjects[(start, end)]);
            pathObjects.Remove((start, end));
            //pathObjects.Remove((end, start));
        }
        else
        {
            Debug.LogError($"Path object not found between {start} and {end}");
        }
    }

    public void DestroyPathsToLeft(Vector3Int playerPosition)
    {
        List<(Vector3Int, Vector3Int)> pathsToRemove = new List<(Vector3Int, Vector3Int)>();

        foreach (var path in pathObjects.Keys)
        {
            if (path.Item1.x < playerPosition.x || path.Item2.x < playerPosition.x)
            {
                pathsToRemove.Add(path);
            }
        }

        foreach (var path in pathsToRemove)
        {
            DestroyPath(path.Item1, path.Item2);
        }
    }

    public Vector3Int GetFurthestLeftEncounterPosition()
    {
        if (encounterPositions == null || encounterPositions.Count == 0)
        {
            Debug.LogError("Encounter positions list is null or empty");
            return Vector3Int.zero;
        }

        // Sort encounters by x-coordinate to find the furthest left
        encounterPositions.Sort((a, b) => a.x.CompareTo(b.x));
        return encounterPositions[0];
    }

    public bool CanMoveTo(Vector3Int from, Vector3Int to)
    {
        if (encounterConnections == null)
        {
            Debug.LogError("Encounter connections dictionary is null");
            return false;
        }

        if (!encounterConnections.ContainsKey(from))
        {
            Debug.LogError($"No connections found from position {from}");
            return false;
        }

        return encounterConnections[from].Contains(to);
    }

    RuleTile GetTileForBiome(BiomeType biomeType)
    {
        BiomeTile biomeTile = biomeTiles.Find(b => b.biomeType == biomeType);
        if (biomeTile == null)
        {
            Debug.LogError($"Biome tile is null for biome type {biomeType}");
            return null;
        }
        return biomeTile.tile;
    }

    public void SaveWorldState()
    {
        Vector3 playerPosition = Vector3.zero;
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerPosition = playerMovement.transform.position;
        }
        else
        {
            Debug.LogError("PlayerMovement object not found!");
        }

        if (biomeGrid == null)
        {
            Debug.LogError("biomeGrid is null!");
        }
        if (encounterPositions == null)
        {
            Debug.LogError("encounterPositions is null!");
        }
        if (encounterConnections == null)
        {
            Debug.LogError("encounterConnections is null!");
        }
        if (pathObjects == null)
        {
            Debug.LogError("pathObjects is null!");
        }

        overworldData.SetGlobalMapValues(
            biomeGrid,
            encounterPositions,
            encounterConnections,
            pathObjects,
            playerPosition
        );
    }

    void LoadWorldState()
    {
        biomeGrid = overworldData.GetBiomes();
        encounterPositions = overworldData.GetEncounterPositions();
        encounterConnections = overworldData.GetEncounterConnections();
        pathObjects = overworldData.GetPathObjects();

        // Restore the tilemap
        PlaceBiomeTiles();

        // Restore the encounters
        foreach (var encounterPosition in encounterPositions)
        {
            var encounterType = GetEncounterTypeAtPosition(encounterPosition);
            var encounterTile = encounterTiles.Find(e => e.encounterType == encounterType).tile;
            OverworldTilemap.SetTile(encounterPosition, encounterTile);
        }

        // Restore the paths

        Dictionary<(Vector3Int, Vector3Int), GameObject> tempObjects = overworldData.GetPathObjects(); ;
        var keysCopy = new List<(Vector3Int, Vector3Int)>(tempObjects.Keys);
        foreach (var path in keysCopy)
        {
            DrawPath(path.Item1, path.Item2);
        }

        OverworldTilemap.RefreshAllTiles();
    }

    public EncounterType GetEncounterTypeAtPosition(Vector3Int position)
    {
        TileBase tile = OverworldTilemap.GetTile(position);
        foreach (var encounterTile in encounterTiles)
        {
            if (encounterTile.tile == tile)
            {
                return encounterTile.encounterType;
            }
        }
        return EncounterType.PlainsEncounter; // Default encounter type
    }
}
