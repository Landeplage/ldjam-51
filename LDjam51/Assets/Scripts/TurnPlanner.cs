using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public BoardSquare[] squares;
    public List<BoardAction> actions = new();

    public Board Copy()
    {
        Board board = new();
        board.width = this.width;
        board.height = this.height;
        board.squares = new BoardSquare[this.squares.Length];
        for (int i = 0; i < board.squares.Length; ++i)
        {
            board.squares[i] = this.squares[i].Copy();
        }
        for (int i = 0; i < board.actions.Count; ++i)
        {
            board.actions.Add(this.actions[i].Copy());
        }
        return board;
    }

    int Index(Vector2Int position)
    {
        return position.x + width * position.y;
    }

    bool ValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }

    public void AddAction(BoardAction action)
    {
        actions.Add(action);
    }

    public void AddIdleAction()
    {
        actions.Add(BoardAction.Idle());
    }

    public void ClearActions()
    {
        actions.Clear();
    }

    public Board ApplyActions()
    {
        var board = Copy();
        foreach (var action in actions)
        {
            if (action.type == BoardActionType.Move)
            {
                board.squares[Index(action.moveTo)].Take(board.squares[Index(action.moveFrom)]);
            }
        }
        return board;
    }

    public BoardSquare At(Vector2Int position)
    {
        if (ValidPosition(position))
        {
            return squares[Index(position)];
        }
        return null;
    }

    List<Vector2Int> BlockedPositions()
    {
        List<Vector2Int> blocked = new();
        foreach (var action in actions)
        {
            blocked.Add(action.moveFrom);
            blocked.Add(action.moveTo);
        }
        return blocked;
    }

    public List<BoardAction> AllValidActions(bool friendly)
    {
        var actions = new List<BoardAction>();
        var applied = ApplyActions();
        foreach (var square in squares)
        {
            actions.AddRange(applied.ValidActionsForInternal(friendly, square.position, BoardActionType.Move, BlockedPositions()));
            //actions.AddRange(ValidActionsFor(friendly, square.position, BoardActionType.Attack));
        }
        return actions;
    }

    public List<BoardAction> ValidActionsFor(bool friendly, Vector2Int position, BoardActionType type)
    {
        return ApplyActions().ValidActionsForInternal(friendly, position, type, BlockedPositions());
    }

    List<BoardAction> ValidActionsForInternal(bool friendly, Vector2Int position, BoardActionType type, List<Vector2Int> blocked)
    {
        var actions = new List<BoardAction>();
        var square = At(position);
        if (square.Type() == BoardSquareType.Unit)
        {
            if (square.Friendly() == friendly)
            {
                if (type == BoardActionType.Move)
                {
                    foreach (var adjacentSquare in Adjacent(square))
                    {
                        if (adjacentSquare.Type() == BoardSquareType.Empty && !blocked.Contains(adjacentSquare.position))
                        {
                            actions.Add(BoardAction.Move(square.position, adjacentSquare.position, square.obj));
                        }
                    }
                }
                else if (type == BoardActionType.Attack)
                {
                    foreach (var adjacentSquare in Adjacent(square))
                    {
                        actions.Add(BoardAction.Attack(square.position, adjacentSquare.position, square.obj));
                    }
                }
            }
        }
        return actions;
    }

    List<BoardSquare> Adjacent(BoardSquare square)
    {
        Vector2Int[] offsets = new Vector2Int[] {
            new(-1, 0),
            new(1, 0),
            new(0, 1),
            new(0, -1),
            new(1, 1),
            new(1, -1),
        };
        if (square.position.y % 2 == 0)
        {
            offsets[2].x -= 1;
            offsets[3].x -= 1;
            offsets[4].x -= 1;
            offsets[5].x -= 1;
        }
        List<BoardSquare> adjacentSquares = new();
        foreach (var offset in offsets)
        {
            var offsetPosition = square.position + offset;
            var offsetSquare = At(offsetPosition);
            if (offsetSquare != null)
            {
                adjacentSquares.Add(offsetSquare);
            }
        }
        return adjacentSquares;
    }
}

