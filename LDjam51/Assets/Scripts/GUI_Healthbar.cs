using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_Healthbar : MonoBehaviour
{
    public Image fill;
    public TMP_Text hpText;

    // Update is called once per frame
    void UpdateBar(int currentHp, int maxHp)
    {
        hpText.text = "" + currentHp;
        fill.fillAmount = Mathf.Clamp(currentHp / maxHp, 0.0f, 1.0f);
    }
}
