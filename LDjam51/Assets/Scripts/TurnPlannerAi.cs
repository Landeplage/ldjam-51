using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlannerAi
{
    public static void PlanMove(Board board, Entropy entropy)
    {
        var actions = board.AllValidActions(false);
        var hasAttack = false;
        for (int i = 0; i < actions.Count; ++i)
        {
            if (GoodAction(board.ApplyActions(), actions[i]))
            {
                if (actions[i].type == BoardActionType.Attack)
                {
                    hasAttack = true;
                }
            }
            else
            {
                actions.RemoveAt(i);
                i--;
            }
        }
        if (hasAttack)
        {
            for (int i = 0; i < actions.Count; ++i)
            {
                if (actions[i].type != BoardActionType.Attack)
                {
                    actions.RemoveAt(i);
                    i--;
                }
            }
        }
        if (actions.Count == 0)
        {
            board.AddIdleAction();
        }
        else
        {
            board.AddAction(actions[(int)(entropy.Next() * actions.Count)]);
        }
    }

    static bool GoodAction(Board board, BoardAction action)
    {
        if (action.type == BoardActionType.Attack)
        {
            var targeting = board.At(action.attackTarget);
            if (targeting.Type() != BoardSquareType.Unit || !targeting.Friendly())
            {
                return false;
            }
        }
        return true;
    }
}
