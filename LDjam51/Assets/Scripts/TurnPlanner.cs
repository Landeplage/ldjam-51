using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int width;
    public int height;
    public BoardSquare[] squares;
    public List<BoardAction> actions = new();

    void Start()
    {
    }

    Board Copy()
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

    public List<BoardAction> ValidActionsFor(Vector2Int position)
    {
        var actions = new List<BoardAction>();
        var square = At(position);
        if (square.type == BoardSquareType.Unit)
        {
            foreach (var adjacentSquare in Adjacent(square))
            {
                actions.Add(BoardAction.Move(square.position, adjacentSquare.position));
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
    public BoardSquareType type;
    public Vector2Int position;

    BoardSquare() {}

    public BoardSquare(GridSlot slot)
    {
        if (slot.entity == null)
        {
            this.type = BoardSquareType.Empty;
        }
        else
        {
            this.type = BoardSquareType.Unit;
        }
        this.slot = slot;
        this.position = slot.position;
    }

    public void MakeEmpty()
    {
        this.type = BoardSquareType.Empty;
    }

    public void Take(BoardSquare other)
    {
        this.type = other.type;
        other.MakeEmpty();
    }

    public BoardSquare Copy()
    {
        BoardSquare square = new();
        square.slot = this.slot;
        square.type = this.type;
        square.position = this.position;
        return square;
    }
}

public enum BoardActionType
{
    Move,
}

public class BoardAction
{
    public BoardActionType type;
    public Vector2Int moveFrom;
    public Vector2Int moveTo;

    public static BoardAction Move(Vector2Int from, Vector2Int to)
    {
        BoardAction action = new();
        action.type = BoardActionType.Move;
        action.moveFrom = from;
        action.moveTo = to;
        return action;
    }

    public BoardAction Copy()
    {
        BoardAction action = new();
        action.type = this.type;
        action.moveFrom = this.moveFrom;
        action.moveTo = this.moveTo;
        return action;
    }
}

public class TurnPlanner : MonoBehaviour
{
    [System.NonSerialized]
    public Grid grid;

    private Board board;
    public TurnPlannerVisuals visuals;

    private List<BoardAction> validActions = new();

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        Game.Get().clickManager.onClickAny.AddListener(OnClickAny);
        Game.Get().onGameStart.AddListener(MakeBoardFromScene);
        visuals = GetComponentInChildren<TurnPlannerVisuals>();
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
        foreach (var action in board.actions)
        {
            if (action.type == BoardActionType.Move)
            {
                visuals.MovementLine(action.moveFrom, action.moveTo);
            }
        }
    }

    void PlanActionsFor(Vector2Int position)
    {
        validActions = board.ApplyActions().ValidActionsFor(position);
        visuals.Clear();
        DrawBoardActions();
        foreach (var move in validActions)
        {
            visuals.HighlightSlot(move.moveTo);
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

    void OnClickGridSlot(GridSlot gridSlot)
    {
        var appliedMove = false;
        foreach (var validAction in validActions)
        {
            if (validAction.type == BoardActionType.Move)
            {
                if (validAction.moveTo == gridSlot.position)
                {
                    board.AddAction(validAction);
                    PlanActionsFor(gridSlot.position);
                    appliedMove = true;
                    break;
                }
            }
        }
        if (!appliedMove)
        {
            PlanActionsFor(gridSlot.position);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeBoardFromScene();
        }
    }
}