public enum BoardSquareType
{
    Empty,
    Unit,
}

public class BoardSquare
{
    public GridSlot slot;
    public Vector2Int position;
    public GameObject obj;

    BoardSquare() {}

    public BoardSquare(GridSlot slot)
    {
        this.slot = slot;
        this.position = slot.position;
        var entity = slot.GetComponent<GridSlot>().entity;
        if (entity != null)
        {
            this.obj = entity.gameObject;
        }
        else
        {
            this.obj = null;
        }
    }

    public BoardSquareType Type()
    {
        if (obj != null)
        {
            return BoardSquareType.Unit;
        }
        else
        {
            return BoardSquareType.Empty;
        }
    }

    public bool Friendly()
    {
        if (obj != null)
        {
            var unit = obj.GetComponent<Unit>();
            if (unit)
            {
                return unit.friendly;
            }
        }
        return false;
    }

    public void MakeEmpty()
    {
        this.obj = null;
    }

    public void Take(BoardSquare other)
    {
        this.obj = other.obj;
        other.MakeEmpty();
    }

    public BoardSquare Copy()
    {
        BoardSquare square = new();
        square.slot = this.slot;
        square.obj = this.obj;
        square.position = this.position;
        return square;
    }
}

public enum BoardActionType
{
    Idle,
    Move,
    Attack,
}

public class BoardAction
{
    public BoardActionType type;
    public GameObject obj;

    public Vector2Int position;

    public Vector2Int moveFrom;
    public Vector2Int moveTo;

    public Vector2Int attackTarget;

    public bool hidden;

    public static BoardAction Idle()
    {
        BoardAction action = new();
        action.type = BoardActionType.Idle;
        action.position = new Vector2Int(-1, -1);
        action.obj = null;
        action.hidden = false;
        return action;
    }

    public static BoardAction Move(Vector2Int from, Vector2Int to, GameObject obj)
    {
        BoardAction action = new();
        action.type = BoardActionType.Move;
        action.position = from;
        action.moveFrom = from;
        action.moveTo = to;
        action.obj = obj;
        action.hidden = false;
        return action;
    }

    public static BoardAction Attack(Vector2Int position, Vector2Int target, GameObject obj)
    {
        BoardAction action = new();
        action.type = BoardActionType.Attack;
        action.position = position;
        action.attackTarget = target;
        action.obj = obj;
        action.hidden = false;
        return action;
    }

    public BoardAction Copy()
    {
        BoardAction action = new();
        action.type = this.type;
        action.obj = this.obj;
        action.position = this.position;
        action.moveFrom = this.moveFrom;
        action.moveTo = this.moveTo;
        action.attackTarget = this.attackTarget;
        action.hidden = false;
        return action;
    }
}

public class TurnPlanner : MonoBehaviour
{
    [System.NonSerialized]
    public Grid grid;

    private Board board;
    private Board aiBoard;
    [System.NonSerialized]
    public TurnPlannerVisuals visuals;
    public GUI_Timeline guiTimeline;

