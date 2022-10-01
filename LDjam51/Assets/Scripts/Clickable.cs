using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    public UnityEvent onClick = new();

    private void OnMouseDown()
    {
        Game.Get().clickManager.Clicked(this);
    }
}
