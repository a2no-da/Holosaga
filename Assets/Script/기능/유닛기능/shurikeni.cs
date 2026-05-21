using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class shurikeni : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    public AudioClip[] audioClips;
    public AudioSource audioSource;
    
    void Start()
    {
        PlaySound(0);

        skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        skeletonAnimation.AnimationState.SetAnimation(0, "spawn", false);
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "kanata_VFX_pattern2_2":
                PlaySound(1);
                break;
        }
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < audioClips.Length)
        {
            audioSource.PlayOneShot(audioClips[soundIndex]);
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "spawn")
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }
    }

    public void Reset()
    {
        PlaySound(0);
        if(skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "spawn", false);
        }
    }

    private void OnEnable()
    {
        Reset();
    }
}
