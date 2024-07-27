using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    //sets health bar max value
    public void SetMaxHealth(int health) 
    { 
        slider.maxValue = health;
    }
    
    //sets the health bar current health
    public void SetHealth(int health) 
    {
        slider.value = health;
    }

}
