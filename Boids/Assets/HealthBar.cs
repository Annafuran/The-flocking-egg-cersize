using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public EnemyBehavior enemyHealth;

    private void Start()
    {
        enemyHealth = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyBehavior>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = enemyHealth.maxHealth;
        healthBar.value = enemyHealth.maxHealth;
    }

    public void SetHealth(float hp)
    {
        healthBar.value = hp;
    }
}