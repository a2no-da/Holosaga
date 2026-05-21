using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class OffDVFX : MonoBehaviour
{
    public GameObject th;
    public SkeletonAnimation skeletonAnimation;
    public AudioSource A1;
    public AudioSource A2;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    public void SetAni(string ani)
    {
        th.SetActive(true);
        //skeletonAnimation.state.ClearTracks();
        skeletonAnimation.AnimationState.SetAnimation(0, ani, false);

        switch (ani)
        {
            case "1":
                A1.Play();
                break;
            case "2":
                A2.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        th.SetActive(false);
    }
}

