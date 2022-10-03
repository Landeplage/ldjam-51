using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODSceneAudioPlayer : MonoBehaviour
{
    [SerializeField]
    EventReference fmodEvent;

    FMOD.Studio.EventInstance instance;

    private void Awake() {
        SceneSwitcher.onFadeOut.AddListener(OnFadeOut);
    }

    private void Start() {
        instance = FMODUtility.Play(fmodEvent);
    }

    void OnFadeOut() {
        FMODUtility.Stop(instance);
    }
}
