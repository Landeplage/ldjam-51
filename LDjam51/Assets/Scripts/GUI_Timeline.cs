using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IconType
{
    QuestionMark,
    Movement,
    Attack,
    Wait
}

public enum TeamType
{
    Player,
    AI,
}

public class GUI_Timeline : MonoBehaviour
{
    public GUI_Timeline_Icon[] blueSlots = new GUI_Timeline_Icon[5];
    public GUI_Timeline_Icon[] redSlots = new GUI_Timeline_Icon[5];

    public void Start()
    {
        Game.Get().OnTurnStart.AddListener(turnNum => Reset());
    }

    public void Reset()
    {
        foreach (GUI_Timeline_Icon slot in blueSlots)
        {
            slot.ShowDot();
        }
        foreach (GUI_Timeline_Icon slot in redSlots)
        {
            slot.ShowDot();
        }
    }

    public void UpdateSlots(int count)
    {
        for (int i = 0; i < 10; i++)
        {
            var slots = i % 2 == 0 ? blueSlots : redSlots;
            if (i < count)
            {
                IconType icon = IconType.QuestionMark;
                /*switch (actions[i].type)
                {
                    case BoardActionType.Move: icon = IconType.Movement; break;
                    case BoardActionType.Attack: icon = IconType.Attack; break;
                    case BoardActionType.Idle: icon = IconType.Wait; break;
                    default: icon = IconType.QuestionMark; break;
                }*/
                slots[i / 2].ShowIcon(icon);
            }
            else
            {
                slots[i / 2].ShowDot();
            }
        }
    }

    public void ActionPerformed(TeamType team, int actionNum)
    {
        if (team == TeamType.Player)
        {
            for (int i = 0; i < actionNum && i < 5; i++)
            {
                blueSlots[i].SetPerformed();
            }
        }
        else
        {
            for (int i = 0; i < actionNum && i < 5; i++)
            {
                redSlots[i].SetPerformed();
            }
        }
    }
}
