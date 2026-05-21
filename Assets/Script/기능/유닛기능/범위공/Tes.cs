using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class Tes : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public MeshRenderer towerRenderer;
    private MeshRenderer myRenderer;
    public float speed;
    public string ani;
    public Suisei suisei;
    public AudioSource A1;
    public AudioSource A2;
    public AudioSource A3;
    public AudioSource A4;
    public AudioSource A5;
    public AudioSource A6;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        myRenderer = GetComponent<MeshRenderer>();
    }

    public void PlayAnimation()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        if (ani == "upgrade")
        {
            transform.position = new Vector3(0f, 0f, 0);
        }

        skeletonAnimation.AnimationState.SetAnimation(0, ani, false);
    }

    void Update()
    {
        if (towerRenderer != null)
        {
            //myRenderer.sortingOrder = towerRenderer.sortingOrder;
            skeletonAnimation.timeScale = speed;
        }

        if(ani == "upgrade" && transform.position != new Vector3(0f, 0f, 0))
        {
            transform.position = new Vector3(0f, 0f, 0);
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_upgrade":
                if (suisei != null)
                {
                    A6.Play();
                    suisei.Comet();
                }
                break;
            case "damage":
                if (suisei != null)
                {
                    A2.Play();
                    suisei.Tetris();
                }
                break;
            case "suisei_VFX_pattern3_1":
                A1.Play();
                break;
            case "suisei_VFX_pattern3_3":
                A3.Play();
                break;
            case "suisei_VFX_pattern3_4":
                A4.Play();
                break;
            case "suisei_VFX_pattern3_5":
                A5.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        skeletonAnimation.gameObject.SetActive(false);
    }
}
