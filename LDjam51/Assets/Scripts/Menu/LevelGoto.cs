using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class LevelGoto : MonoBehaviour
{
    [SerializeField]
    EventReference eventOnGoTo;

    public void GoToLevel(int id) {
        Game.level = id;
        SceneSwitcher.GoTo("GameSingleScreen");
        FMODUtility.Play(eventOnGoTo);
    }
}
