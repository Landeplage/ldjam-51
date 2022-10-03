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
    [SerializeField] EventReference musicFmodEvent;

    EventInstance sweetMusic;

    void Start()
    {
        sweetMusic = FMODUtility.Play(musicFmodEvent, transform.position);
    }

    public void StartVoice1()
    {
        FMODUtility.Play(ambience1FmodEvent, transform.position);
        FMODUtility.Play(voiceOver1FmodEvent, transform.position);
    }

    public void StartVoice2()
    {
        FMODUtility.Play(ambience2FmodEvent, transform.position);
        FMODUtility.Play(voiceOver2FmodEvent, transform.position);
    }

    public void StartVoice3()
    {
        FMODUtility.Play(ambience3FmodEvent, transform.position);
        FMODUtility.Play(voiceOver3FmodEvent, transform.position);
    }

    public void StartVoice4()
    {
        FMODUtility.Play(ambience4FmodEvent, transform.position);
        FMODUtility.Play(voiceOver4FmodEvent, transform.position);
    }

    public void StartVoice5()
    {
        FMODUtility.Play(ambience5FmodEvent, transform.position);
        FMODUtility.Play(voiceOver5FmodEvent, transform.position);
    }

    public void FadeOut()
    {
        sweetMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void End()
    {
        SceneSwitcher.GoTo("GameSingleScreen");
    }
}
