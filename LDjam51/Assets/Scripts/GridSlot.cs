using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSlot : MonoBehaviour
{
    public Vector2Int position;

    [System.NonSerialized]
    public GridEntity entity;

    private void Start()
    {
        GetComponent<Clickable>().onClick.AddListener(OnClick);
    }

    void Update()
    {
        var grid = GetComponentInParent<Grid>();
        GetComponent<SpriteRenderer>().color = grid.visible ? Color.white : new Color(0.0f, 0.0f, 0.0f, 0.0f);
        if (!Application.IsPlaying(gameObject))
        {
            if (position.x >= grid.width || position.y >= grid.height)
            {
                DestroyImmediate(gameObject);
            }
        }
    }

    void OnClick()
    {
        var grid = GetComponentInParent<Grid>();
        grid.onClickGrid.Invoke(this);
    }

    public void MoveTo(Vector2Int position)
    {
        if (entity != null)
        {
            var grid = GetComponentInParent<Grid>();
            var targetSlot = grid.At(position);
            if (targetSlot != null)
            {
                grid.MoveEntity(entity, targetSlot);
            }
        }
    }

    public bool IsFriendlyUnit()
    {
        var unit = GetComponent<Unit>();
        if (unit)
        {
            return unit.friendly;
        }
        return false;
    }
}
