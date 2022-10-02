using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotVisual : MonoBehaviour
{
    public SpriteRenderer visual;
    public Collider2D collider2d;
    public float hoverBoost = 2.0f;
    Color color;
    GridSlot slot;

    void Start()
    {
        color = visual.color;
    }

    public void Init(GridSlot slot, bool enableHover)
    {
        this.slot = slot;
        collider2d.enabled = enableHover;
    }

    void OnMouseEnter()
    {
        var boostedColor = color;
        boostedColor.a *= hoverBoost;
        visual.color = boostedColor;
    }

    void OnMouseExit()
    {
        visual.color = color;
    }

    void OnMouseDown()
    {
        if (slot && slot.GetComponent<Clickable>())
        {
            Game.Get().clickManager.Clicked(slot.GetComponent<Clickable>());
        }
    }
}
