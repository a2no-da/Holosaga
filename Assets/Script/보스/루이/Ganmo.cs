using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Ganmo : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] AnimClip;
    private string CurrentAnimation;
    public Lui lui;
    public AudioSource Ho;
    public AudioSource At;

    void Start()
    {
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage1":
                At.Play();
                GO();
                break;
            case "Louie_VFX_pattern2_hooting":
                Ho.Play();
                break;
        }
    }

    void GO()
    {
        lui.GanmoGGo();
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        CurrentAnimation = ""; 
        SetAnimation(0, AnimClip[0], false);
    }

    private void SetAnimation(int trackIndex, AnimationReferenceAsset animation, bool loop)
    {
        if (animation.name.Equals(CurrentAnimation))
            return;
        skeletonAnimation.AnimationState.SetAnimation(trackIndex, animation.name, loop);
        CurrentAnimation = animation.name;
    }

    public void GoMan()
    {
        SetAnimation(0, AnimClip[1], false);
    }
}
