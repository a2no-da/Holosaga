using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Taimu : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    // Start is called before the first frame update
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        skeletonAnimation.AnimationState.Complete -= HandleAnimationComplete;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
