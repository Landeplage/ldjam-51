using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickManager : MonoBehaviour
{
    public UnityEvent<Clickable> onClickAny = new();
    public UnityEvent onClickNothing = new();

    public bool canClick = true;

    public void Clicked(Clickable clickable)
    {
        if (canClick)
        {
            onClickAny.Invoke(clickable);
            clickable.onClick.Invoke();
            canClick = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            onClickNothing.Invoke();
        }
    }

    public void LateUpdate()
    {
        canClick = true;
    }
}
