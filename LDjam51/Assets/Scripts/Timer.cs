using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float time = -1.0f;

    void Start()
    {
        Game.Get().onPlanningStart.AddListener(OnPlanningStart);
    }

    void OnPlanningStart()
    {
        time = 10.0f;
    }

    void Update()
    {
        if (time >= 0.0f)
        {
            var lastTime = time;
            time -= Time.deltaTime;
            if (time < 0.0f)
            {
                Game.Get().onExecutionStart.Invoke();
            }
            if (lastTime > 9.0f && time <= 9.0f)
            {
                Game.Get().onAiPlanMove.Invoke();
            }
            if (lastTime > 7.0f && time <= 7.0f)
            {
                Game.Get().onAiPlanMove.Invoke();
            }
            if (lastTime > 5.0f && time <= 5.0f)
            {
                Game.Get().onAiPlanMove.Invoke();
            }
            if (lastTime > 3.0f && time <= 3.0f)
            {
                Game.Get().onAiPlanMove.Invoke();
            }
            if (lastTime > 1.0f && time <= 1.0f)
            {
                Game.Get().onAiPlanMove.Invoke();
            }
        }
        var text = GetComponent<TMPro.TextMeshPro>();
        if (time >= 0.0f)
        {
            text.text = time.ToString();
        }
        else
        {
            text.text = "";
        }
    }
}
