using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats" )]
public class PlayerStats : ScriptableObject
{

    public int curHealth = 100;
    public int maxHealth = 100;
    public Vector3 worldPos;
    public   bool firstRun = true;
    private string loseScene = "Lose Scene";

    //public List<Card> currentCards = new List<Card>();

    public void newHealthValue(int changeHealth)
    {
        curHealth+=changeHealth;
        if (curHealth <= 0)
        {
            ResetPlayerStats();
            //PlayDeathSound
            SceneManager.LoadScene(loseScene);
        }
        else if (curHealth > maxHealth) 
        { 
            curHealth = maxHealth;
        }
    }
    public int GetMaxHealth() { 
        return maxHealth;
    }

    public int GetCurrentHealth() { 
        return curHealth;
    }

    public void ResetPlayerStats()
    {
        curHealth = maxHealth;
        worldPos = new Vector3(0f,0f,0f);
    }
    public bool GetRun() 
    { 
        return firstRun;
    }
    public void AfterFirstRun() 
    {
        firstRun = false;
    }
}
