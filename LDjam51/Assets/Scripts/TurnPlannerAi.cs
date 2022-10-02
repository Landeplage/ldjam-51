using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlannerAi
{
    public static BoardAction PlanMovesFor(Board board, Vector2Int position)
    {
        Entropy entropy = new(board.entropy);
        var actions = board.ValidActionsFor(false, position);
        var hasAttack = false;
        for (int i = 0; i < actions.Count; ++i)
        {
            if (actions[i].type == BoardActionType.Attack)
            {
                hasAttack = true;
            }
            if (!actions[i].enabled || actions[i].type == BoardActionType.Attack)
            {
                actions.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < actions.Count; ++i)
        {
            if (GoodAction(board, actions[i]))
            {
                if (actions[i].type == BoardActionType.Attack)
                {
                    hasAttack = true;
                }
                if (actions[i].type == BoardActionType.Move)
                {
                    var wellPosition = board.ClosestWell(actions[i].target);
                    if (wellPosition.x == -1)
                    {
                        actions[i].score = 9999.0f;
                    }
                    else
                    {
                        actions[i].score = ((Vector2)actions[i].target - wellPosition).magnitude;
                    }
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
        actions.Sort((x, y) => x.score.CompareTo(y.score));
        if (actions.Count == 0)
        {
            return BoardAction.Idle();
        }
        else
        {
            return actions[(int)(entropy.Next() * actions.Count)];
        }
    }

    static bool GoodAction(Board board, BoardAction action)
    {
        if (action.type == BoardActionType.Attack)
        {
            var targeting = board.At(action.target);
            var targettingFriendlyUnit = BoardSquare.FriendlyType(targeting.type);
            var targettingWell = targeting.type == BoardSquareType.Well;
            if (!targettingFriendlyUnit && !targettingWell)
            {
                return false;
            }
        }
        return true;
    }
}
