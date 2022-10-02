using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_PhaseHeader : MonoBehaviour
{
    public GameObject planning;
    public GameObject executing;
    public TMP_Text turnText;

    // Start is called before the first frame update
    void Start()
    {
        SetPlanning();
        Game.Get().OnTurnStart.AddListener(OnTurnStart);
    }

    public void SetPlanning()
    {
        planning.SetActive(true);
        executing.SetActive(false);
    }

    public void SetExecuting()
    {
        planning.SetActive(false);
        executing.SetActive(true);
    }
    
    public void OnTurnStart(int turn)
    {
        turnText.text = "Turn " + turn;
    }
}
