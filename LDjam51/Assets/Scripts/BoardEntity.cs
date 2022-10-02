using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEntity : MonoBehaviour
{
    public BoardSquareType type;

    public GameObject friendlyMeleeVisuals;
    public GameObject friendlyRangedVisuals;
    public GameObject friendlyHealerVisuals;
    public GameObject enemyVisuals;
    public GameObject wellVisuals;
    public GameObject enemyWellVisuals;

    [System.NonSerialized]
    public int maxHealth;
    [System.NonSerialized]
    public int health;

    private GUI_Healthbar healthbar;

    public void Create()
    {
        if (type == BoardSquareType.Well)
        {
            Instantiate(enemyWellVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.EnemyWell)
        {
            Instantiate(wellVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.FriendlyMelee)
        {
            Instantiate(friendlyMeleeVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.FriendlyRange)
        {
            Instantiate(friendlyRangedVisuals).transform.SetParent(transform, false);
        }
        else if (type == BoardSquareType.FriendlyHealer)
        {
            Instantiate(friendlyHealerVisuals).transform.SetParent(transform, false);
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

    public void Heal(int amount)
    {
        health = (int)Mathf.Min(health + amount, maxHealth);
    }

    public bool Dead()
    {
        return health == 0;
    }
}
