using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Projectile : MonoBehaviour
{
    public Vector3 target;
    [SerializeField] EventReference fireFmodEvent;
    [SerializeField] EventReference impactFmodEvent;

    private void Start()
    {
        FMODUtility.Play(fireFmodEvent, transform.position);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 10.0f);
        if (Vector3.Distance(transform.position, target) < 1.0f)
        {
            FMODUtility.Play(impactFmodEvent, transform.position);
            Destroy(gameObject);
        }
    }
}
