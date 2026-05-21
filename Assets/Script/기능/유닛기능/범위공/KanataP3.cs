using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class KanataP3 : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public AudioClip[] audioClips; 
    public AudioSource audioSource;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "kanata_VFX_pattern3_1":
                PlaySound(0); 
                break;
            case "kanata_VFX_pattern3_2":
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
        Destroy(gameObject);
    }
}
