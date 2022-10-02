using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MoveInfo
{
    public GameObject obj;
    public Vector3 position;
    public Vector3 targetPosition;
    public GridSlot newSlot;
    public GridEntity gridEntity;
}

public class TurnExecutor : MonoBehaviour
{
    public GUI_Timeline guiTimeline;
    public GameObject boardEntity;

    public void ResetEntities(Board board)
    {
        var grid = FindObjectOfType<Grid>();
        foreach (var slot in grid.Slots())
        {
            if (slot.entity)
            {
                Destroy(slot.entity.gameObject);
            }
        }
        foreach (var square in board.squares)
        {
            if (square.type != BoardSquareType.Empty)
            {
                var entity = Instantiate(boardEntity);
                grid.At(square.position).SetEntity(entity.GetComponent<GridEntity>());
            }
        }
    }

    MoveInfo GetMoveInfo(BoardAction action)
    {
        var grid = FindObjectOfType<Grid>();
        MoveInfo info = new();
        info.obj = grid.At(action.position).entity.gameObject;
        info.position = grid.At(action.position).transform.position;
        info.targetPosition = grid.At(action.target).transform.position;
        info.newSlot = grid.At(action.target);
        if (action.type == BoardActionType.Move)
        {
            info.gridEntity = grid.At(action.position).entity;
        }
        return info;
    }

    public IEnumerator PlayAction(BoardAction action)
    {
        if (action.type != BoardActionType.Idle)
        {
            var info = GetMoveInfo(action);
            if (action.type == BoardActionType.Move)
            {
                info.obj.transform.position = info.targetPosition;
            }
            yield return new WaitForSeconds(0.2f);
            if (info.gridEntity)
            {
                info.newSlot.SetEntity(info.gridEntity);
            }
        }
    }

    public IEnumerator UndoAction(BoardAction action)
    {
        if (action.type != BoardActionType.Idle)
        {
            var info = GetMoveInfo(action);
            yield return new WaitForSeconds(0.2f);
            if (info.gridEntity)
            {
                info.newSlot.SetEntity(info.gridEntity);
            }
        }
    }
}