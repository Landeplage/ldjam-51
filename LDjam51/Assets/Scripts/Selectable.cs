using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Clickable))]
public class Selectable : MonoBehaviour
{
    static public UnityEvent<Selectable> onSelectionChange = new();

    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    private void Start()
    {
        GetComponent<Clickable>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Game.Get().selectManager.Select(this);
    }
}
