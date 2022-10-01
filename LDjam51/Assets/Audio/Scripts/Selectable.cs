using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    static Selectable selected = null;
    static bool canSelect = false;

    static public UnityEvent<Selectable> onSelectionChange = new UnityEvent<Selectable>();

    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0) && canSelect)
        {
            if (selected)
            {
                selected.onDeselect.Invoke();
                onSelectionChange.Invoke(null);
                selected = null;
            }
        }*/
    }

    private void LateUpdate()
    {
        canSelect = true;
    }

    private void OnMouseDown()
    {
        if (canSelect)
        {
            if (selected)
            {
                selected.onDeselect.Invoke();
            }
            selected = this;
            onSelect.Invoke();
            onSelectionChange.Invoke(this);
            canSelect = false;
        }
    }
}
