using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;

    //SFX
    public AudioClip inGameButtonSound;
    public AudioClip mainMenuButtonSound;
    public AudioClip pawnHealSound;
    public AudioClip pawnMoveSound;
    public AudioClip portalSound;
    public AudioClip tileCaptureSound;

    private void Awake()
    {
        instance = this;
    }

    public void PlayLoop(AudioClip clip)
    {
        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.Play();

    }
}
