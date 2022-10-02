using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public static class FMODUtility
{
    #region Playback

    /// <summary>
    /// Plays an event. Returns an event instance that can be used to stop, etc.
    /// </summary>
    /// <param name="reference">FMOD event</param>
    static public EventInstance Play(EventReference reference) {
        if (reference.IsNull)
            return new EventInstance();

        EventInstance instance = RuntimeManager.CreateInstance(reference);
        instance.start();

        return instance;
    }

    /// <summary>
    /// Plays an event in the world. Returns an event instance that can be used to stop, etc.
    /// </summary>
    /// <param name="reference">FMOD event</param>
    /// <param name="position">World position</param>
    static public EventInstance Play(EventReference reference, Vector3 position = default) {
        if (reference.IsNull)
            return new EventInstance();

        EventInstance instance = RuntimeManager.CreateInstance(reference);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();

        return instance;
    }

    static public void PlayAttached(EventReference reference, GameObject gameObject) {
        if (reference.IsNull)
            return;

        if (gameObject == null)
            return;

        RuntimeManager.PlayOneShotAttached(reference, gameObject);
    }

    static public void PlayAndSetParameter(EventReference reference, Vector3 position, string parameterName, float parameterValue) {
        EventInstance instance = RuntimeManager.CreateInstance(reference);
        instance.setParameterByName(parameterName, parameterValue);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        instance.release();
    }

    #endregion

    #region Instances

    static public void Stop(EventInstance instance) {
        if (!instance.isValid())
            return;

        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    static public void MoveInstanceTo(EventInstance instance, GameObject go) {
        if (!instance.isValid())
            return;

        if (go == null)
            return;

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(go));
    }

    #endregion

    #region Global parameters

    static public PARAMETER_ID GetGlobalParameterID(string name) {
        RuntimeManager.StudioSystem.getParameterDescriptionByName(name, out FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription);
        return parameterDescription.id;
    }

    static public void SetGlobalParameterValue(string name, float value) {
        PARAMETER_ID id = GetGlobalParameterID(name);
        SetGlobalParameterValue(id, value);
    }

    static public void SetGlobalParameterValue(PARAMETER_ID id, float value) {
        RuntimeManager.StudioSystem.setParameterByID(id, value);
    }

    static public float SetGlobalParameterValue(string name) {
        PARAMETER_ID id = GetGlobalParameterID(name);
        return GetGlobalParameterValue(id);
    }

    static public float GetGlobalParameterValue(PARAMETER_ID id) {
        RuntimeManager.StudioSystem.getParameterByID(id, out float value);
        return value;
    }

    #endregion

    #region Buses

    static public void BusSetPaused(string busPath, bool paused) {
        Bus bus = RuntimeManager.GetBus(busPath);
        BusSetPaused(bus, paused);
    }

    static public void BusSetPaused(Bus bus, bool paused) {
        if (!bus.isValid()) {
            Debug.LogWarning("FMODUtility: Tried to pause an invalid bus.");
            return;
        }
        bus.setPaused(paused);
    }

    #endregion
}
