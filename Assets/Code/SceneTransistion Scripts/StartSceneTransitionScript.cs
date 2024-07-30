using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneTransitionScript : MonoBehaviour
{
    public string nextSceneFirstRun;
    public string nextSceneOtherRun;
    
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = Resources.Load<PlayerStats>("PlayerStats");
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
            }
            else 
            {
                SceneManager.LoadSceneAsync(nextSceneOtherRun);
                Debug.Log("Not Running Intro");
            }
        }    
    }
}
