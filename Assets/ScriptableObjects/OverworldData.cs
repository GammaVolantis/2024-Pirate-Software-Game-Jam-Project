using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "new OverworldData", menuName = "ScriptableObjects/OverworldData")]
public class OverworldData : ScriptableObject
{

    //Combat Scene Data to Pass
    //private OverworldGenerator.BiomeType[,] biomeGrid;
    //private List<Vector3Int> encounterPositions;
    //private Dictionary<Vector3Int, List<Vector3Int>> encounterConnections;
    //private Dictionary<(Vector3Int, Vector3Int), GameObject> pathObjects;

    public OverworldGenerator.BiomeType[,] biomeGrid;
    public List<Vector3Int> encounterPositions = new List<Vector3Int>();
    public Dictionary<Vector3Int, List<Vector3Int>> encounterConnections = new Dictionary<Vector3Int, List<Vector3Int>>();
    public Dictionary<(Vector3Int, Vector3Int), GameObject> pathObjects = new Dictionary<(Vector3Int, Vector3Int), GameObject>();
    public Vector3 playerPosition = new Vector3(-5.9f, -3.99f, 0f);

    //Player Overworld Position Data
    //private Vector3 playerPosition = new Vector3(-5.9f, -3.99f, 0f);

    public void OnEnable()
    {
        //Don't know if I need to put a check for the biomeGrid in here
        if (encounterConnections == null) encounterConnections = new Dictionary<Vector3Int,List<Vector3Int>>();
        if (pathObjects == null) pathObjects = new Dictionary<(Vector3Int, Vector3Int), GameObject>();
        if (encounterPositions == null) encounterPositions = new List<Vector3Int>();
    }

    //Overworld Combat Scene Objects to load
    public void SetGlobalMapValues(OverworldGenerator.BiomeType[,] biomes, List<Vector3Int> eps, Dictionary<Vector3Int, List<Vector3Int>> ecs, Dictionary<(Vector3Int, Vector3Int), GameObject> pos, Vector3 pp) 
    {
        biomeGrid = biomes;
        encounterPositions = new List<Vector3Int>(eps);
        encounterConnections = new Dictionary<Vector3Int, List<Vector3Int>>(ecs);
        pathObjects = new Dictionary<(Vector3Int, Vector3Int), GameObject>(pos);
        playerPosition = pp;
    }
    public OverworldGenerator.BiomeType[,] GetBiomes() 
    {
        return biomeGrid;
    }
    public List<Vector3Int> GetEncounterPositions() 
    {
        return encounterPositions;
    }
    public Dictionary<Vector3Int, List<Vector3Int>> GetEncounterConnections() 
    {
        return encounterConnections;
    }
    public Dictionary<(Vector3Int, Vector3Int), GameObject> GetPathObjects() 
    {
        return pathObjects;
    }
    public Vector3 GetPlayerPosition() 
    {
        return playerPosition;
    }
    //Combat Scene Data to Pass

    //Methods to pass data below


}
