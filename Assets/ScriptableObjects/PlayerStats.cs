using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats" )]
public class PlayerStats : ScriptableObject
{

    public int curHealth = 100;
    public int maxHealth = 100;
    public Vector3 worldPos;

    //public List<Card> currentCards = new List<Card>();

    public void newHealthValue(int changeHealth)
    {
        curHealth+=changeHealth;
        if (curHealth <= 0)
        {
            //Change Scene to Lose Scene

            ////Reset Player Stats
            //ResetPlayerStats();
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

    private void ResetPlayerStats()
    {
        curHealth = maxHealth;
        worldPos = new Vector3(0f,0f,0f);
    }
}
