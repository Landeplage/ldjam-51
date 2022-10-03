using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusicController : MonoBehaviour
{
    FMODUnity.StudioEventEmitter emitter;

    private void Start() {
        emitter = Singleton.Instance.GetComponent<FMODUnity.StudioEventEmitter>();

        if (emitter.IsPlaying())
            return;

        emitter.Play();
    }
}
