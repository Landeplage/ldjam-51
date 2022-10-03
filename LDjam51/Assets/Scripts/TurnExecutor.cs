using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

class MoveInfo
{
    public BoardEntity obj;
    public BoardEntity target;
    public Vector3 position;
    public Vector3 targetPosition;
    public GridSlot newSlot;
    public GridEntity gridEntity;
}

public class TurnExecutor : MonoBehaviour
{
    public GUI_Timeline guiTimeline;
    public GameObject boardEntity;

    [SerializeField] EventReference footstepFmodEvent;
    [SerializeField] EventReference attackFmodEvent;
    [SerializeField] EventReference enemyAttackFmodEvent;
    [SerializeField] EventReference damageFmodEvent;
    [SerializeField] EventReference enemyDamageFmodEvent;
    [SerializeField] EventReference healFmodEvent;
    [SerializeField] EventReference hitWellFmodEvent;
    [SerializeField] EventReference hitEggsFmodEvent;

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
                var entity = Instantiate(this.boardEntity);
                grid.At(square.position).SetEntity(entity.GetComponent<GridEntity>());
                var boardEntity = entity.GetComponent<BoardEntity>();
                boardEntity.type = square.type;
                boardEntity.health = square.health;
                boardEntity.maxHealth = square.maxHealth;
                boardEntity.Create();
            }
        }
    }

    MoveInfo GetMoveInfo(BoardAction action)
    {
        var grid = FindObjectOfType<Grid>();
        MoveInfo info = new();
        info.obj = grid.At(action.position).entity.GetComponent<BoardEntity>();
        if (grid.At(action.target).entity && grid.At(action.target).entity.GetComponent<BoardEntity>())
        {
            info.target = grid.At(action.target).entity.GetComponent<BoardEntity>();
        }
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
                Vector3 startPos = info.obj.transform.position;
                FMODUtility.Play(footstepFmodEvent, info.obj.transform.position);
                for (float i = 0; i < 1.0; i += 0.35f)
                {
                    info.obj.transform.position = Vector3.Lerp(startPos, info.targetPosition, i);
                    yield return new WaitForSeconds(0.016f);
                }
                info.obj.transform.position = info.targetPosition;
            }
            if (action.type == BoardActionType.Attack)
            {
                if (BoardSquare.FriendlyType(info.obj.type))
                {
                    FMODUtility.Play(attackFmodEvent, info.obj.transform.position);
                }
                else
                {
                    FMODUtility.Play(enemyAttackFmodEvent, info.obj.transform.position);
                }
                yield return info.obj.Attack(info.position, info.targetPosition);
                if (info.target.type == BoardSquareType.EnemyWell)
                {
                    FMODUtility.Play(hitEggsFmodEvent, info.target.transform.position);
                }
                else if (info.target.type == BoardSquareType.Well)
                {
                    FMODUtility.Play(hitWellFmodEvent, info.target.transform.position);
                }
                else if (BoardSquare.FriendlyType(info.target.type))
                {
                    FMODUtility.Play(enemyDamageFmodEvent, info.target.transform.position);
                }
                else
                {
                    FMODUtility.Play(damageFmodEvent, info.target.transform.position);
                }
                info.target.Hurt(1);
                if (info.target.Dead())
                {
                    Destroy(info.target.gameObject);
                }
            }
            if (action.type == BoardActionType.Heal)
            {
                if (BoardSquare.FriendlyType(info.obj.type))
                {
                    FMODUtility.Play(healFmodEvent, info.obj.transform.position);
                }
                info.target.Heal(1);
                yield return info.obj.Attack(info.position, info.targetPosition);
            }
            yield return new WaitForSeconds(0.05f);
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