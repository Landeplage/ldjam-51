using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFade : MonoBehaviour
{
    float alpha = 1.0f;
    bool fadeOut = false;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0.0f);
    }

    public void FadeIn()
    {
        alpha = 0.0f;
    }

    public void FadeOut()
    {
        fadeOut = true;
    }

    void Update()
    {
        if (fadeOut)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime);
        }
        else
        {
            alpha = Mathf.Clamp01(alpha + Time.deltaTime);
        }
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, alpha);
        if (fadeOut && alpha == 0.0f)
        {
            Destroy(GetComponentInParent<BoardEntity>().gameObject);
        }
    }
}
