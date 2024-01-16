using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    int health;
    int maxHealth;
    private void Start()
    {
        maxHealth = Random.Range(50, 200);
        health = maxHealth;
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health < 0)
        {
            Die();
        }
    }
    void Die()
    {
        // deathlogic
    }
}
