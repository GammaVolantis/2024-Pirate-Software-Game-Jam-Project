using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBar : MonoBehaviour
{

    public PlayerStats player;
    public GameObject healthFill;

    private float healthScaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        healthScaleFactor = healthFill.transform.localScale.x / player.maxHealth;
        healthFill.transform.localScale = new Vector3(healthScaleFactor * player.curHealth, healthFill.transform.localScale.y, 0f);
        float difHealth = player.curHealth - player.maxHealth;
        healthFill.transform.position = new Vector3(healthFill.transform.position.x + difHealth * healthScaleFactor / 2, healthFill.transform.position.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HealthBarUpdate(int hC)
    {
        int oldHealth = player.curHealth;
        player.newHealthValue(hC);
        int newHealth = player.curHealth;
        //set new player healthbar value
        healthFill.transform.localScale = new Vector3(healthScaleFactor * player.curHealth, healthFill.transform.localScale.y, 0f);
        float difHealth = newHealth - oldHealth;
        healthFill.transform.position = new Vector3(healthFill.transform.position.x + difHealth * healthScaleFactor / 2, healthFill.transform.position.y, 0f);

    }
}
