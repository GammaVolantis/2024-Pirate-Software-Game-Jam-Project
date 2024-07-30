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
    private List<bool> enemiesMoveTurn;
    private GameObject player;
    private bool playerAction = false;
    private bool playerMove = false;

    //Enemies Methods
    private void OnEnable()
    {
        // Initialize lists if they are null
        if (enemiesVirtualLoc == null) enemiesVirtualLoc = new List<Vector3Int>();
        if (enemiesRealLoc == null) enemiesRealLoc = new List<Vector3>();
        if (enemies == null) enemies = new List<GameObject>();
        if (enemiesMoveTurn == null) enemiesMoveTurn = new List<bool>();
    }
    public void AddEnemyLocations(Vector3Int evl, Vector3 erl, GameObject enemy) 
    { 
        enemiesVirtualLoc.Add(evl);
        enemiesRealLoc.Add(erl);
        enemies.Add(enemy);
        enemiesMoveTurn.Add(false);
    }
    public void AddEnemyLocationsTest(Vector3Int evl, Vector3 erl)
    {
        enemiesVirtualLoc.Add(evl);
        enemiesRealLoc.Add(erl);
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
    public List<GameObject> GetAllEnemyObjects() 
    {
        return enemies;
    }
    public int GetNumberOfEnemies() {
        return enemies.Count;
    }
    public List<Vector3> GetAllEnemyReal()
    { 
        return enemiesRealLoc;
    }
    public List<Vector3Int> GetAllEnemyVirtual() 
    {
        return enemiesVirtualLoc;
    }
    public void RemoveEnemy(int loc) 
    { 
        enemies.RemoveAt(loc);
        enemiesRealLoc.RemoveAt(loc);
        enemiesVirtualLoc.RemoveAt(loc);
        enemiesMoveTurn.RemoveAt(loc);
    }
    public void ResetEnemiesLocationData()
    {
        enemiesVirtualLoc.Clear();
        enemiesRealLoc.Clear();
        enemies.Clear();
        enemiesMoveTurn.Clear();
        playerAction = false;
        playerMove = false;
    }

    //Player Methods
    public void UpdatePlayerPosition(Vector3Int pvl, Vector3 prl) 
    {
        playerVirtualLoc = pvl;
        playerRealLoc = prl;
    }
    public void SetPlayerObject(GameObject pro) 
    {
        player = pro;
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
    public GameObject GetPlayerObject() 
    {
        return player;
    }

    //Player Checks for CombatTurns
    public void SetPlayerMove() 
    {
        playerMove = true;
    }
    public bool GetPlayerMove() 
    {
        return playerMove;
    }
    public void SetPlayerAction() 
    {
        playerAction = true;
    }
    public bool GetPlayerAction() 
    {
        return playerAction;
    }

    //CombatTurn Controls
    public void CheckPlayerStatus() 
    {
        Debug.Log($"PlayerAction = {playerAction}\nPlayerMove = {playerMove}");
        if (playerAction && playerMove) 
        {
            for (int i =0; i<enemiesMoveTurn.Count; i++) 
            {
                enemiesMoveTurn[i] = true;
            }
            playerAction = false;
            playerMove = false;
        }
    }
    public void SetEnemyMoveState(int loc) 
    {
        enemiesMoveTurn[loc] = false;
    }
    public bool CheckEnemyMoveState(int loc) 
    {
        return enemiesMoveTurn[loc];
    }
    public void CheckEnemiesStatus() 
    { 
        bool waitingOnAnEnemy = false;
        for (int i = 0; i < enemiesMoveTurn.Count; i++) 
        {
            if (enemiesMoveTurn[i]) 
            {
                waitingOnAnEnemy = true;
            }    
        }
        if (!waitingOnAnEnemy) 
        {
            playerAction = false;
            playerMove=false;
        }
    }
}
