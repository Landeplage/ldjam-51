using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridEntity : MonoBehaviour
{
    public GridSlot gridSlot;

    void Update()
    {
        if (gridSlot != null)
        {
            transform.position = gridSlot.transform.position;
        }
    }
}
