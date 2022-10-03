using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    TMPro.TextMeshPro text;
    int n = 0;
    bool ending = false;

    void Start()
    {
        text = GetComponentInChildren<TMPro.TextMeshPro>();
        if (Game.level == 1)
        {
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(4, 3), new Vector2Int(6, 4));
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(-1, -1), new Vector2Int(-1, -1));
        }
        if (Game.level == 2)
        {
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(4, 3), new Vector2Int(6, 4));
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(6, 4), new Vector2Int(7, 6));
        }
        if (Game.level == 3)
        {
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(3, 4), new Vector2Int(4, 4));
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(-1, -1), new Vector2Int(-1, -1));
        }
        Next();
    }

    public void End()
    {
        ending = true;
    }

    public void Next()
    {
        n += 1;
        text.text = "";
        if (ending)
        {
            return;
        }
        if (Game.level == 1)
        {
            if (n == 1)
            {
                text.text = "Move your Melee unit to attack the enemies\n";
            }
            if (n == 2)
            {
                text.text = "Units attack automatically";
            }
            if (n == 3)
            {
                text.text = "Press Space to skip moving and only attack";
            }
        }
        if (Game.level == 2)
        {
            if (n == 1)
            {
                text.text = "Ranged units can attack from further away";
            }
            if (n == 3)
            {
                text.text = "Maximize damage by hitting multiple enemies";
            }
        }
        if (Game.level == 3)
        {
            if (n == 1)
            {
                text.text = "Move the Healer in range to heal friendly units";
            }
            if (n == 3)
            {
                text.text = "Healers can heal 1 unit at the beginning of each turn\nPress Space to skip moving";
            }
        }
        if (Game.level == 4)
        {
            if (n == 1)
            {
                text.text = "Each turn takes 1 second of time\nMore enemies spawn from nests every 10 seconds";
            }
            if (n >= 3)
            {
                text.text = "Destroy the nests to prevent getting overwhelmed";
            }
        }
        if (Game.level == 5)
        {
            if (n == 1)
            {
                text.text = "Wells grant Brightsight, the ability to turn back time";
            }
            if (n >= 3)
            {
                text.text = "Press Z to undo moves\nIf the wells are destroyed, Brightsight will no longer be available";
            }
        }
    }
}
