using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEntity : MonoBehaviour
{
    public BoardSquareType type;

    [System.NonSerialized]
    public int maxHealth;
    [System.NonSerialized]
    public int health;

    private GUI_Healthbar healthbar;

    public void Create()
    {
        healthbar = GetComponentInChildren<GUI_Healthbar>();
        healthbar.UpdateBar(health, maxHealth);
    }

    public void Hurt(int amount)
    {
        health = (int)Mathf.Max(health - amount, 0.0f);
        healthbar.UpdateBar(health, maxHealth);
    }

    public bool Dead()
    {
        return health == 0;
    }
}
