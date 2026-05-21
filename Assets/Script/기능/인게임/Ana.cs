using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Ana : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AudioClip round;
    public AudioClip count;
    public AudioClip start;
    public AudioClip boss;
    public AudioClip cl;
    public AudioSource AudioSource;

    void Start()
    {
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "nextRound":
                SoundRound();
                break;
            case "count":
                SoundCount();
                break;
            case "gameStart":
                SoundStart();
                break;
            case "boss":
                SoundBoss();
                break;
            case "clear":
                SoundCl();
                break;
        }
    }

    void SoundRound()
    {
        AudioSource.PlayOneShot(round);
    }

    void SoundCount()
    {
        AudioSource.PlayOneShot(count);
    }

    void SoundStart()
    {
        AudioSource.PlayOneShot(start);
    }

    void SoundBoss()
    {
        AudioSource.PlayOneShot(boss);
    }

    void SoundCl()
    {
        AudioSource.PlayOneShot(cl);
    }
}
