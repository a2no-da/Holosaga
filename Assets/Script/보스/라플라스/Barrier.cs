using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class Barrier : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    public AudioSource spn;
    public AudioSource rem;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;

        skeletonAnimation.AnimationState.SetAnimation(0, "summon", false);
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "laplus_VFX_shield_summon":
                spn.Play();
                break;
            case "laplus_VFX_shield_remove":
                rem.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "summon")
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }

        if (trackEntry.Animation.Name == "remove")
        {
            gameObject.SetActive(false);
        }
    }
}
