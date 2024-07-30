using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBar : MonoBehaviour
{

    public PlayerStats player;
    public GameObject healthFill;

    private float healthScaleFactor;
    private int previousHealth;
    // Start is called before the first frame update
    void Awake()
    {
        healthScaleFactor = healthFill.transform.localScale.x / player.maxHealth;
        healthFill.transform.localScale = new Vector3(healthScaleFactor * player.curHealth, healthFill.transform.localScale.y, 0f);
        float difHealth = player.curHealth - player.maxHealth;
        healthFill.transform.position = new Vector3(healthFill.transform.position.x + difHealth * healthScaleFactor / 2, healthFill.transform.position.y, 0f);
        previousHealth = player.curHealth;
    }
    private void Update()
    {
        if (previousHealth != player.curHealth) 
        { 
            int change = player.curHealth - previousHealth;
            healthFill.transform.localScale = new Vector3(healthScaleFactor * player.curHealth, healthFill.transform.localScale.y, 0f);
            healthFill.transform.position = new Vector3(healthFill.transform.position.x + change * healthScaleFactor / 2, healthFill.transform.position.y, 0f);
            previousHealth = player.curHealth;
        }
    }
    public void HealthBarUpdate(int hC)
    {
        player.newHealthValue(hC);
    }
}
