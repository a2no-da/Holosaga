using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Audio;

public class Three : MonoBehaviour
{
    public SkeletonGraphic skeletonAnimation;
    public AudioSource one;
    public AudioClip warning;

    public void Start()
    {
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "warning_below30":
                vfx();
                break;
        }
    }

    void vfx()
    {
        one.PlayOneShot(warning);
    }
}
