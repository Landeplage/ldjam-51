using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : MonoBehaviour
{
    public GUI_Healthbar hpBar;
    public int maxHealth = 1;
    int health = 0;

    private void Start()
    {
        health = maxHealth;
        hpBar.UpdateBar(health, maxHealth);
    }

    public void Hurt(int amount)
    {
        health = (int)Mathf.Max(health - amount, 0.0f);
        hpBar.UpdateBar(health, maxHealth);
    }

    public bool Dead()
    {
        return health == 0;
    }
}
