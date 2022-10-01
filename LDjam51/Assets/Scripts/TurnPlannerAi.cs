using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlannerAi
{
    public static void PlanMove(Board board)
    {
        var actions = board.AllValidActions(false);
        if (actions.Count == 0)
        {
            board.AddIdleAction();
        }
        else
        {
            board.AddAction(actions[Random.Range(0, actions.Count)]);
        }
    }
}
