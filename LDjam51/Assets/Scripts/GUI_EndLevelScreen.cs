using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_EndLevelScreen : MonoBehaviour
{
    public CanvasGroup fader;
    public GameObject youWin;
    public GameObject youLose;
    public GameObject end;

    bool transitionQueued = false;
    
    public void Show(bool didWin, bool lastLevel)
    {
        gameObject.SetActive(true);
        StartCoroutine(PlayFadeIn());
        youWin.SetActive(didWin && !lastLevel);
        end.SetActive(didWin && lastLevel);
        youLose.SetActive(!didWin);
    }

    public void RestartLevel()
    {
        if (!transitionQueued)
        {
            transitionQueued = true;
            SceneSwitcher.Restart();
        }
    }

    public void GoToNextLevel()
    {
        if (!transitionQueued)
        {
            transitionQueued = true;
            Game.level += 1;
            SceneSwitcher.Restart();
        }
    }

    public void GoToMenu()
    {
        if (!transitionQueued)
        {
            transitionQueued = true;
            SceneSwitcher.GoTo("Menu");
        }
    }

    IEnumerator PlayFadeIn()
    {
        fader.alpha = 0.0f;
        for (float i = 0; i < 1.0; i += 0.02f)
        {
            fader.alpha  = Mathf.Lerp(0.0f, 1.0f, i);
            yield return null;
        }
        fader.alpha = 1.0f;
    }
}
