using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Intro : MonoBehaviour
{
    [SerializeField] EventReference ambience1FmodEvent;
    [SerializeField] EventReference ambience2FmodEvent;
    [SerializeField] EventReference ambience3FmodEvent;
    [SerializeField] EventReference ambience4FmodEvent;
    [SerializeField] EventReference ambience5FmodEvent;
    [SerializeField] EventReference voiceOver1FmodEvent;
    [SerializeField] EventReference voiceOver2FmodEvent;
    [SerializeField] EventReference voiceOver3FmodEvent;
    [SerializeField] EventReference voiceOver4FmodEvent;
    [SerializeField] EventReference voiceOver5FmodEvent;

    List<EventInstance> sweetSounds = new();
    bool ending = false;

    public void StartVoice1()
    {
        sweetSounds.Add(FMODUtility.Play(ambience1FmodEvent, transform.position));
        sweetSounds.Add(FMODUtility.Play(voiceOver1FmodEvent, transform.position));
    }

    public void StartVoice2()
    {
        sweetSounds.Add(FMODUtility.Play(ambience2FmodEvent, transform.position));
        sweetSounds.Add(FMODUtility.Play(voiceOver2FmodEvent, transform.position));
    }

    public void StartVoice3()
    {
        sweetSounds.Add(FMODUtility.Play(ambience3FmodEvent, transform.position));
        sweetSounds.Add(FMODUtility.Play(voiceOver3FmodEvent, transform.position));
    }

    public void StartVoice4()
    {
        sweetSounds.Add(FMODUtility.Play(ambience4FmodEvent, transform.position));
        sweetSounds.Add(FMODUtility.Play(voiceOver4FmodEvent, transform.position));
    }

    public void StartVoice5()
    {
        sweetSounds.Add(FMODUtility.Play(ambience5FmodEvent, transform.position));
        sweetSounds.Add(FMODUtility.Play(voiceOver5FmodEvent, transform.position));
    }

    public void FadeOut()
    {
        foreach (var sweetSound in sweetSounds)
        {
            sweetSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void End()
    {
        ending = true;
        SceneSwitcher.GoTo("GameSingleScreen");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !ending)
        {
            FadeOut();
            End();
        }
    }
}
