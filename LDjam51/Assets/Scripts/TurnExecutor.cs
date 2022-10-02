using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TurnExecutorState
{
    Stopped,
    PlayerAction,
    AiAction,
}

public class TurnExecutor : MonoBehaviour
{
    public GUI_Timeline guiTimeline;
    List<BoardAction> actions;
    int playerActionDoneCount = 0;
    int aiActionDoneCount = 0;

    TurnExecutorState state = TurnExecutorState.Stopped;
    TurnPlannerVisuals visuals;

    List<GameObject> deadEntities;

    public void Go(List<BoardAction> actions)
    {
        deadEntities = new();
        visuals = FindObjectOfType<TurnPlannerVisuals>();
        this.actions = actions;
        this.playerActionDoneCount = 0;
        this.aiActionDoneCount = 0;
        StartCoroutine(RunActions());
    }

    private IEnumerator RunActions()
    {
        while (true)
        {
            if (state == TurnExecutorState.Stopped || state == TurnExecutorState.AiAction)
            {
                if (actions.Count > 0)
                {
                    state = TurnExecutorState.PlayerAction;
                }
                else
                {
                    state = TurnExecutorState.Stopped;
                }
            }
            else
            {
                if (actions.Count > 0)
                {
                    state = TurnExecutorState.AiAction;
                }
                else
                {
                    state = TurnExecutorState.Stopped;
                }
            }
            if (state == TurnExecutorState.PlayerAction)
            {
                var action = actions[0];
                actions.RemoveAt(0);
                yield return RunAction(true, action);
                playerActionDoneCount++;
                guiTimeline.ActionPerformed(TeamType.Player, playerActionDoneCount);
            }
            else if (state == TurnExecutorState.AiAction)
            {
                var action = actions[0];
                actions.RemoveAt(0);
                yield return RunAction(true, action);
                aiActionDoneCount++;
                guiTimeline.ActionPerformed(TeamType.AI, aiActionDoneCount);
            }
            else if (state == TurnExecutorState.Stopped)
            {
                visuals.Clear();
                yield return new WaitForSeconds(0.5f);
                Game.Get().OnExecutionEnd();
                break;
            }
        }
    }

    private bool ValidAction(BoardAction action)
    {
        var grid = FindObjectOfType<Grid>();
        if (action.type == BoardActionType.Move)
        {
            return grid.At(action.moveTo).entity == null;
        }
        if (action.type == BoardActionType.Attack)
        {
            if (action.obj == null || deadEntities.Contains(action.obj) || grid.At(action.attackTarget).entity == null || grid.At(action.attackTarget).entity.gameObject == null || deadEntities.Contains(grid.At(action.attackTarget).entity.gameObject))
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator RunAction(bool player, BoardAction action)
    {
        visuals.Clear();
        if (ValidAction(action))
        {
            if (action.type == BoardActionType.Move)
            {
                visuals.MovementLine(action.moveFrom, action.moveTo);
                visuals.MoveSlot(action.moveTo);
            }
            else if (action.type == BoardActionType.Attack)
            {
                visuals.AttackSlot(action.attackTarget);
            }
            yield return new WaitForSeconds(0.5f);
            if (action.type == BoardActionType.Move)
            {
                action.obj.GetComponent<GridEntity>().MoveTo(action.moveTo);
            }
            else if (action.type == BoardActionType.Attack)
            {
                var grid = FindObjectOfType<Grid>();
                var targetEntity = grid.At(action.attackTarget).entity;
                var unit = targetEntity.gameObject.GetComponent<Unit>();
                var well = targetEntity.gameObject.GetComponent<Well>();
                if (targetEntity != null)
                {
                    if (unit)
                    {
                        unit.Hurt(1);
                        if (unit.Dead())
                        {
                            deadEntities.Add(unit.gameObject);
                            Destroy(unit.gameObject);
                        }
                    }
                    if (well)
                    {
                        well.Hurt(1);
                        if (well.Dead())
                        {
                            deadEntities.Add(well.gameObject);
                            Destroy(well.gameObject);
                        }
                    }
                }
            }
        }
    }
}