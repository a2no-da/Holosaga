using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FaunaSeed : MonoBehaviour
{
    private Fauna fauna;
    public string seedName;
    public int seedLev;
    public int PlusSeed;
    public SkeletonAnimation skeletonAnimation;
    public float Power;
    public AudioSource a1;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        //layerMask = LayerMask.GetMask("Tower");
    }

    public void Initialize(Fauna fauna, float Power)
    {
        this.fauna = fauna;
        this.Power = (int)Power;
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "fauna_VFX_pattern1_grow":
                a1.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "heal0" || trackEntry.Animation.Name == "hpup0" ||
            trackEntry.Animation.Name == "speed0" || trackEntry.Animation.Name == "power0")
        {
            Destroy(gameObject);
        }
    }

    public void PlayAni(string Name)
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, Name, false);
        }
    }
}
