using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    //Creates a singleton instance that will allow access to the baseGrid variable across scripts - will update when the script reruns
    public static RoomGenerator Instance { get; private set; }

    public Tilemap FloorTilemap; // Public to select a tilemap to work with

    [System.Serializable]
    public class WeightedTile // This creates a weight system for the appearance of the tiles for a category. Weight is a combination of individual elements divided by the tal sum. So 3 elements with a 2, 3, and 5 (sum of 10) means 5 will have 50% chance (5/10) of selection
    {
        public RuleTile tile; // The tile to be used
        public int weight; // The weight determining the probability of this tile being chosen as expressed by a single integer
    }

    // This enumerates the environemtn types for encounter/instance generation. When going through the Environemtn Settings List, it will create a dropdown to associate the time mapping to the environment.
    public enum EnvironmentType
    {
        Dungeon,
        Forest,
        Cave,
        Volcano
    }

    // This is a grouping of lists for wall tiles. The Proc gen will take each Rule Tile in the list and then assign it to an appropriate position. Darkness is a single tile, so no list needed. Tried to name variables meaningfully - corner is when the angle points away from room and bend is when it points into room
    [System.Serializable]
    public class EnvironmentTiles
    {
        public List<WeightedTile> bottomLeftCornerTiles; 
        public List<WeightedTile> bottomRightCornerTiles;
        public List<WeightedTile> upperLeftCornerTiles;
        public List<WeightedTile> upperRightCornerTiles;
        public List<WeightedTile> lowerWallTiles;
        public List<WeightedTile> upperWallTiles;
        public List<WeightedTile> leftWallTiles;
        public List<WeightedTile> rightWallTiles;
        public List<WeightedTile> innerBendLowerLeftTiles;
        public List<WeightedTile> innerBendLowerRightTiles;
        public List<WeightedTile> innerBendUpperLeftTiles;
        public List<WeightedTile> innerBendUpperRightTiles;
        public WeightedTile darknessTile; 
        public List<WeightedTile> floorTiles;
    }

    // Class to hold settings for each environment
    [System.Serializable]
    public class EnvironmentSettings
    {
        public EnvironmentType environmentType;
        public EnvironmentTiles environmentTiles;
        public bool useGameOfLife; // This is a placeholder to allow Game of Life generation; however, selecting it will break because I am not done with the generation code. GoL will create a blobish shape good for caves
    }

    public List<EnvironmentSettings> environmentSettingsList; // This holds all the tile and weight settings for each environment
    private EnvironmentSettings currentEnvironmentSettings; // The current environment settings

    public int roomWidth = 16; // The width of the room
    public int roomHeight = 9; // The height of the room

    public EnvironmentType selectedEnvironment; // The selected environment type that will be generated on run (used in SetEnvironment below)

    // The final grid that holds the values for which grid associates to which tile mapping. "0" is darkness tiles, "1" is wall tiles (all walls - types are handled elsewhere), and "2" is floor tiles (same note as wall tiles)
    public int[,] baseGrid;

    // Ensures only one instance is ever called at a time
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetEnvironment(selectedEnvironment); // Sets the environment based on the selected type (see SetEnvironment below)
        GenerateRoom(); // The main room generation function
    }

    // Sets the current environment settings based on the selected environment type
    void SetEnvironment(EnvironmentType environmentType)
    {
        currentEnvironmentSettings = environmentSettingsList.Find(env => env.environmentType == environmentType);
        if (currentEnvironmentSettings == null)
        {
            Debug.LogError("Environment settings not found for: " + environmentType);
        }
    }

    // Main room generation function
    void GenerateRoom()
    {
        if (currentEnvironmentSettings == null) return; // First checks to see if an environment was set and stops if it was not

        baseGrid = new int[roomHeight, roomWidth]; // Initializes the grid used for holding the tile type information

        if (currentEnvironmentSettings.useGameOfLife)
        {
            GenerateCave(baseGrid); // Generates room using Game of Life algorithm (for caves) !!! This will not work right now!!!
        }
        else
        {
            GenerateDungeon(baseGrid); // Generates dungeons
        }

        // This script uses flood fill to find all floor tiles from the middle point (which is always a floor piece) - This is meant to assist in removing completely inaccessbile room
        int centerX = roomWidth / 2;
        int centerY = roomHeight / 2;
        List<(int, int)> contiguousFloors = FloodFill(baseGrid, centerX, centerY);

        // Any floor element ("2") not in the contiguousFloors list is converted to a darkness element ("0")
        for (int row = 0; row < roomHeight; row++)
        {
            for (int col = 0; col < roomWidth; col++)
            {
                if (baseGrid[row, col] == 2 && !contiguousFloors.Contains((col, row)))
                {
                    baseGrid[row, col] = 0;
                }
            }
        }

        // Once orphaned rooms are converted to darkness tile, I need to convert lingering walls
        ConvertOrphanedWalls(baseGrid);

        // Initializes the Tilemap using the grid - begins by setting everything to darkness tiles
        RuleTile[,] tileGrid = new RuleTile[roomWidth, roomHeight];
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                tileGrid[x, y] = currentEnvironmentSettings.environmentTiles.darknessTile.tile; // In the beginning, there was only darkness...
            }
        }

        // This is where the grid is leveraged to convert the tiles
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (baseGrid[y, x] == 2) // Floor tiles
                {
                    tileGrid[x, y] = GetRandomTile(currentEnvironmentSettings.environmentTiles.floorTiles); // Randomly selects tile (with weight)
                }
                else if (baseGrid[y, x] == 1) // Wall tiles
                {
                    tileGrid[x, y] = GetWallTile(x, y, baseGrid); // Randomly selects tile, but based on weight and neighboring factors
                }
            }
        }

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                FloorTilemap.SetTile(new Vector3Int(x - roomWidth / 2, y - roomHeight / 2, 0), tileGrid[x, y]); // This set the Tilemap graphic
            }
        }
    }

    // Generates dungeon
    void GenerateDungeon(int[,] grid)
    {
        // Initialize the grid with '1's
        for (int row = 0; row < roomHeight; row++)
        {
            for (int col = 0; col < roomWidth; col++)
            {
                grid[row, col] = 1;
            }
        }

        // Generates random rectangles that begin at an edge (top, bottom, left, or right)
        int iterations = Random.Range(0, 5);
        List<(int, int, int, int)> boxes = new List<(int, int, int, int)>();

        for (int i = 0; i < iterations; i++)
        {
            int width = Random.Range(1, 5);
            int height = Random.Range(1, 3);
            int start = Random.Range(1, 5);
            int posx = 0, posy = 0;

            switch (start)
            {
                case 1: // Left side
                    posx = 0;
                    posy = Random.Range(0, roomHeight - height);
                    break;
                case 2: // Right side
                    posx = roomWidth - width;
                    posy = Random.Range(0, roomHeight - height);
                    break;
                case 3: // Bottom side
                    posx = Random.Range(0, roomWidth - width);
                    posy = 0;
                    break;
                case 4: // Top side
                    posx = Random.Range(0, roomWidth - width);
                    posy = roomHeight - height;
                    break;
            }

            boxes.Add((posy, posy + height, posx, posx + width));
        }

        // Updates the grid with '0's based on the overlaps of the rectangles (0 is darkness type)
        foreach (var box in boxes)
        {
            for (int row = box.Item1; row < box.Item2; row++)
            {
                for (int col = box.Item3; col < box.Item4; col++)
                {
                    grid[row, col] = 0;
                }
            }
        }

        // Figures out which tiles are '2's (floor pieces)
        for (int row = 1; row < roomHeight - 1; row++)
        {
            for (int col = 1; col < roomWidth - 1; col++)
            {
                if (grid[row, col] == 1)
                {
                    bool isFloor = true;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (grid[row + i, col + j] == 0)
                            {
                                isFloor = false;
                                break;
                            }
                        }
                        if (!isFloor) break;
                    }
                    if (isFloor) grid[row, col] = 2;
                }
            }
        }
    }

    // Game of Life cave generation algorithm !!!!! DO NOT USE!!!!
    void GenerateCave(int[,] grid)
    {
        // Initialize the grid with random values
        for (int row = 0; row < roomHeight; row++)
        {
            for (int col = 0; col < roomWidth; col++)
            {
                grid[row, col] = Random.Range(0, 2); 
            }
        }

        // Game of Life rules
        for (int iteration = 0; iteration < 5; iteration++)
        {
            int[,] newGrid = new int[roomHeight, roomWidth];
            for (int row = 0; row < roomHeight; row++)
            {
                for (int col = 0; col < roomWidth; col++)
                {
                    int neighbors = CountAliveNeighbors(grid, row, col);
                    if (grid[row, col] == 1)
                    {
                        newGrid[row, col] = (neighbors < 2 || neighbors > 3) ? 0 : 1;
                    }
                    else
                    {
                        newGrid[row, col] = (neighbors == 3) ? 1 : 0;
                    }
                }
            }
            grid = newGrid;
        }

        // Convert '1's to walls and '0's to floor
        for (int row = 0; row < roomHeight; row++)
        {
            for (int col = 0; col < roomWidth; col++)
            {
                grid[row, col] = (grid[row, col] == 1) ? 1 : 2;
            }
        }
    }

    // Count the number of alive neighbors for a cell in the Game of Life algorithm
    int CountAliveNeighbors(int[,] grid, int row, int col)
    {
        int aliveCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int newRow = row + i;
                int newCol = col + j;
                if (newRow >= 0 && newRow < roomHeight && newCol >= 0 && newCol < roomWidth)
                {
                    aliveCount += grid[newRow, newCol];
                }
            }
        }
        return aliveCount;
    }

    // Flood fill algorithm to find all contiguous floor pieces
    List<(int, int)> FloodFill(int[,] grid, int startX, int startY)
    {
        List<(int, int)> contiguousFloors = new List<(int, int)>();
        if (grid[startY, startX] != 2) return contiguousFloors;

        bool[,] visited = new bool[roomHeight, roomWidth];
        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();
            if (x < 0 || x >= roomWidth || y < 0 || y >= roomHeight || visited[y, x] || grid[y, x] != 2)
                continue;

            visited[y, x] = true;
            contiguousFloors.Add((x, y));

            stack.Push((x + 1, y));
            stack.Push((x - 1, y));
            stack.Push((x, y + 1));
            stack.Push((x, y - 1));
        }

        return contiguousFloors;
    }

    // Converts orphaned walls to darkness 
    void ConvertOrphanedWalls(int[,] grid)
    {
        for (int row = 0; row < roomHeight; row++)
        {
            for (int col = 0; col < roomWidth; col++)
            {
                if (grid[row, col] == 1)
                {
                    bool isOrphan = true;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int newRow = row + i;
                            int newCol = col + j;
                            if (newRow >= 0 && newRow < roomHeight && newCol >= 0 && newCol < roomWidth && grid[newRow, newCol] == 2)
                            {
                                isOrphan = false;
                                break;
                            }
                        }
                        if (!isOrphan) break;
                    }
                    if (isOrphan) grid[row, col] = 0;
                }
            }
        }
    }

    // Selects a random tile based on weights
    RuleTile GetRandomTile(List<WeightedTile> weightedTiles)
    {
        int totalWeight = 0;
        foreach (var weightedTile in weightedTiles)
        {
            totalWeight += weightedTile.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var weightedTile in weightedTiles)
        {
            if (randomWeight < weightedTile.weight)
            {
                return weightedTile.tile;
            }
            randomWeight -= weightedTile.weight;
        }

        return null; // This should never happen if weights are properly set
    }

    // Gets the right wall tile based on the surrounding tiles
    RuleTile GetWallTile(int x, int y, int[,] baseGrid)
    {
        bool hasDarknessAbove = y == roomHeight - 1 || baseGrid[y + 1, x] == 0;
        bool hasDarknessBelow = y == 0 || baseGrid[y - 1, x] == 0;
        bool hasDarknessLeft = x == 0 || baseGrid[y, x - 1] == 0;
        bool hasDarknessRight = x == roomWidth - 1 || baseGrid[y, x + 1] == 0;

        // Determine the appropriate wall or corner tile
        if (hasDarknessAbove && hasDarknessLeft) return GetRandomTile(currentEnvironmentSettings.environmentTiles.upperLeftCornerTiles);
        if (hasDarknessAbove && hasDarknessRight) return GetRandomTile(currentEnvironmentSettings.environmentTiles.upperRightCornerTiles);
        if (hasDarknessBelow && hasDarknessLeft) return GetRandomTile(currentEnvironmentSettings.environmentTiles.bottomLeftCornerTiles);
        if (hasDarknessBelow && hasDarknessRight) return GetRandomTile(currentEnvironmentSettings.environmentTiles.bottomRightCornerTiles);
        if (hasDarknessAbove) return GetRandomTile(currentEnvironmentSettings.environmentTiles.upperWallTiles);
        if (hasDarknessBelow) return GetRandomTile(currentEnvironmentSettings.environmentTiles.lowerWallTiles);
        if (hasDarknessLeft) return GetRandomTile(currentEnvironmentSettings.environmentTiles.leftWallTiles);
        if (hasDarknessRight) return GetRandomTile(currentEnvironmentSettings.environmentTiles.rightWallTiles);

        // Inner bends
        if (y > 0 && x > 0 && baseGrid[y - 1, x - 1] == 0 && baseGrid[y, x - 1] != 0 && baseGrid[y - 1, x] != 0)
            return GetRandomTile(currentEnvironmentSettings.environmentTiles.innerBendLowerLeftTiles);
        if (y > 0 && x < roomWidth - 1 && baseGrid[y - 1, x + 1] == 0 && baseGrid[y, x + 1] != 0 && baseGrid[y - 1, x] != 0)
            return GetRandomTile(currentEnvironmentSettings.environmentTiles.innerBendLowerRightTiles);
        if (y < roomHeight - 1 && x > 0 && baseGrid[y + 1, x - 1] == 0 && baseGrid[y, x - 1] != 0 && baseGrid[y + 1, x] != 0)
            return GetRandomTile(currentEnvironmentSettings.environmentTiles.innerBendUpperLeftTiles);
        if (y < roomHeight - 1 && x < roomWidth - 1 && baseGrid[y + 1, x + 1] == 0 && baseGrid[y, x + 1] != 0 && baseGrid[y + 1, x] != 0)
            return GetRandomTile(currentEnvironmentSettings.environmentTiles.innerBendUpperRightTiles);

        return null;
    }

    void Update()
    {
        
    }
}
