using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlanner : MonoBehaviour
{
    void Start()
    {
        Game.Get().clickManager.onClickAny.AddListener(OnClickAny);
    }

    void OnClickAny(Clickable clickable)
    {
        var unit = clickable.GetComponent<Unit>();
        if (unit)
        {
            OnClickUnit(unit);
            return;
        }

        var gridSlot = clickable.GetComponent<GridSlot>();
        if (gridSlot)
        {
            OnClickGridSlot(gridSlot);
            return;
        }
    }

    void OnClickUnit(Unit unit)
    {
        Debug.Log("CLICKED UNIT");
    }

    void OnClickGridSlot(GridSlot gridSlot)
    {
        var selected = Game.Get().selectManager.selected;
        if (selected == null)
        {
            return;
        }
        var unit = selected.GetComponent<Unit>();
        if (!unit)
        {
            return;
        }
        var currentGridSlot = unit.GetGridSlot();
        // plan unit move
        Debug.Log("CLICKED GRID SLOT");
    }

    void Update()
    {
    }
}
