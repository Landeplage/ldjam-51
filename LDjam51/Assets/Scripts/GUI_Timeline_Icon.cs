using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Timeline_Icon : MonoBehaviour
{
    public GameObject slot;
    public GameObject dot;

    public GameObject Icon_QuestionMark;
    public GameObject Icon_Movement;
    public GameObject Icon_Attack;
    public GameObject Icon_Wait;

    Sprite EnabledSprite;
    public Sprite DisabledSprite;

    void Start()
    {
        EnabledSprite = slot.GetComponent<Image>().sprite;
        ShowDot();
    }

    public void ShowIcon(IconType ico)
    {
        slot.GetComponent<Image>().sprite = EnabledSprite;
        slot.SetActive(true);
        dot.SetActive(false);
        
        Icon_QuestionMark.SetActive(false);
        Icon_Movement.SetActive(false); 
        Icon_Attack.SetActive(false); 
        Icon_Wait.SetActive(false); 
        switch (ico)
        {
            case IconType.QuestionMark: Icon_QuestionMark.SetActive(true); break;
            case IconType.Movement: Icon_Movement.SetActive(true); break;
            case IconType.Attack: Icon_Attack.SetActive(true); break;
            case IconType.Wait: Icon_Wait.SetActive(true); break;
        }
    }

    public void ShowDot()
    {
        slot.SetActive(false);
        dot.SetActive(true);
    }

    public void SetPerformed()
    {
        slot.GetComponent<Image>().sprite = DisabledSprite;
    }
}
