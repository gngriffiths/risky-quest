using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{

    [SerializeField] AudioSource audioSource = null;

        void Start()
        {
            audioSource.Play();
        }

}
