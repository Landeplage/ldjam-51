using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlannerVisuals : MonoBehaviour
{
    [System.NonSerialized]
    public Grid grid;
    public GameObject highlightSlot;
    public GameObject movementLine;

    public void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void HighlightSlot(Vector2Int position)
    {
        var slot = grid.At(position);
        if (slot != null)
        {
            var highlight = Instantiate(highlightSlot);
            highlight.transform.parent = transform;
            highlight.transform.position = slot.transform.position;
        }
    }

    public void MovementLine(Vector2Int from, Vector2Int to)
    {
        var fromSlot = grid.At(from);
        var toSlot = grid.At(to);
        if (fromSlot != null && toSlot != null)
        {
            var lineObject = Instantiate(movementLine);
            var line = lineObject.GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, fromSlot.transform.position);
            line.SetPosition(1, toSlot.transform.position);
        }
    }
}
