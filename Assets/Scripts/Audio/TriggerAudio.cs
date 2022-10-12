using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    public FMODUnity.EventReference fmodEvent; public string Event;
    public bool PlayOnAwake;
    public bool PlayOnDestroy;

    public void PlayOneShot()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(Event, gameObject);
    }

    private void Start()
    {
        if (PlayOnAwake)
            PlayOneShot();
    }

    private void OnDestroy()
    {
        if (PlayOnDestroy)
            PlayOneShot();
    }
}
