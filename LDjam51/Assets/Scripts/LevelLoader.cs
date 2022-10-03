using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    void Start()
    {
        if (Game.level > 9)
        {
            Game.level = 1;
        }
        if (Game.level == 1)
        {
            transform.Find("Level1").gameObject.SetActive(true);
        }
        else if (Game.level == 2)
        {
            transform.Find("Level2").gameObject.SetActive(true);
        }
        else if (Game.level == 3)
        {
            transform.Find("Level3").gameObject.SetActive(true);
        }
        else if (Game.level == 4)
        {
            transform.Find("Level4").gameObject.SetActive(true);
        }
        else if (Game.level == 5)
        {
            transform.Find("Level5").gameObject.SetActive(true);
        }
        else if (Game.level == 6)
        {
            transform.Find("Level6").gameObject.SetActive(true);
        }
        else if (Game.level == 7)
        {
            transform.Find("Level7").gameObject.SetActive(true);
        }
        else if (Game.level == 8)
        {
            transform.Find("Level8").gameObject.SetActive(true);
        }
        else if (Game.level == 9)
        {
            transform.Find("Level9").gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneSwitcher.Restart();
        }
    }
}
