using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

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

    public void Log(string title)
    {
        Debug.Log(string.Format("board: {0}", title));
        foreach (var square in squares)
        {
            if (square.type != BoardSquareType.Empty)
            {
                Debug.Log(string.Format("  {0} (health: {1}, max: {2})", square.position, square.health, square.maxHealth));
            }
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
        if (action.type == BoardActionType.Attack)
        {
            board.squares[Index(action.target)].health -= 1;
            if (board.squares[Index(action.target)].health == 0)
            {
                board.squares[Index(action.target)].type = BoardSquareType.Empty;
            }
        }
        if (action.type == BoardActionType.Heal)
        {
            board.squares[Index(action.target)].health = Mathf.Min(board.squares[Index(action.target)].health + 1, board.squares[Index(action.target)].maxHealth);
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
        var squareFriendly = BoardSquare.FriendlyType(square.type);
        if (squareFriendly == friendly)
        {
            if (BoardSquare.MovingType(square.type)) {
                foreach (var adjacentSquare in MoveType(square))
                {
                    if (adjacentSquare.type != BoardSquareType.Blocked)
                    {
                        actions.Add(BoardAction.Move(square.position, adjacentSquare.position, adjacentSquare.type == BoardSquareType.Empty));
                    }
                }
            }
            if (BoardSquare.AttackingType(square.type)) {
                foreach (var adjacentSquare in AttackType(square))
                {
                    var otherFriendly = BoardSquare.FriendlyType(adjacentSquare.type);
                    if ((BoardSquare.AttackableType(adjacentSquare.type) && friendly != otherFriendly) || (adjacentSquare.type == BoardSquareType.Empty && friendly))
                    {
                        actions.Add(BoardAction.Attack(square.position, adjacentSquare.position, adjacentSquare.type != BoardSquareType.Empty));
                    }
                }
            }
            if (BoardSquare.HealingType(square.type)) {
                foreach (var adjacentSquare in HealType(square))
                {
                    var otherFriendly = BoardSquare.FriendlyType(adjacentSquare.type);
                    if ((BoardSquare.AttackableType(adjacentSquare.type) && friendly == otherFriendly) || (adjacentSquare.type == BoardSquareType.Empty && friendly))
                    {
                        actions.Add(BoardAction.Heal(square.position, adjacentSquare.position, adjacentSquare.type != BoardSquareType.Empty));
                    }
                }
            }
        }
        return actions;
    }

    List<BoardSquare> Moves(List<List<BoardSquare>> input)
    {
        List<BoardSquare> moves = new();
        foreach (var list in input)
        {
            foreach (var item in list)
            {
                if (item != null && !moves.Contains(item))
                {
                    moves.Add(item);
                }
            }
        }
        return moves;
    }

    List<BoardSquare> Offsets(BoardSquare square, List<(int, int)> angleDistance)
    {
        var offset = new Vector2Int(0, 0);
        foreach (var (angle, distance) in angleDistance)
        {
            if (angle == 0)
            {
                offset.x -= distance;
            }
            else if (angle == 3)
            {
                offset.x += distance;
            }
            else
            {
                var even = (square.position + offset).y % 2 == 0;
                for (var i = 0; i < distance; ++i)
                {
                    if (angle == 1 || angle == 5)
                    {
                        if (even)
                        {
                            offset.x -= 1;
                        }
                    }
                    else
                    {
                        if (!even)
                        {
                            offset.x += 1;
                        }
                    }
                    if (angle == 1 || angle == 2)
                    {
                        offset.y += 1;
                    }
                    else
                    {
                        offset.y -= 1;
                    }
                    even = !even;
                }
            }
        }
        var list = new List<BoardSquare>();
        var offsetPosition = square.position + offset;
        list.Add(At(offsetPosition));
        return list;
    }

    List<BoardSquare> Adjacent(BoardSquare square)
    {
        return Moves(new List<List<BoardSquare>>
        {
            Offsets(square, new List<(int, int)> { ( 0, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 1 ) }),
        });
    }

    List<BoardSquare> Adjacent2(BoardSquare square)
    {
        return Moves(new List<List<BoardSquare>>
        {
            Offsets(square, new List<(int, int)> { ( 0, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 0, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 0, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 0, 2 ), ( 2, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 2 ), ( 3, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 2 ), ( 4, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 2 ), ( 5, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 2 ), ( 0, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 2 ), ( 1, 1 ) }),
        });
    }

    List<BoardSquare> RangedAttack(BoardSquare square)
    {
        return Moves(new List<List<BoardSquare>>
        {
            Offsets(square, new List<(int, int)> { ( 0, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 0, 2 ), ( 2, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 2 ), ( 3, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 2 ), ( 4, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 2 ), ( 5, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 2 ), ( 0, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 2 ), ( 1, 1 ) }),
            /*Offsets(square, new List<(int, int)> { ( 0, 3 ) }),
            Offsets(square, new List<(int, int)> { ( 0, 3 ), ( 2, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 0, 3 ), ( 2, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 3 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 3 ), ( 3, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 1, 3 ), ( 3, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 3 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 3 ), ( 4, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 2, 3 ), ( 4, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 3 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 3 ), ( 5, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 3, 3 ), ( 5, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 3 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 3 ), ( 0, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 4, 3 ), ( 0, 2 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 3 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 3 ), ( 1, 1 ) }),
            Offsets(square, new List<(int, int)> { ( 5, 3 ), ( 1, 2 ) }),*/
        });
    }

    List<BoardSquare> AttackType(BoardSquare square)
    {
        if (square.type == BoardSquareType.FriendlyMelee)
        {
            return Adjacent(square);
        }
        else if (square.type == BoardSquareType.FriendlyRange)
        {
            return RangedAttack(square);
        }
        else if (square.type == BoardSquareType.Enemy)
        {
            return Adjacent(square);
        }
        else
        {
            return new();
        }
    }

    List<BoardSquare> RangedHeal(BoardSquare square)
    {
        return Adjacent2(square);
    }

    List<BoardSquare> MoveType(BoardSquare square)
    {
        if (square.type == BoardSquareType.FriendlyRange || square.type == BoardSquareType.FriendlyMelee)
        {
            return Adjacent2(square);
        }
        else
        {
            return Adjacent(square);
        }
    }

    List<BoardSquare> HealType(BoardSquare square)
    {
        if (square.type == BoardSquareType.FriendlyHealer)
        {
            return RangedHeal(square);
        }
        else
        {
            return new();
        }
    }

    public Vector2Int ClosestAiInterest(Vector2Int position, BoardAiType aiType)
    {
        var closest = new Vector2Int(-1, -1);
        var distance = 0.0f;
        foreach (var square in squares)
        {
            var interest = false;
            if (aiType == BoardAiType.UnitFocus)
            {
                interest = BoardSquare.FriendlyType(square.type) && square.type != BoardSquareType.Well;
            }
            else if (aiType == BoardAiType.WellFocus)
            {
                interest = square.type == BoardSquareType.Well;
            }
            else if (aiType == BoardAiType.Any)
            {
                interest = BoardSquare.FriendlyType(square.type);
            }
            if (interest)
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
    Blocked,
    FriendlyMelee,
    FriendlyRange,
    FriendlyHealer,
    Enemy,
    Well,
    EnemyWell,
}

public enum BoardAiType
{
    Any,
    UnitFocus,
    WellFocus,
}

public class BoardSquare
{
    public Vector2Int position;
    public BoardSquareType type;
    public int maxHealth;
    public int health;
    public BoardAiType aiType = BoardAiType.Any;

    public BoardSquare(Vector2Int position, BoardSquareType type)
    {
        this.position = position;
        this.type = type;
        maxHealth = 2;
        if (type == BoardSquareType.FriendlyMelee)
        {
            maxHealth = 4;
        }
        else if (type == BoardSquareType.Well)
        {
            maxHealth = 5;
        }
        else if (type == BoardSquareType.EnemyWell)
        {
            maxHealth = 5;
        }
        else if (type == BoardSquareType.Enemy)
        {
            maxHealth = 2;
        }
        else if (type == BoardSquareType.FriendlyHealer)
        {
            maxHealth = 1;
        }
        health = maxHealth;
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
        health = other.health;
        maxHealth = other.maxHealth;
        aiType = other.aiType;
        other.type = BoardSquareType.Empty;
    }

    static public bool FriendlyType(BoardSquareType type)
    {
        return type == BoardSquareType.Well || type == BoardSquareType.FriendlyMelee || type == BoardSquareType.FriendlyRange || type == BoardSquareType.FriendlyHealer;
    }

    static public bool EnemyType(BoardSquareType type)
    {
        return type == BoardSquareType.EnemyWell || type == BoardSquareType.Enemy;
    }

    static public bool AttackingType(BoardSquareType type)
    {
        return type == BoardSquareType.FriendlyMelee || type == BoardSquareType.FriendlyRange || type == BoardSquareType.Enemy;
    }

    static public bool HealingType(BoardSquareType type)
    {
        return type == BoardSquareType.FriendlyHealer;
    }

    static public bool AttackableType(BoardSquareType type)
    {
        return FriendlyType(type) || EnemyType(type);
    }

    static public bool MovingType(BoardSquareType type)
    {
        return type == BoardSquareType.FriendlyMelee || type == BoardSquareType.FriendlyRange || type == BoardSquareType.FriendlyHealer || type == BoardSquareType.Enemy;
    }
}

public enum BoardActionType
{
    Idle,
    Move,
    Attack,
    Heal,
}

public class BoardAction
{
    public BoardActionType type;
    public Vector2Int position;
    public Vector2Int target;
    public bool enabled;

    public float score = 0.0f;

    public static BoardAction Idle()
    {
        BoardAction action = new();
        action.type = BoardActionType.Idle;
        action.position = new Vector2Int(-1, -1);
        return action;
    }

    public static BoardAction Move(Vector2Int from, Vector2Int to, bool enabled)
    {
        BoardAction action = new();
        action.type = BoardActionType.Move;
        action.position = from;
        action.target = to;
        action.enabled = enabled;
        return action;
    }

    public static BoardAction Attack(Vector2Int position, Vector2Int target, bool enabled)
    {
        BoardAction action = new();
        action.type = BoardActionType.Attack;
        action.position = position;
        action.target = target;
        action.enabled = enabled;
        return action;
    }

    public static BoardAction Heal(Vector2Int position, Vector2Int target, bool enabled)
    {
        BoardAction action = new();
        action.type = BoardActionType.Heal;
        action.position = position;
        action.target = target;
        action.enabled = enabled;
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
    private List<Board> redoBoards = new();
    private List<GridSlot> previousSelectedSlots = new();
    private List<GridSlot> redoSelectedSlots = new();
    private Board board;
    [System.NonSerialized]
    public TurnPlannerVisuals visuals;
    public GUI_Timeline guiTimeline;
    public GUI_UndoBuffer guiUndoBuffer;
    public GUI_SpawnTimer guiSpawnTimer;

    private List<BoardAction> validActions = new();
    private int appliedActions = 0;
    private int currentSecond = 1;
    private GridSlot selectedSlot = null;

    [System.NonSerialized]
    public bool planning = false;

    private List<(Vector2Int, Vector2Int)> forcedMoves = new();

    [SerializeField] EventReference undoFmodEvent;
    [SerializeField] EventReference redoFmodEvent;
    [SerializeField] EventReference victoryFmodEvent;
    [SerializeField] EventReference defeatFmodEvent;
    [SerializeField] EventReference enemySpawnFmodEvent;

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
        if (Win())
        {
            FindObjectOfType<Tutorial>().End();
            FMODUtility.Play(victoryFmodEvent, transform.position);
            Game.level += 1;
            SceneSwitcher.Restart();
        }
        else if (Lost())
        {
            FMODUtility.Play(defeatFmodEvent, transform.position);
            planning = true;
        }
        else
        {
            planning = true;
            PlanActions();
        }
    }

    void MakeBoardFromScene()
    {
        board = new();
        board.width = grid.width;
        board.height = grid.height;
        board.squares = new BoardSquare[grid.width * grid.height];
        foreach (var slot in grid.Slots())
        {
            if (slot.entity && slot.entity.GetComponent<BoardEntity>())
            {
                var boardEntity = slot.entity.GetComponent<BoardEntity>();
                board.squares[board.Index(slot.position)] = new BoardSquare(slot.position, boardEntity.type);
                Destroy(slot.entity.gameObject);
                slot.entity = null;
            }
            else
            {
                board.squares[board.Index(slot.position)] = new BoardSquare(slot.position, BoardSquareType.Empty);
            }
        }
    }

    void DrawBoardVisuals()
    {
        if (forcedMoves.Count > 0)
        {
            if (!selectedSlot)
            {
                visuals.Arrow(forcedMoves[0].Item1);
            }
            else
            {
                visuals.Arrow(forcedMoves[0].Item2);
            }
        }
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
        DrawBoardVisuals();
        if (planning)
        {
            validActions = new();
            if (selectedSlot)
            {
                validActions = board.ValidActionsFor(true, selectedSlot.position);
            }
            List<Vector2Int> attackSquares = new();
            foreach (var action in board.AllValidActions(true))
            {
                var noneSelected = selectedSlot == null || board.At(selectedSlot.position).type == BoardSquareType.Empty;
                var selected = selectedSlot != null && action.position == selectedSlot.position;
                var allowHover = true;
                if (forcedMoves.Count > 0)
                {
                    if (forcedMoves[0].Item2 != action.target || selectedSlot == null)
                    {
                        allowHover = false;
                    }
                }
                if (action.type == BoardActionType.Move && action.enabled)
                {
                    if (selected)
                    {
                        visuals.MoveSlot(action.target, action.enabled && allowHover);
                    }
                }
                else if (action.type == BoardActionType.Attack)
                {
                    if (noneSelected || selected)
                    {
                        if (!attackSquares.Contains(action.target))
                        {
                            visuals.AttackSlot(action.target, false);
                            attackSquares.Add(action.target);
                        }
                    }
                }
                else if (action.type == BoardActionType.Heal)
                {
                    if (selected)
                    {
                        if (!attackSquares.Contains(action.target))
                        {
                            visuals.HealSlot(action.target, false);
                            attackSquares.Add(action.target);
                        }
                    }
                }
            }
        }
        else
        {
            validActions = new();
        }
        if (!isClear)
        {
            guiTimeline.UpdateSlots(appliedActions);
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
                if (validAction.enabled)
                {
                    if (validAction.type == BoardActionType.Move)
                    {
                        if (forcedMoves.Count > 0)
                        {
                            if (validAction.target != forcedMoves[0].Item2)
                            {
                                continue;
                            }
                        }
                        if (validAction.target == gridSlot.position)
                        {
                            planning = false;
                            StartCoroutine(AddAction(validAction, gridSlot));
                            appliedMove = true;
                            break;
                        }
                    }
                    /*else if (validAction.type == BoardActionType.Attack)
                    {
                        if (validAction.target == gridSlot.position)
                        {
                            planning = false;
                            StartCoroutine(AddAction(validAction, selectedSlot));
                            appliedMove = true;
                            break;
                        }
                    }*/
                }
            }
        }
        if (!appliedMove)
        {
            selectedSlot = gridSlot;
            if (selectedSlot != null)
            {
                if (board != null && board.At(selectedSlot.position) != null && (!BoardSquare.FriendlyType(board.At(selectedSlot.position).type) || board.At(selectedSlot.position).type == BoardSquareType.Well))
                {
                    selectedSlot = null;
                }
            }
            if (forcedMoves.Count > 0 && selectedSlot)
            {
                if (selectedSlot.position != forcedMoves[0].Item1)
                {
                    selectedSlot = null;
                }
            }
            PlanActions();
        }
    }

    IEnumerator AddAction(BoardAction action, GridSlot nextSelection)
    {
        FindObjectOfType<Tutorial>().Next();
        if (forcedMoves.Count > 0)
        {
            forcedMoves.RemoveAt(0);
        }

        visuals.Clear();
        var turnExecutor = FindObjectOfType<TurnExecutor>();
        previousBoards.Add(board);
        previousSelectedSlots.Add(selectedSlot);
        redoBoards = new();
        redoSelectedSlots = new();

        appliedActions += 1;
        guiTimeline.UpdateSlots(appliedActions);
        yield return turnExecutor.PlayAction(action);
        board = board.ApplyAction(action);

        var autoActions = board.AllValidActions(true);
        for (var i = 0; i < autoActions.Count; ++i)
        {
            if (autoActions[i].type == BoardActionType.Heal)
            {
                autoActions.RemoveAt(i);
                i--;
            }
        }
        foreach (var autoAction in autoActions)
        {
            if (autoAction.type == BoardActionType.Attack && autoAction.enabled && board.At(autoAction.target).type != BoardSquareType.Empty)
            {
                yield return turnExecutor.PlayAction(autoAction);
                board = board.ApplyAction(autoAction);
            }
        }

        appliedActions += 1;
        yield return new WaitForSeconds(0.25f);
        guiUndoBuffer.QueueNextActionStart(BlockOwner.AI);
        guiSpawnTimer.SetSeconds(++currentSecond, NestsCount());
        yield return new WaitForSeconds(0.25f);
        List<Vector2Int> enemies = new();
        foreach (var square in board.squares)
        {
            if (board.At(square.position).type == BoardSquareType.Enemy)
            {
                enemies.Add(square.position);
            }
        }
        foreach (var enemy in enemies)
        {
            var aiAction = TurnPlannerAi.PlanMovesFor(board, enemy);
            guiTimeline.UpdateSlots(appliedActions);
            yield return turnExecutor.PlayAction(aiAction);
            board = board.ApplyAction(aiAction);
        }

        autoActions = board.AllValidActions(false);
        foreach (var autoAction in autoActions)
        {
            if (autoAction.type == BoardActionType.Attack && autoAction.enabled && board.At(autoAction.target).type != BoardSquareType.Empty)
            {
                yield return turnExecutor.PlayAction(autoAction);
                board = board.ApplyAction(autoAction);
            }
        }

        autoActions = board.AllValidActions(true);
        BoardAction healAction = null;
        for (var i = 0; i < autoActions.Count; ++i)
        {
            if (autoActions[i].type == BoardActionType.Heal)
            {
                if (autoActions[i].enabled && board.At(autoActions[i].target).type != BoardSquareType.Empty)
                {
                    if ((board.At(autoActions[i].target).health / (float)board.At(autoActions[i].target).maxHealth) < 1.0)
                    {
                        if (healAction == null || (board.At(autoActions[i].target).health / (float)board.At(autoActions[i].target).maxHealth) < (board.At(healAction.target).health / (float)board.At(healAction.target).maxHealth))
                        {
                            healAction = autoActions[i];
                        }
                    }
                }
                autoActions.RemoveAt(i);
                i--;
            }
        }
        yield return new WaitForSeconds(0.25f);
        guiUndoBuffer.QueueNextActionStart(BlockOwner.Player);
        yield return new WaitForSeconds(0.25f);

        if (healAction != null)
        {
            yield return turnExecutor.PlayAction(healAction);
            board = board.ApplyAction(healAction);
        }

        selectedSlot = nextSelection;
        if (appliedActions == 10)
        {
            appliedActions = 0;
            guiTimeline.UpdateSlots(appliedActions);
            SpawnEnemy();
        }
        
        guiSpawnTimer.SetSeconds(++currentSecond, NestsCount());
        
        OnPlanningStart();
        FindObjectOfType<Tutorial>().Next();
    }

    public void WaitAction()
    {
        if (!planning)
        {
            return;
        }
        
        var canWait = true;
        if (forcedMoves.Count > 0)
        {
            if (forcedMoves[0].Item1 != new Vector2Int(-1, -1) || forcedMoves[0].Item2 != new Vector2Int(-1, -1))
            {
                canWait = false;
            }
        }
        if (canWait)
        {
            planning = false;
            StartCoroutine(AddAction(BoardAction.Idle(), selectedSlot));
        }
    }

    public void UndoAction()
    {
        if (!planning)
        {
            return;
        }

        if (previousBoards.Count > 0)
        {
            FMODUtility.Play(undoFmodEvent, transform.position);
            var turnExecutor = FindObjectOfType<TurnExecutor>();
            redoBoards.Add(board);
            redoSelectedSlots.Add(selectedSlot);
            board = previousBoards[^1];
            selectedSlot = previousSelectedSlots[^1];
            previousBoards.RemoveAt(previousBoards.Count - 1);
            previousSelectedSlots.RemoveAt(previousSelectedSlots.Count - 1);
            appliedActions -= 2;
            currentSecond -= 2;
            guiSpawnTimer.SetSeconds(currentSecond, NestsCount());
            if (appliedActions < 0)
            {
                appliedActions += 10;
            }
            guiUndoBuffer.QueueRemovePlayerAction();
            guiTimeline.UpdateSlots(appliedActions);
            turnExecutor.ResetEntities(board);
            PulseWells();
            OnPlanningStart();
        }
    }

    void RedoAction()
    {
        if (redoBoards.Count > 0)
        {
            FMODUtility.Play(redoFmodEvent, transform.position);
            var turnExecutor = FindObjectOfType<TurnExecutor>();
            previousBoards.Add(board);
            previousSelectedSlots.Add(selectedSlot);
            board = redoBoards[^1];
            selectedSlot = redoSelectedSlots[^1];
            redoBoards.RemoveAt(redoBoards.Count - 1);
            redoSelectedSlots.RemoveAt(redoSelectedSlots.Count - 1);
            appliedActions += 2;
            if (appliedActions > 10)
            {
                appliedActions -= 10;
            }
            guiTimeline.UpdateSlots(appliedActions);
            turnExecutor.ResetEntities(board);
            OnPlanningStart();
        }
    }

    public void PreventUndos()
    {
        previousBoards = new();
        previousSelectedSlots = new();
    }

    public bool CanUndo()
    {
        foreach (var square in board.squares)
        {
            if (square.type == BoardSquareType.Well)
            {
                return true;
            }
        }
        return false;
    }

    void PulseWells()
    {
        foreach (var square in board.squares)
        {
            if (square.type == BoardSquareType.Well)
            {
                grid.At(square.position).entity.GetComponentInChildren<Animator>().Play("Idle");
                grid.At(square.position).entity.GetComponentInChildren<Animator>().Play("Brightsight");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && planning && CanUndo())
        {
            UndoAction();
        }
        if (Input.GetKeyDown(KeyCode.Y) && planning && CanUndo())
        {
            //RedoAction();
        }
        if (Input.GetKeyDown(KeyCode.Space) && planning)
        {
            WaitAction();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Game.level += 1;
            SceneSwitcher.Restart();
        }
        if (Input.GetKeyDown(KeyCode.O) && Game.level > 1)
        {
            Game.level -= 1;
            SceneSwitcher.Restart();
        }
    }

    int NestsCount()
    {
        int result = 0;
        foreach (var square in board.squares)
        {
            if (board.At(square.position).type == BoardSquareType.EnemyWell)
            {
                result++;
            }
        }
        return result;
    }

    public void SpawnEnemy()
    {
        List<Vector2Int> enemyWells = new();
        foreach (var square in board.squares)
        {
            if (board.At(square.position).type == BoardSquareType.EnemyWell)
            {
                enemyWells.Add(square.position);
            }
        }
        Entropy entropy = new(board.entropy);
        foreach (var enemyWell in enemyWells)
        {
            for (int i = 0; i < 100; ++i)
            {
                int index = (int)(entropy.Next() * board.squares.Length);
                var distance = ((Vector2)enemyWell - board.squares[index].position).magnitude;
                if (board.squares[index].type == BoardSquareType.Empty && distance < 3)
                {
                    var position = board.squares[index].position;
                    var turnExecutor = FindObjectOfType<TurnExecutor>();
                    board.squares[board.Index(position)] = new BoardSquare(position, BoardSquareType.Enemy);
                    var r = entropy.Next();
                    if (r < 0.45)
                    {
                        board.squares[board.Index(position)].aiType = BoardAiType.UnitFocus;
                    }
                    else if (r < 0.9)
                    {
                        board.squares[board.Index(position)].aiType = BoardAiType.WellFocus;
                    }
                    else
                    {
                        board.squares[board.Index(position)].aiType = BoardAiType.Any;
                    }
                    turnExecutor.ResetEntities(board);
                    FMODUtility.Play(enemySpawnFmodEvent, grid.At(position).transform.position);
                    break;
                }
            }
        }
    }

    public bool Win()
    {
        foreach (var square in board.squares)
        {
            if (BoardSquare.EnemyType(square.type))
            {
                return false;
            }
        }
        return true;
    }

    public bool Lost()
    {
        foreach (var square in board.squares)
        {
            if (BoardSquare.FriendlyType(square.type))
            {
                return false;
            }
        }
        return true;
    }

    public void ForceMove(Vector2Int from, Vector2Int to)
    {
        forcedMoves.Add((from, to));
    }
}