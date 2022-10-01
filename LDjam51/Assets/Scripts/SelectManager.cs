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
        }
    }

    public void Select(Selectable selectable)
    {
        if (selected != null)
        {
            Deselect(selected);
        }
        selectable.onSelect.Invoke();
        this.selected = selectable;
    }

    public void Deselect(Selectable selectable)
    {
        selectable.onDeselect.Invoke();
        this.selected = null;
    }
}
