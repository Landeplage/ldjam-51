using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public BoardSquare[] squares;
    public List<BoardAction> actions = new();
    public Entropy entropy = new();

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
        board.entropy = new(this.entropy);
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
                board.entropy.Apply(action.position.x + action.position.y);
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

    public List<BoardAction> AllValidActions(bool friendly)
    {
        var actions = new List<BoardAction>();
        var applied = ApplyActions();
        foreach (var square in squares)
        {
            actions.AddRange(applied.ValidActionsForInternal(friendly, square.position, BoardActionType.Move, this.actions));
            actions.AddRange(applied.ValidActionsForInternal(friendly, square.position, BoardActionType.Attack, this.actions));
        }
        return actions;
    }

    public List<BoardAction> ValidActionsFor(bool friendly, Vector2Int position, BoardActionType type)
    {
        return ApplyActions().ValidActionsForInternal(friendly, position, type, actions);
    }

    List<BoardAction> ValidActionsForInternal(bool friendly, Vector2Int position, BoardActionType type, List<BoardAction> appliedActions)
    {
        List<Vector2Int> blockedEntities = new();
        foreach (var action in appliedActions)
        {
            if (action.type == BoardActionType.Attack)
            {
                blockedEntities.Add(action.position);
            }
        }

        var actions = new List<BoardAction>();
        var square = At(position);
        if (square.Type() == BoardSquareType.Unit)
        {
            if (square.Friendly() == friendly && !blockedEntities.Contains(square.position))
            {
                if (type == BoardActionType.Move)
                {
                    foreach (var adjacentSquare in Adjacent(square))
                    {
                        var allowedMove = true;
                        foreach (var action in appliedActions)
                        {
                            if ((action.moveFrom == adjacentSquare.position || action.moveTo == adjacentSquare.position) && action.obj == square.obj)
                            {
                                allowedMove = false;
                            }
                        }
                        if (adjacentSquare.Type() == BoardSquareType.Empty && allowedMove)
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

    public Vector2Int ClosestWell(Vector2Int position)
    {
        var closest = new Vector2Int(-1, -1);
        var distance = 0.0f;
        foreach (var square in squares)
        {
            if (square.Type() == BoardSquareType.Well)
            {
                if (((Vector2)square.position - position).magnitude < distance || closest.x == -1)
                {
                    closest = square.position;
                    distance = ((Vector2)square.position - position).magnitude;
                }
            }
        }
        return closest;
    }
}

public enum BoardSquareType
{
    Empty,
    Unit,
    Well,
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
            if (obj.GetComponent<Unit>())
            {
                return BoardSquareType.Unit;
            }
            else
            {
                return BoardSquareType.Well;
            }
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
    public float score = 0.0f;

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
    }

    void DrawBoardActions()
    {
        for (var i = 0; i < board.actions.Count; ++i)
        {
            var action = board.actions[i];
            var second = i + 1;
            GameObject currentObj = null;
            if (selectedSlot != null)
            {
                currentObj = board.ApplyActions().At(selectedSlot.position).obj;
            }
            if (action.type == BoardActionType.Move)
            {
                if (i < board.actions.Count)
                {
                    visuals.Ghost(action.moveTo, action.obj);
                }
            }
            var lastEnemyMove = i == board.actions.Count - 1;
            if (action.obj == currentObj || lastEnemyMove)
            {
                if (action.type == BoardActionType.Move)
                {
                    visuals.MovementLine(action.moveFrom, action.moveTo);
                    if (!lastEnemyMove)
                    {
                        visuals.SecondIndicator(action.moveFrom, action.moveTo, second);
                    }
                }
                else if (action.type == BoardActionType.Attack)
                {
                    visuals.AttackSlot(action.attackTarget, false);
                    if (!lastEnemyMove)
                    {
                        visuals.SecondIndicator(action.position, action.attackTarget, second);
                    }
                }
            }
        }
    }

    void PlanActions(bool isClear = false)
    {
        visuals.Clear();
        DrawBoardActions();
        if (selectedSlot != null && CanPlan())
        {
            validActions = board.ValidActionsFor(true, selectedSlot.position, this.actionType);
            foreach (var action in validActions)
            {
                if (action.type == BoardActionType.Move)
                {
                    visuals.MoveSlot(action.moveTo, true);
                }
                else if (action.type == BoardActionType.Attack)
                {
                    visuals.AttackSlot(action.attackTarget, true);
                }
            }
        }
        else
        {
            validActions = new();
        }
        if (!isClear)
        {
            guiTimeline.UpdateFromBoard(board);
        }
    }

    bool CanPlan()
    {
        return planning && board.actions.Count < 10;
    }

    void OnClickAny(Clickable clickable)
    {
        if (!planning)
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
        if (!planning)
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
                    selectedSlot = gridSlot;
                    AddAction(validAction);
                    appliedMove = true;
                    break;
                }
            }
            else if (validAction.type == BoardActionType.Attack)
            {
                if (validAction.attackTarget == gridSlot.position)
                {
                    AddAction(validAction);
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
        TurnPlannerAi.PlanMove(board, new(board.ApplyActions().entropy));
        PlanActions();
    }

    void ClearActions()
    {
        board.ClearActions();
        selectedSlot = null;
        PlanActions(true);
    }

    void UndoAction()
    {
        if (board.actions.Count > 0)
        {
            var lastAction = board.actions[board.actions.Count - 2];
            if (lastAction.position.x != -1)
            {
                selectedSlot = grid.At(lastAction.position);
            }
            actionType = BoardActionType.Move;
            board.actions.RemoveAt(board.actions.Count - 1);
            board.actions.RemoveAt(board.actions.Count - 1);
            PlanActions();
        }
    }

    void Update()
    {
        if (!planning)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoAction();
        }
        if (Input.GetKeyDown(KeyCode.Space) && board.actions.Count == 10)
        {
            planning = false;
            List<BoardAction> actions = new(board.actions);
            ClearActions();
            Game.Get().OnPlanningEnd(actions);
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
        if (Input.GetKeyDown(KeyCode.W) && CanPlan())
        {
            AddAction(BoardAction.Idle());
        }
    }
}