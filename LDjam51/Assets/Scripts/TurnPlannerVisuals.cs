using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineIndicatorPosition
{
    Center,
    Left,
    Right
}

public class TurnPlannerVisuals : MonoBehaviour
{
    [System.NonSerialized]
    public Grid grid;
    public GameObject debug;
    public GameObject moveSlot;
    public GameObject movementLine;
    public GameObject attackSlot;
    public GameObject healSlot;
    public GameObject attackLine;
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

    public void Debug(Vector2Int position, Color color)
    {
        var slot = grid.At(position);
        if (slot != null)
        {
            var debug = Instantiate(this.debug);
            debug.transform.parent = transform;
            debug.transform.position = slot.transform.position + new Vector3(0.0f, 0.0f, -0.1f);
            debug.GetComponent<SpriteRenderer>().color = color;
        }
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

    void SetArrowTransform(GameObject arrow, Vector3 from, Vector3 to, LineIndicatorPosition adjust)
    {
        var startToEnd = to - from;
        startToEnd.z = 0.0f;
        var halfpoint = from + startToEnd / 2.0f;

        // Shift tangent along the arrow dir based on the adjustment
        Vector3 adjustOffset = Vector3.zero;
        switch (adjust)
        {
            case LineIndicatorPosition.Right: // fall-through
            case LineIndicatorPosition.Left: adjustOffset = 0.20f * Vector3.Cross(Vector3.Normalize(startToEnd), Vector3.back); break;
            default: break;
        }

        arrow.transform.position = new Vector3(halfpoint.x, halfpoint.y, to.z) + adjustOffset + new Vector3(0.0f, 0.0f, -0.2f);
        float angle = Vector3.SignedAngle(
            new Vector3(1.0f, 0.0f, 0.0f), 
            new Vector3(startToEnd.x, startToEnd.y, 0.0f),
            Vector3.forward
        );
        arrow.transform.rotation = Quaternion.AngleAxis(
            angle, 
            new Vector3(0.0f, 0.0f, 1.0f)
        );
        if (arrow.GetComponent<SpriteRenderer>())
        {
            arrow.GetComponent<SpriteRenderer>().flipY = angle > 90.0f || angle < -90.0f;
        }
    }

    public void MovementLine(Vector2Int from, Vector2Int to, int second, LineIndicatorPosition adjust = LineIndicatorPosition.Center)
    {
        var fromSlot = grid.At(from);
        var toSlot = grid.At(to);
        if (fromSlot != null && toSlot != null)
        {
            var lineObject = Instantiate(movementLine);
            lineObject.transform.parent = transform;
            SetArrowTransform(lineObject, fromSlot.transform.position, toSlot.transform.position, adjust);
            lineObject.GetComponent<IndicatorLine>().Init(second);
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

    public void HealSlot(Vector2Int position, bool enableHover)
    {
        var slot = grid.At(position);
        if (slot != null)
        {
            var attack = Instantiate(healSlot);
            attack.GetComponent<SlotVisual>().Init(slot, enableHover);
            attack.transform.parent = transform;
            attack.transform.position = slot.transform.position + new Vector3(0.0f, 0.0f, -0.1f);
        }
    }

    public void AttackLine(Vector2Int from, Vector2Int to, int second, LineIndicatorPosition adjust = LineIndicatorPosition.Center)
    {
        var fromSlot = grid.At(from);
        var toSlot = grid.At(to);
        if (fromSlot != null && toSlot != null)
        {
            var lineObject = Instantiate(attackLine);
            lineObject.transform.parent = transform;
            SetArrowTransform(lineObject, fromSlot.transform.position, toSlot.transform.position, adjust);
            lineObject.GetComponent<IndicatorLine>().Init(second);
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
