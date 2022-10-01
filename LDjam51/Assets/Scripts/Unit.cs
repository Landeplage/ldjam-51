using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GridSlot GetGridSlot()
    {
        return GetComponent<GridEntity>().gridSlot;
    }
}
