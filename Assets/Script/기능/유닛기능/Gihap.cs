using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Gihap : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
