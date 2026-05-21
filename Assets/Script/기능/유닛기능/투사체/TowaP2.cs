using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class TowaP2 : MonoBehaviour
{
    public GameObject th;
    public SkeletonAnimation skeletonAnimation;
    public float speed;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        th.SetActive(false);
    }

    void Update()
    {
        skeletonAnimation.timeScale = speed;
    }
}
