using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LocationsData", menuName = "ScriptableObjects/LocationsData")]
public class LocationData : ScriptableObject
{
    private List<Vector3Int> enemiesVirtualLoc;
    private List<Vector3> enemiesRealLoc;
    private Vector3Int playerVirtualLoc;
    private Vector3 playerRealLoc;
    private List<GameObject> enemies;
    private GameObject player;

    //Enemies Methods
    public void AddEnemyLocations(Vector3Int evl, Vector3 erl, GameObject enemy) 
    { 
        enemiesVirtualLoc.Add(evl);
        enemiesRealLoc.Add(erl);
        enemies.Add(enemy);
    }
    public void UpdateEnemyLocation(Vector3Int evl, Vector3 erl, int loc) 
    {
        enemiesVirtualLoc[loc] = evl;
        enemiesRealLoc[loc] = erl;
    }
    public void UpdateEnemyVirtual(Vector3Int evl, int loc)
    {
        enemiesVirtualLoc[loc] = evl;
    }
    public void UpdateEnemyReal(Vector3 erl, int loc) 
    {
        enemiesRealLoc[loc] = erl;
    }
    public Vector3Int GetEnemyVirtual(int loc)
    {
        return enemiesVirtualLoc[loc];
    }
    public Vector3 GetEnemyReal(int loc)
    {
        return enemiesRealLoc[loc];
    }
    public GameObject GetEnemyObject(int loc) 
    {
        return enemies[loc];
    }
    public List<Vector3> GetAllEnemyReal()
    { 
        return enemiesRealLoc;
    }
    public List<Vector3Int> GetAllEnemyVirtual() 
    {
        return enemiesVirtualLoc;
    }
    public void ResetEnemiesLocationData() 
    { 
        enemiesVirtualLoc.Clear();
        enemiesRealLoc.Clear();
        enemies.Clear();
    }

    //Player Methods
    public void UpdatePlayerPosition(Vector3Int pvl, Vector3 prl) 
    {
        playerVirtualLoc = pvl;
        playerRealLoc = prl;
    }
    public void UpdatePlayerReal(Vector3 prl) 
    {
        playerRealLoc = prl;
    }
    public void UpdatePlayerVirtual(Vector3Int pvl) 
    {
        playerVirtualLoc = pvl;
    }
    public Vector3Int GetPlayerVirtual() 
    { 
        return playerVirtualLoc;
    }
    public Vector3 GetPlayerReal()
    {
        return playerRealLoc;
    }
}
