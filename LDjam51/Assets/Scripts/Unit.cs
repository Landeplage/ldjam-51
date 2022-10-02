using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool friendly;
    public GUI_Healthbar hpBar;
    public int maxHealth = 1;
    int health = 0;

    private void Start()
    {
        health = maxHealth;
        hpBar.UpdateBar(health, maxHealth);
        GetComponent<Ghostable>().onGhost.AddListener(OnGhost);
        GetComponent<Ghostable>().onUnghost.AddListener(OnUnghost);
    }

    void OnGhost()
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (spriteRenderer.gameObject.name != "DropShadow")
            {
                if (friendly)
                {
                    spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
                }
                else
                {
                    spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }

    void OnUnghost()
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (spriteRenderer.gameObject.name != "DropShadow")
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    public GridSlot GetGridSlot()
    {
        return GetComponent<GridEntity>().gridSlot;
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
