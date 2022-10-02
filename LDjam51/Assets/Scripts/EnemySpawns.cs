using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawns : MonoBehaviour
{
    float timer = 0.0f;

    void Update()
    {
        //timer += Time.deltaTime;
        if (timer > 10.0) 
        {
            var turnPlanner = FindObjectOfType<TurnPlanner>();
            if (turnPlanner.planning)
            {
                timer = 0.0f;

                turnPlanner.PreventUndos();

                var grid = FindObjectOfType<Grid>();
                var slots = grid.Slots();
                for (int i = 0; i < 100; ++i)
                {
                    int index = Random.Range(0, slots.Length);
                    if (!slots[index].entity && slots[index].position.x > 3)
                    {
                        break;
                    }
                }
            }
        }
    }
}
