using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public ClickManager clickManager;
    public SelectManager selectManager;

    public UnityEvent onGameStart = new();
    public UnityEvent<int> OnTurnStart = new();

    public UnityEvent onPlanningStart = new();
    public UnityEvent onExecutionStart = new();

    private float timeAlive = 0.0f;
    private bool gameStarted = false;
    private int turnNum = 1;

    public static Game Get()
    {
        var game = FindObjectOfType<Game>();
        if (game == null)
        {
            GameObject newGame = new();
            newGame.name = "Game";
            game = newGame.AddComponent<Game>();
            game.clickManager = newGame.AddComponent<ClickManager>();
            game.selectManager = newGame.AddComponent<SelectManager>();
            return game;
        }
        else
        {
            return game;
        }
    }

    public void Update()
    {
        timeAlive += Time.deltaTime;
        if (!gameStarted && timeAlive > 0.1f)
        {
            onGameStart.Invoke();
            OnTurnStart.Invoke(turnNum);
            onPlanningStart.Invoke();
            gameStarted = true;
        }
    }

    public void OnPlanningEnd(List<BoardAction> actions)
    {
        onExecutionStart.Invoke();
        FindObjectOfType<TurnExecutor>().Go(actions);
    }

    public void OnExecutionEnd()
    {
        OnTurnStart.Invoke(++turnNum);
        onPlanningStart.Invoke();
    }
}
