using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEntity : MonoBehaviour
{
    public BoardSquareType type;

    public GameObject friendlyVisuals;
    public GameObject enemyVisuals;
    public GameObject wellVisuals;

    [System.NonSerialized]
    public int maxHealth;
    [System.NonSerialized]
    public int health;

    private GUI_Healthbar healthbar;

    public void Create()
    {
        if (type == BoardSquareType.Well)
        {
            Instantiate(wellVisuals).transform.SetParent(transform, false);
        }
        else if (BoardSquare.FriendlyType(type))
        {
            Instantiate(friendlyVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.Enemy)
        {
            Instantiate(enemyVisuals).transform.SetParent(transform, false);
        }

        healthbar = GetComponentInChildren<GUI_Healthbar>();
        if (healthbar)
        {
            healthbar.UpdateBar(health, maxHealth);
        }
    }

    public void Hurt(int amount)
    {
        health = (int)Mathf.Max(health - amount, 0.0f);
        if (healthbar)
        {
            healthbar.UpdateBar(health, maxHealth);
        }
    }

    public bool Dead()
    {
        return health == 0;
    }
}
