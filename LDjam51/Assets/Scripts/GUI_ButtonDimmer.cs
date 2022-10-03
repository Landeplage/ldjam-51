using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ButtonDimmer : MonoBehaviour
{
    public bool interactable = true;
    public CanvasGroup canvasGrp;
    public Button btn;
    public float dimAmount = 0.5f;
    public GameObject noWellsWarning;

    void Start()
    {
        SetInteractable(interactable);
    }
    
    public void SetInteractable(bool value)
    {
        btn.interactable = value;
        canvasGrp.alpha = value ? 1.0f : dimAmount;

        // Hack for wells btn
        if (noWellsWarning)
        {
            noWellsWarning.SetActive(!value && Game.level >= 5);
        }
    }
}
