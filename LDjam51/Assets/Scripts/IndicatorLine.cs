using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndicatorLine : MonoBehaviour
{
    public TMP_Text text;
    
    public void Init(int second)
    {
        text.text = "" + second;
        text.gameObject.transform.rotation = Quaternion.identity;
    }
}
