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

    bool IsAdjecent(GridSlot other)
    {
        return true;
    }
}
