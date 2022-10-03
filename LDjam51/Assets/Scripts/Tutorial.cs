using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    void Start()
    {
        if (Game.level == 1)
        {
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(4, 3), new Vector2Int(6, 4));
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(-1, -1), new Vector2Int(-1, -1));
        }
        if (Game.level == 2)
        {
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(4, 3), new Vector2Int(6, 4));
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(6, 4), new Vector2Int(7, 6));
        }
        if (Game.level == 3)
        {
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(3, 4), new Vector2Int(4, 4));
            FindObjectOfType<TurnPlanner>().ForceMove(new Vector2Int(-1, -1), new Vector2Int(-1, -1));
        }
    }
}
