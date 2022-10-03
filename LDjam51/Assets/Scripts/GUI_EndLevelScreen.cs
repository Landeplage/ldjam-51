using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_EndLevelScreen : MonoBehaviour
{
    public GameObject youWin;
    public GameObject youLose;
    
    public void Show(bool didWin)
    {
        youWin.SetActive(didWin);
        youLose.SetActive(!didWin);
    }
}
