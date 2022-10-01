using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnExecutor : MonoBehaviour
{
    List<BoardAction> actions;

    public TurnPlannerVisuals visuals;

    bool running = false;

    private void Start()
    {
        visuals = FindObjectOfType<TurnPlannerVisuals>();
    }

    public void Go(List<BoardAction> actions)
    {
        this.actions = actions;
        StartCoroutine(NextAction());
    }

    private IEnumerator NextAction()
    {
        running = actions.Count > 0;
        if (running)
        {
            var action = actions[0];
            actions.RemoveAt(0);
            yield return RunAction(action);
        }
        else
        {
            visuals.Clear();
            Game.Get().onPlanningStart.Invoke();
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

    private IEnumerator RunAction(BoardAction action)
    {
        visuals.Clear();
        if (action.type == BoardActionType.Move)
        {
            visuals.MovementLine(action.moveFrom, action.moveTo);
            if (ValidAction(action))
            {
                visuals.MoveSlot(action.moveTo);
            }
            else
            {
                visuals.BadMoveSlot(action.moveTo);
            }
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
        yield return NextAction();
    }
}
