using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public ClickManager clickManager;
    public SelectManager selectManager;

    public UnityEvent onSetupGrid = new();
    public UnityEvent onGameStart = new();
    public UnityEvent<int> OnTurnStart = new();

    private float timeAlive = 0.0f;
    private bool gameStarted = false;
    private int turnNum = 1;

    static public int level = 1;
    static public int maxLevels = 10;

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
            onSetupGrid.Invoke();
            onGameStart.Invoke();
            OnTurnStart.Invoke(turnNum);
            gameStarted = true;
        }
    }
}
