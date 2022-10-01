using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Ghostable>().onGhost.AddListener(OnGhost);
        GetComponent<Ghostable>().onUnghost.AddListener(OnUnghost);
    }

    void OnGhost()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    void OnUnghost()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public GridSlot GetGridSlot()
    {
        return GetComponent<GridEntity>().gridSlot;
    }
}
