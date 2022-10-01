using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ClickManager clickManager;
    public SelectManager selectManager;

    public static Game Get()
    {
        var game = FindObjectOfType<Game>();
        if (game == null)
        {
            var newGame = new GameObject();
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
}
