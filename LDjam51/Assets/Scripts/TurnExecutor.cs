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
    List<BoardAction> playerActions;
    List<BoardAction> aiActions;
    int playerActionDoneCount = 0;
    int aiActionDoneCount = 0;

    TurnExecutorState state = TurnExecutorState.Stopped;
    TurnPlannerVisuals visuals;

    public void Go(List<BoardAction> playerActions, List<BoardAction> aiActions)
    {
        visuals = FindObjectOfType<TurnPlannerVisuals>();
        this.playerActions = playerActions;
        this.aiActions = aiActions;
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
                if (playerActions.Count > 0)
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
                if (aiActions.Count > 0)
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
                var action = playerActions[0];
                playerActions.RemoveAt(0);
                yield return RunAction(true, action);
                playerActionDoneCount++;
                guiTimeline.ActionPerformed(TeamType.Player, playerActionDoneCount);
            }
            else if (state == TurnExecutorState.AiAction)
            {
                var action = aiActions[0];
                aiActions.RemoveAt(0);
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
        return true;
    }

    private IEnumerator RunAction(bool player, BoardAction action)
    {
        visuals.Clear();
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
        if (ValidAction(action))
        {
            if (action.type == BoardActionType.Move)
            {
                action.obj.GetComponent<GridEntity>().MoveTo(action.moveTo);
            }
        }
    }
}