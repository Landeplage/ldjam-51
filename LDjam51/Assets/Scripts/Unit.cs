using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool friendly;

    private void Start()
    {
        GetComponent<Ghostable>().onGhost.AddListener(OnGhost);
        GetComponent<Ghostable>().onUnghost.AddListener(OnUnghost);
    }

    void OnGhost()
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (spriteRenderer.gameObject.name != "DropShadow")
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
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
        }
    }

    public GridSlot GetGridSlot()
    {
        return GetComponent<GridEntity>().gridSlot;
    }
}
