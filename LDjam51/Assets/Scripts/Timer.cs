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
            time -= Time.deltaTime;
            if (time < 0.0f)
            {
                //Game.Get().OnPlanningEnd();
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
