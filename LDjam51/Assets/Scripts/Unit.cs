using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Selectable selectable;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        selectable = GetComponent<Selectable>();
        selectable.onSelect.AddListener(OnSelect);
        selectable.onDeselect.AddListener(OnDeselect);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnSelect()
    {
        spriteRenderer.color = Color.yellow;
    }

    void OnDeselect()
    {
        spriteRenderer.color = Color.white;
    }
}
