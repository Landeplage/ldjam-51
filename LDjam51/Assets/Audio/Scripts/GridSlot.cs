using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSlot : MonoBehaviour
{
    [System.NonSerialized]
    public Vector2Int gridPosition;

    [System.NonSerialized]
    public GridEntity entity;

    void Update()
    {
        var grid = GetComponentInParent<Grid>();
        GetComponent<SpriteRenderer>().color = grid.visible ? Color.white : new Color(0.0f, 0.0f, 0.0f, 0.0f);
        if (!Application.IsPlaying(gameObject))
        {
            if (gridPosition.x >= grid.width || gridPosition.y >= grid.height)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
