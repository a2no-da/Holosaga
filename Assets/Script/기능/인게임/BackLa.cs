using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class BackLa : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    public void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        skeletonAnimation.AnimationState.SetAnimation(0, "spawn", false);
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "spawn")
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }
    }
}
