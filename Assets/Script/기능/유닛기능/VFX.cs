using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class VFX : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public EventDataReferenceAsset One;
    public EventDataReferenceAsset Two;
    public AudioSource one;
    public AudioSource two;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += AnimationStateOnEvent;
    }

    void Update()
    {
        if (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    private void AnimationStateOnEvent(TrackEntry trackentry, Spine.Event e)
    {
        if (e.Data == One.EventData)
        {
            if (one != null)
            {
                if (one.clip != null)
                {
                    one.Play();
                }
            }
        }

        if (e.Data == Two.EventData)
        {
            two.Play();
        }
    }
}
