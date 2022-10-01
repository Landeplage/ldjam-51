using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectManager : MonoBehaviour
{
    public UnityEvent<Selectable> onSelectionChange = new();
    public Selectable selected;

    private void Start()
    {
        Game.Get().clickManager.onClickNothing.AddListener(OnClickNothing);
    }

    void OnClickNothing()
    {
        if (selected != null)
        {
            Deselect(selected);
            onSelectionChange.Invoke(null);
        }
    }

    public void Select(Selectable selectable)
    {
        if (selected != null)
        {
            Deselect(selected);
        }
        selectable.onSelect.Invoke();
        onSelectionChange.Invoke(selectable);
        this.selected = selectable;
    }

    public void Deselect(Selectable selectable)
    {
        selectable.onDeselect.Invoke();
        this.selected = null;
    }
}