    private List<BoardAction> validActions = new();
    private BoardActionType actionType = BoardActionType.Move;
    private GridSlot selectedSlot = null;
    private bool planning = false;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        Game.Get().clickManager.onClickAny.AddListener(OnClickAny);
        Game.Get().clickManager.onClickNothing.AddListener(OnClickNothing);
        Game.Get().onPlanningStart.AddListener(OnPlanningStart);
        visuals = GetComponentInChildren<TurnPlannerVisuals>();
    }

    void OnPlanningStart()
    {
        planning = true;
        MakeBoardFromScene();
        for (int i = 0; i < 5; ++i)
        {
            TurnPlannerAi.PlanMove(aiBoard);
        }
        selectedSlot = null;
        PlanActions();
    }

    void MakeBoardFromScene()
    {
        board = new();
        board.width = grid.width;
        board.height = grid.height;
        board.squares = new BoardSquare[grid.width * grid.height];
        foreach (var slot in grid.Slots())
        {
            board.squares[slot.position.x + grid.width * slot.position.y] = new BoardSquare(slot);
        }
        aiBoard = board.Copy();
    }

    void DrawBoardActions(List<BoardAction> actions, bool player)
    {
        for (var i = 0; i < actions.Count; ++i)
        {
            var action = actions[i];
            if (action.type == BoardActionType.Move)
            {
                visuals.MovementLine(action.moveFrom, action.moveTo);
                visuals.Ghost(action.moveTo, action.obj);
                visuals.SecondIndicator(action.moveFrom, action.moveTo, i * 2 + (player ? 1 : 2));
            }
        }
    }

    void PlanActions()
    {
        visuals.Clear();
        DrawBoardActions(board.actions, true);
        DrawBoardActions(aiBoard.actions, false);
        if (selectedSlot != null && CanPlan())
        {
            validActions = board.ValidActionsFor(true, selectedSlot.position, this.actionType);
            foreach (var action in validActions)
            {
                if (action.type == BoardActionType.Move)
                {
                    visuals.MoveSlot(action.moveTo);
                }
                else if (action.type == BoardActionType.Attack)
                {
                    visuals.AttackSlot(action.attackTarget);
                }
            }
        }
        else
        {
            validActions = new();
        }
        guiTimeline.UpdateFromBoards(board, aiBoard);
    }

    bool CanPlan()
    {
        return planning && board.actions.Count < 5;
    }

    void OnClickAny(Clickable clickable)
    {
        if (!CanPlan())
        {
            return;
        }
        var gridSlot = clickable.GetComponent<GridSlot>();
        if (gridSlot)
        {
            OnClickGridSlot(gridSlot);
            return;
        }
    }

    void OnClickNothing()
    {
        if (!CanPlan())
        {
            return;
        }
        selectedSlot = null;
        PlanActions();
    }

    void OnClickGridSlot(GridSlot gridSlot)
    {
        var appliedMove = false;
        foreach (var validAction in validActions)
        {
            if (validAction.type == BoardActionType.Move)
            {
                if (validAction.moveTo == gridSlot.position)
                {
                    AddAction(validAction);
                    selectedSlot = gridSlot;
                    PlanActions();
                    appliedMove = true;
                    break;
                }
            }
            else if (validAction.type == BoardActionType.Attack)
            {
                if (validAction.attackTarget == gridSlot.position)
                {
                    AddAction(validAction);
                    selectedSlot = null;
                    PlanActions();
                    appliedMove = true;
                    break;
                }
            }
        }
        if (!appliedMove)
        {
            selectedSlot = gridSlot;
            actionType = BoardActionType.Move;
            PlanActions();
        }
    }

    void AddAction(BoardAction action)
    {
        board.AddAction(action);
    }

    void ClearActions()
    {
        board.ClearActions();
        aiBoard.ClearActions();
        selectedSlot = null;
        PlanActions();
    }

    void UndoAction()
    {
        if (board.actions.Count > 0)
        {
            var lastAction = board.actions[board.actions.Count - 1];
            selectedSlot = grid.At(lastAction.position);
            board.actions.RemoveAt(board.actions.Count - 1);
            PlanActions();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoAction();
        }
        if (Input.GetKeyDown(KeyCode.Space) && board.actions.Count == 5)
        {
            planning = false;
            List<BoardAction> playerActions = new(board.actions);
            List<BoardAction> aiActions = new(aiBoard.actions);
            ClearActions();
            Game.Get().OnPlanningEnd(playerActions, aiActions);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            actionType = BoardActionType.Move;
            PlanActions();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            actionType = BoardActionType.Attack;
            PlanActions();
        }
    }
}