using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{

    public PlayerStats player;

    private float healthScaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        Transform playerTransform = gameObject.transform;
        Transform childTransform = playerTransform.Find("HealthBar/Fill");
        if (childTransform != null) 
        {
            healthScaleFactor = childTransform.localScale.x;
        }
        else 
        { 
            Debug.Log("Could Not Find Fill!!!"); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            //change in health
            player.newHealthValue(player.GetCurrentHealth(),-20);
            //set new player healthbar value
            
        }
    }
}
