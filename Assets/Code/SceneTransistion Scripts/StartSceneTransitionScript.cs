using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneTransitionScript : MonoBehaviour
{
    public string nextSceneFirstRun;
    public string nextSceneOtherRun;
    
    private PlayerStats playerStats;
    private OverworldData overworldData;
    private LocationData locationData;


    private void Start()
    {
        playerStats = Resources.Load<PlayerStats>("PlayerStats");
        overworldData = Resources.Load<OverworldData>("OverWorldData");
        locationData = Resources.Load<LocationData>("AllLocationInformation");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t")) {
            playerStats.ResetPlayerStats();
            if (playerStats.GetRun())
            {
                playerStats.AfterFirstRun();
                SceneManager.LoadScene(nextSceneFirstRun);
                Debug.Log("Running Intro");
                playerStats.ResetPlayerStats();
                overworldData.ResetWorldData();
                locationData.ResetEnemiesLocationData();
            }
            else 
            {
                SceneManager.LoadSceneAsync(nextSceneOtherRun);
                Debug.Log("Not Running Intro");
                playerStats.ResetPlayerStats();
                overworldData.ResetWorldData();
                locationData.ResetEnemiesLocationData();
            }

        }    
    }
}
