using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_SpawnTimer : MonoBehaviour
{
    public GameObject timerContents;
    public GameObject noNestsContents;
    public Image image;
    public TMP_Text text;
    public TMP_Text nestsText;

    // Start is called before the first frame update
    void Start()
    {
        SetSeconds(1, 0);
    }

    public void SetSeconds(int seconds, int nests)
    {
        if (nests == 0)
        {
            //GetComponent<CanvasGroup>().alpha = 0.5f;
            noNestsContents.SetActive(true);
            timerContents.SetActive(false);
        }
        else
        {
            //GetComponent<CanvasGroup>().alpha = 1.0f;
            noNestsContents.SetActive(false);
            timerContents.SetActive(true);
        }
        seconds %= 10;
        if (seconds == 0)
        {
            seconds = 1;
        }
        text.text = seconds + "s";
        image.fillAmount = seconds / 10.0f;
        nestsText.text = nests + "";
    }
}
