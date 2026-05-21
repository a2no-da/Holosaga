using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class FlareV3 : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public MeshRenderer towerRenderer;
    private MeshRenderer myRenderer;
    public AudioSource A1;
    public float speed;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        myRenderer = GetComponent<MeshRenderer>();
    }

    public void PlayAnimation()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.SetAnimation(0, "animation", false);
    }

    void Update()
    {
        if(towerRenderer != null)
        {
            myRenderer.sortingOrder = towerRenderer.sortingOrder;
            skeletonAnimation.timeScale = speed;
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "hureya_VFX_pattern3":
                if (A1 != null)
                {
                    A1.Play();
                }
                break;
        }
    }
}
