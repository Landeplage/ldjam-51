using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public BoardSquare[] squares;
    public Entropy entropy = new();

    public Board() { }

    public Board(Board copy)
    {
        width = copy.width;
        height = copy.height;
        squares = new BoardSquare[copy.squares.Length];
        for (int i = 0; i < squares.Length; ++i)
        {
            squares[i] = new BoardSquare(copy.squares[i]);
        }
    }

    public int Index(Vector2Int position)
    {
        return position.x + width * position.y;
    }

    public bool ValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }

    public Board ApplyAction(BoardAction action)
    {
        var board = new Board(this);
        if (action.type == BoardActionType.Move)
        {
            board.squares[Index(action.target)].Take(board.squares[Index(action.position)]);
        }
        board.entropy.Apply(Index(action.position));
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
        foreach (var square in squares)
        {
            actions.AddRange(ValidActionsForInternal(friendly, square.position));
            //actions.AddRange(ValidActionsForInternal(friendly, square.position, BoardActionType.Attack));
        }
        return actions;
    }

    public List<BoardAction> ValidActionsFor(bool friendly, Vector2Int position)
    {
        return ValidActionsForInternal(friendly, position);
    }

    List<BoardAction> ValidActionsForInternal(bool friendly, Vector2Int position)
    {
        var actions = new List<BoardAction>();
        var square = At(position);
        if (square.type == BoardSquareType.Friendly || square.type == BoardSquareType.Enemy)
        {
            var squareFriendly = square.type == BoardSquareType.Friendly;
            if (squareFriendly == friendly)
            {
                foreach (var adjacentSquare in Adjacent(square))
                {
                    if (adjacentSquare.type == BoardSquareType.Empty)
                    {
                        actions.Add(BoardAction.Move(square.position, adjacentSquare.position));
                    }
                }
                foreach (var adjacentSquare in Adjacent(square))
                {
                    if (adjacentSquare.type != BoardSquareType.Empty && adjacentSquare.type != square.type)
                    {
                        actions.Add(BoardAction.Attack(square.position, adjacentSquare.position));
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
            if (square.type == BoardSquareType.Well)
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
    Friendly,
    Enemy,
    Well,
}

public class BoardSquare
{
    public Vector2Int position;
    public BoardSquareType type;
    public int maxHealth;
    public int health;

    public BoardSquare(Vector2Int position, BoardSquareType type)
    {
        this.position = position;
        this.type = type;
        this.maxHealth = 3;
        this.health = 3;
    }

    public BoardSquare(BoardSquare copy)
    {
        position = copy.position;
        type = copy.type;
        maxHealth = copy.maxHealth;
        health = copy.health;
    }

    public void Take(BoardSquare other)
    {
        type = other.type;
        other.type = BoardSquareType.Empty;
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
    public Vector2Int position;
    public Vector2Int target;

    public float score = 0.0f;

    public static BoardAction Idle()
    {
        BoardAction action = new();
        action.type = BoardActionType.Idle;
        action.position = new Vector2Int(-1, -1);
        return action;
    }

    public static BoardAction Move(Vector2Int from, Vector2Int to)
    {
        BoardAction action = new();
        action.type = BoardActionType.Move;
        action.position = from;
        action.target = to;
        return action;
    }

    public static BoardAction Attack(Vector2Int position, Vector2Int target)
    {
        BoardAction action = new();
        action.type = BoardActionType.Attack;
        action.position = position;
        action.target = target;
        return action;
    }

    BoardAction() {}

    BoardAction(BoardAction copy)
    {
        type = copy.type;
        position = copy.position;
        target = copy.target;
    }
}

public class TurnPlanner : MonoBehaviour
{
    [System.NonSerialized]
    public Grid grid;

    private List<Board> previousBoards = new();
    private List<GridSlot> previousSelectedSlots = new();
    private Board board;
    [System.NonSerialized]
    public TurnPlannerVisuals visuals;
    public GUI_Timeline guiTimeline;

    private List<BoardAction> validActions = new();
    private GridSlot selectedSlot = null;
    private bool planning = false;

    private BoardAction aiAction = BoardAction.Idle();

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        Game.Get().clickManager.onClickAny.AddListener(OnClickAny);
        Game.Get().clickManager.onClickNothing.AddListener(OnClickNothing);
        Game.Get().onGameStart.AddListener(OnPlanningStart);
        visuals = GetComponentInChildren<TurnPlannerVisuals>();
    }

    public void OnPlanningStart()
    {
        if (board == null)
        {
            var turnExecutor = FindObjectOfType<TurnExecutor>();
            MakeBoardFromScene();
            turnExecutor.ResetEntities(board);
        }
        planning = true;
        aiAction = TurnPlannerAi.PlanMove(board, new(board.entropy));
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
            if (slot.position.x == 0)
            {
                board.squares[board.Index(slot.position)] = new BoardSquare(slot.position, BoardSquareType.Friendly);
            }
            else if (slot.position.x == 6)
            {
                board.squares[board.Index(slot.position)] = new BoardSquare(slot.position, BoardSquareType.Enemy);
            }
            else
            {
                board.squares[board.Index(slot.position)] = new BoardSquare(slot.position, BoardSquareType.Empty);
            }
        }
    }

    void DrawBoard()
    {
        foreach (var square in board.squares)
        {
            if (square.type == BoardSquareType.Friendly)
            {
                visuals.Debug(square.position, Color.green);
            }
            if (square.type == BoardSquareType.Enemy)
            {
                visuals.Debug(square.position, Color.red);
            }
        }
    }

    void DrawBoardAction(BoardAction action)
    {
        /*if (action.type == BoardActionType.Move)
        {
            //visuals.Ghost(action.position, action.obj);
        }
        if (action.type == BoardActionType.Move)
        {
            visuals.MovementLine(action.position, action.target, second, adjustment);
        }
        else if (action.type == BoardActionType.Attack)
        {
            var opposing = FindOpposingAction(action.position, action.attackTarget);
            LineIndicatorPosition adjustment = LineIndicatorPosition.Center;
            if (opposing.Item1 != null)
            {
                adjustment = opposing.Item2 < i ? LineIndicatorPosition.Left : LineIndicatorPosition.Right;
            }
            visuals.AttackSlot(action.attackTarget, false);
            visuals.AttackLine(action.position, action.attackTarget, second, adjustment);
        }*/
    }

    void PlanActions(bool isClear = false)
    {
        visuals.Clear();
        DrawBoard();
        //DrawBoardActions();
        if (selectedSlot != null && planning)
        {
            validActions = board.ApplyAction(aiAction).ValidActionsFor(true, selectedSlot.position);
            foreach (var action in validActions)
            {
                if (action.type == BoardActionType.Move)
                {
                    visuals.MoveSlot(action.target, true);
                }
                else if (action.type == BoardActionType.Attack)
                {
                    visuals.AttackSlot(action.target, true);
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

    void OnClickAny(Clickable clickable)
    {
        var gridSlot = clickable.GetComponent<GridSlot>();
        if (gridSlot)
        {
            OnClickGridSlot(gridSlot);
            return;
        }
    }

    void OnClickNothing()
    {
        selectedSlot = null;
        PlanActions();
    }

    void OnClickGridSlot(GridSlot gridSlot)
    {
        var appliedMove = false;
        if (planning)
        {
            foreach (var validAction in validActions)
            {
                if (validAction.type == BoardActionType.Move)
                {
                    if (validAction.target == gridSlot.position)
                    {
                        planning = false;
                        StartCoroutine(AddAction(validAction, gridSlot));
                        appliedMove = true;
                        break;
                    }
                }
                else if (validAction.type == BoardActionType.Attack)
                {
                    if (validAction.target == gridSlot.position)
                    {
                        planning = false;
                        StartCoroutine(AddAction(validAction, selectedSlot));
                        appliedMove = true;
                        break;
                    }
                }
            }
        }
        if (!appliedMove)
        {
            selectedSlot = gridSlot;
            PlanActions();
        }
    }

    IEnumerator AddAction(BoardAction action, GridSlot nextSelection)
    {
        visuals.Clear();
        var turnExecutor = FindObjectOfType<TurnExecutor>();
        previousBoards.Add(board);
        previousSelectedSlots.Add(selectedSlot);
        //turnExecutor.ResetEntities(board);
        yield return FindObjectOfType<TurnExecutor>().PlayAction(aiAction);
        board = board.ApplyAction(aiAction);
        //turnExecutor.ResetEntities(board);
        yield return FindObjectOfType<TurnExecutor>().PlayAction(action);
        board = board.ApplyAction(action);
        selectedSlot = nextSelection;
        OnPlanningStart();
    }

    void UndoAction()
    {
        if (previousBoards.Count > 0)
        {
            var turnExecutor = FindObjectOfType<TurnExecutor>();
            board = previousBoards[^1];
            selectedSlot = previousSelectedSlots[^1];
            previousBoards.RemoveAt(previousBoards.Count - 1);
            previousSelectedSlots.RemoveAt(previousSelectedSlots.Count - 1);
            turnExecutor.ResetEntities(board);
            OnPlanningStart();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && planning)
        {
            UndoAction();
        }
    }
}