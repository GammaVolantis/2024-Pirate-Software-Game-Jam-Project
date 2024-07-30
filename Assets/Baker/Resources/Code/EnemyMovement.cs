using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public KeyCode moveKey = KeyCode.E; // Key to initiate enemy movement
    public int maxHorizontalVerticalDistance = 2;
    public int maxDiagonalDistance = 1;

    private Tilemap tilemap;
    private Director director;
    private LocationData locationData;
    private PlayerStats playerStats;
    private List<GameObject> enemies;
    private bool initialized = false;
    private bool isMoving = false;
    private int enIndex;

    public float timer = 3;

    void Start()
    {
        // Initialize references
        locationData = Resources.Load<LocationData>("AllLocationInformation");
        playerStats = Resources.Load<PlayerStats>("PlayerStats");
        Initialize();
    }

    void Initialize()
    {
        if (director == null)
        {
            director = FindObjectOfType<Director>();
        }

        if (director != null)
        {
            tilemap = director.tilemap;
            // Initialize enemies list
            enemies = new List<GameObject>();
            foreach (Transform enemy in director.enemiesParent)
            {
                enemies.Add(enemy.gameObject);
            }
            initialized = true;
        }
        else
        {
            Debug.LogError("Director instance not found!");
        }
    }

    void Update()
    {
        if (!initialized)
        {
            Initialize();
        }

        //replace the KeyDown with a value passed from the locationsDataScript
        int iter = 0;
        //int currentIter = 0;
        foreach (GameObject enem in locationData.GetAllEnemyObjects()) 
        {
            if (this.gameObject == enem)
            {
                enIndex = iter;
            }
            iter++;
        }
        if (locationData.CheckEnemyMoveState(enIndex) && !isMoving)
        {
            timer = 3;
            Debug.Log("Starting enemy movement.");
            //StartCoroutine(MoveEnemiesWithDelay());
            MoveEnemy();
            Vector3 enemyPos = locationData.GetEnemyVirtual(enIndex);
            Vector3 playerPos = locationData.GetPlayerVirtual();
            if (Mathf.Floor(Mathf.Sqrt(Mathf.Pow(enemyPos.x - playerPos.x, 2) + Mathf.Pow(enemyPos.y - playerPos.y, 2))) <= 1)
            {
                //Damage the player
                playerStats.newHealthValue(-10);
            }
            locationData.SetEnemyMoveState(enIndex);

        }
    }

    void MoveEnemy()
    {
        Vector3Int enemyGridPosition = tilemap.WorldToCell(transform.position);
        Vector3Int targetPosition = FindBestMovePosition(enemyGridPosition);
        Debug.Log(targetPosition.ToString());

        if (targetPosition != enemyGridPosition && targetPosition != director.GetPlayerGridPosition())
        {
            Vector3 worldPosition = tilemap.CellToWorld(targetPosition) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            transform.position = worldPosition;
            //int enemyIndex = director.enemies.IndexOf(gameObject);
            director.UpdateEnemyGridPosition(enIndex, targetPosition);
            locationData.UpdateEnemyLocation(targetPosition, worldPosition, enIndex);
            Debug.Log($"Updated enemy grid position at index {enIndex}: {targetPosition}");
            foreach (var enemyPos in director.GetEnemyGridPositions()) {
                Debug.Log($"Enemy Loc at:: {enemyPos}");
            }
        }
    }

    Vector3Int FindBestMovePosition(Vector3Int start)
    {
        Vector3Int target = director.GetPlayerGridPosition();
        List<Vector3Int> path = AStarPathfinding(start, target);

        if (path.Count > 1)
        {
            // Ensure the next step is within movement constraints
            Vector3Int nextStep = path[1];
            if ((nextStep.x == start.x || nextStep.y == start.y) && path.Count > 2)
            {
                Vector3Int secondStep = path[2];
                if (IsWithinMoveRange(start, nextStep))
                {
                    return secondStep;
                }
            }

            if (IsWithinMoveRange(start, nextStep))
            {
                return nextStep;
            }
        }

        return start; // If no valid move is found, stay in the current position
    }

    List<Vector3Int> AStarPathfinding(Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> openSet = new List<Vector3Int> { start };
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> gScore = new Dictionary<Vector3Int, int> { { start, 0 } };
        Dictionary<Vector3Int, int> fScore = new Dictionary<Vector3Int, int> { { start, HeuristicCostEstimate(start, goal) } };

        while (openSet.Count > 0)
        {
            Vector3Int current = GetLowestFScore(openSet, fScore);

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGScore = gScore[current] + Distance(current, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);
            }
        }

        return new List<Vector3Int>(); // Return an empty path if no path is found
    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> totalPath = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    Vector3Int GetLowestFScore(List<Vector3Int> openSet, Dictionary<Vector3Int, int> fScore)
    {
        Vector3Int lowest = openSet[0];
        foreach (Vector3Int node in openSet)
        {
            if (fScore.ContainsKey(node) && fScore[node] < fScore[lowest])
            {
                lowest = node;
            }
        }
        return lowest;
    }

    int HeuristicCostEstimate(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    int Distance(Vector3Int a, Vector3Int b)
    {
        if (Mathf.Abs(a.x - b.x) <= 1 && Mathf.Abs(a.y - b.y) <= 1)
        {
            return 1;
        }
        return 2;
    }

    List<Vector3Int> GetNeighbors(Vector3Int current)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int[] possibleMoves = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, 1, 0)
        };

        foreach (Vector3Int move in possibleMoves)
        {
            Vector3Int neighbor = current + move;
            if (IsValidFloorTile(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    bool IsValidFloorTile(Vector3Int cellPosition)
    {
        if (RoomGenerator.Instance != null)
        {
            int[,] grid = RoomGenerator.Instance.baseGrid;
            int row = cellPosition.y - tilemap.cellBounds.yMin;
            int col = cellPosition.x - tilemap.cellBounds.xMin;

            foreach (var enemyPos in director.GetEnemyGridPositions())
            {
                Debug.Log("Checking Position: " + enemyPos);
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

    bool IsWithinMoveRange(Vector3Int start, Vector3Int target)
    {
        int dx = Mathf.Abs(start.x - target.x);
        int dy = Mathf.Abs(start.y - target.y);

        return (dx <= maxHorizontalVerticalDistance && dy == 0) ||
               (dy <= maxHorizontalVerticalDistance && dx == 0) ||
               (dx == dy && dx <= maxDiagonalDistance);
    }

    public void SetEnemyIndexVal(int index)
    {
        enIndex = index;
    }
}