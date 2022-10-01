using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_Timeline : MonoBehaviour
{
    public GUI_Timeline_Icon[] blueSlots = new GUI_Timeline_Icon[5];
    public GUI_Timeline_Icon[] redSlots = new GUI_Timeline_Icon[5];

    public void SetAction(int turn, IconType icon)
    {
        if (turn % 2 == 0)
        {
            redSlots[turn / 2].ShowIcon(icon);
        }
        else
        {
            blueSlots[turn / 2].ShowIcon(icon);
        }
    }

    public void UnsetAction(int turn)
    {
        if (turn % 2 == 0)
        {
            redSlots[turn / 2].ShowDot();
        }
        else
        {
            blueSlots[turn / 2].ShowDot();
        }
    }
}
