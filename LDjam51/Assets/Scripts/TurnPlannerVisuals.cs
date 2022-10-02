using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlannerVisuals : MonoBehaviour
{
    [System.NonSerialized]
    public Grid grid;
    public GameObject moveSlot;
    public GameObject movementLine;
    public GameObject attackSlot;
    public GameObject secondIndicator;

    private List<Ghostable> ghosts = new();

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
        foreach (var ghost in ghosts)
        {
            ghost.Unghost();
        }
        ghosts = new();
    }

    public void MoveSlot(Vector2Int position, bool enableHover)
    {
        var slot = grid.At(position);
        if (slot != null)
        {
            var move = Instantiate(moveSlot);
            move.GetComponent<SlotVisual>().Init(slot, enableHover);
            move.transform.parent = transform;
            move.transform.position = slot.transform.position + new Vector3(0.0f, 0.0f, -0.1f);
        }
    }

    public void BadMoveSlot(Vector2Int position)
    {
        var slot = grid.At(position);
        if (slot != null)
        {
            var move = Instantiate(attackSlot);
            move.GetComponent<SlotVisual>().Init(slot, false);
            move.transform.parent = transform;
            move.transform.position = slot.transform.position + new Vector3(0.0f, 0.0f, -0.1f);
        }
    }

    public void MovementLine(Vector2Int from, Vector2Int to)
    {
        var fromSlot = grid.At(from);
        var toSlot = grid.At(to);
        if (fromSlot != null && toSlot != null)
        {
            var lineObject = Instantiate(movementLine);
            lineObject.transform.parent = transform;
            lineObject.transform.position = Vector3.zero;
            var line = lineObject.GetComponent<LineRenderer>();
            //line.startColor = color;
            //line.endColor = color;
            line.positionCount = 2;
            line.SetPosition(0, fromSlot.transform.position);
            line.SetPosition(1, toSlot.transform.position);
        }
    }

    public void AttackSlot(Vector2Int position, bool enableHover)
    {
        var slot = grid.At(position);
        if (slot != null)
        {
            var attack = Instantiate(attackSlot);
            attack.GetComponent<SlotVisual>().Init(slot, enableHover);
            attack.transform.parent = transform;
            attack.transform.position = slot.transform.position + new Vector3(0.0f, 0.0f, -0.1f);
        }
    }

    public void Ghost(Vector2Int position, GameObject obj)
    {
        var ghostable = obj.GetComponent<Ghostable>();
        var positionSlot = grid.At(position);
        if (ghostable != null && positionSlot != null)
        {
            if (!ghosts.Contains(ghostable))
            {
                ghosts.Add(ghostable);
            }
            var copy = ghostable.Ghost();
            copy.transform.parent = transform;
            copy.transform.position = positionSlot.transform.position + new Vector3(0.0f, 0.0f, -0.2f);
        }
    }

    public void SecondIndicator(Vector2Int from, Vector2Int to, int second)
    {
        var fromSlot = grid.At(from);
        var toSlot = grid.At(to);
        if (fromSlot != null && toSlot != null)
        {
            var center = ((((Vector2)fromSlot.transform.position) + ((Vector2)toSlot.transform.position)) * 0.5f);
            var obj = Instantiate(secondIndicator);
            obj.transform.SetParent(transform, false);
            obj.transform.position = new Vector3(center.x, center.y, 0.0f);
            obj.GetComponentInChildren<TMPro.TextMeshPro>().text = second.ToString();
        }
    }
}
