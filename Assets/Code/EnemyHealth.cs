using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 60;
    private int health;
    public GameObject healthFill;
    private float healthScaleFactor;
    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
        healthScaleFactor = healthFill.transform.localScale.x / maxHealth;
        healthFill.transform.localScale = new Vector3(healthScaleFactor * health, healthFill.transform.localScale.y, 0f);

    }

    // Update is called once per frame
    public void EnemyHealthUpdate(int hC)
    {
        int oldHealth = health;
        health += hC;
        if (health < 0)
        {
            health = 0;
        }
        else if (health > maxHealth) {
            health = maxHealth;
        }
        //set new player healthbar value
        healthFill.transform.localScale = new Vector3(healthScaleFactor * health, healthFill.transform.localScale.y, 0f);
        float difHealth = health - oldHealth;
        healthFill.transform.position = new Vector3(healthFill.transform.position.x + difHealth * healthScaleFactor / 2, healthFill.transform.position.y, 0f);
    }
    public int GetHealth()
    {
        return health;
    }
}
