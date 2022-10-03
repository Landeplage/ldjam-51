using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoto : MonoBehaviour
{
    public void GoToLevel(int id) {
        Game.level = id;
        SceneSwitcher.GoTo("GameSingleScreen");
    }
}
