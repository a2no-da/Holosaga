using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class Angel : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public AudioSource A1;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "abilityCard_a8":
                A1.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
