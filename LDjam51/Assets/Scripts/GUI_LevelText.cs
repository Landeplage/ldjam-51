using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_LevelText : MonoBehaviour
{
    public TMP_Text text;

    void Start()
    {
        text.text = "Level " + Game.level + "/" + Game.maxLevels;
    }
}
