using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAura : MonoBehaviour
{
    public float timeAlive = 0.0f;

    void Start()
    {
        GetComponent<Animator>().Play("Heal");
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > 1.0)
        {
            Destroy(gameObject);
        }
    }
}
