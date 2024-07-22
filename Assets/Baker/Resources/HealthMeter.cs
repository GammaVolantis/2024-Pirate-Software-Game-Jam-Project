using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider healthSlider;  // Reference to the health slider
    private float health = 100f;
    private float maxHealth = 100f;

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        healthSlider.value = health;
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
        healthSlider.value = health;
    }
}
