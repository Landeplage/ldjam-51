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
                if (actions[i].type == BoardActionType.Move)
                {
                    var wellPosition = board.ClosestWell(actions[i].moveTo);
                    if (wellPosition.x == -1)
                    {
                        actions[i].score = 9999.0f;
                    }
                    else
                    {
                        actions[i].score = ((Vector2)actions[i].moveTo - wellPosition).magnitude;
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
        var foundEntities = new List<Vector2Int>();
        for (int i = 0; i < actions.Count; ++i) {
            if (foundEntities.Contains(actions[i].position))
            {
                actions.RemoveAt(i);
                i--;
            }
            else
            {
                foundEntities.Add(actions[i].position);
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
            var targettingFriendlyUnit = targeting.Type() == BoardSquareType.Unit && targeting.Friendly();
            var targettingWell = targeting.Type() == BoardSquareType.Well;
            if (!targettingFriendlyUnit && !targettingWell)
            {
                return false;
            }
        }
        return true;
    }
}
