using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    public GameObject unitMenu;

    void Start()
    {
        Selectable.onSelectionChange.AddListener(OnSelectionChange);
    }

    void OnSelectionChange(Selectable selectable)
    {
        if (selectable == null)
        {
            unitMenu.SetActive(false);
        }
        else
        {
            unitMenu.SetActive(true);
        }
    }
}
