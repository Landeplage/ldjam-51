using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicMuter : MonoBehaviour
{
    [SerializeField]
    EventReference mutedSnapshot;

    FMOD.Studio.EventInstance instance;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            if (instance.isValid()) {
                FMODUtility.Stop(instance);
            } else {
                instance = FMODUtility.Play(mutedSnapshot);
            }
        }
    }
}
