using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ghostable : MonoBehaviour
{
    public GameObject visualCopy;
    public UnityEvent onGhost = new();
    public UnityEvent onUnghost = new();

    [System.NonSerialized]
    public GameObject ghostInstance;

    public GameObject Ghost()
    {
        if (ghostInstance == null)
        {
            onGhost.Invoke();
        }
        else
        {
            Destroy(ghostInstance);
        }
        ghostInstance = Instantiate(visualCopy);
        return ghostInstance;
    }

    public void Unghost()
    {
        this.onUnghost.Invoke();
        if (ghostInstance)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
        }
    }
}
