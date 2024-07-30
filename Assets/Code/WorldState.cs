using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public OverworldGenerator.BiomeType[,] biomeGrid;
    public List<Vector3Int> encounterPositions;
    public Dictionary<Vector3Int, List<Vector3Int>> encounterConnections;
    public Dictionary<(Vector3Int, Vector3Int), GameObject> pathObjects;
}
