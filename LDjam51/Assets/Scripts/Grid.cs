using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    public GameObject slotPrefab;
    public int width = 10;
    public int height = 10;

    public bool visible = false;

    void Start()
    {
        if (Application.IsPlaying(gameObject))
        {
            var entities = FindObjectsOfType<GridEntity>();
            var slots = GetComponentsInChildren<GridSlot>();
            for (int i = 0; i < entities.Length; ++i)
            {
                if (entities[i].gridSlot == null)
                {
                    GridSlot closestSlot = null;
                    float closestDistance = 0.0f;
                    for (int j = 0; j < slots.Length; ++j)
                    {
                        float distance = (((Vector2)slots[j].transform.position) - ((Vector2)entities[i].transform.position)).magnitude;
                        if (closestSlot == null || distance < closestDistance)
                        {
                            closestSlot = slots[j];
                            closestDistance = distance;
                        }
                    }
                    if (closestSlot != null && closestSlot.entity == null)
                    {
                        closestSlot.entity = entities[i];
                        entities[i].gridSlot = closestSlot;
                    }
                    else
                    {
                        Destroy(entities[i].gameObject);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            var grid = GetComponent<UnityEngine.Grid>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var label = string.Format("{0},{1}", x, y);
                    var child = transform.Find(label);
                    if (child == null)
                    {
                        var newChild = PrefabUtility.InstantiatePrefab(slotPrefab) as GameObject;
                        newChild.name = label;
                        var slot = newChild.GetComponent<GridSlot>();
                        slot.gridPosition = new Vector2Int(x, y);
                        child = newChild.transform;
                        child.transform.parent = transform;
                    }
                    var xx = x * 1.0f * grid.cellSize.x;
                    var yy = y * 0.75f * grid.cellSize.y;
                    if (y % 2 == 1)
                    {
                        xx += 0.5f * grid.cellSize.x;
                    }
                    child.transform.localPosition = new Vector3(xx, yy, 0.0f);
                    child.transform.localScale = new Vector3(grid.cellSize.x, grid.cellSize.y, 0.0f);
                }
            }
        }
    }
}
