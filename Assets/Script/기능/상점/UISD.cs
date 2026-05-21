using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.UI;

public class UISD : MonoBehaviour
{
    public GameObject me;
    public GameObject fan; 
    public CollectBox collectBox;
    public SkeletonGraphic skeletonGraphic;

    public AudioClip[] audioClips;
    public AudioSource audioSource;

    void Start()
    {
        skeletonGraphic.AnimationState.Event += HandleAnimationEvent;
        skeletonGraphic.AnimationState.Complete += HandleAnimationComplete;
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "boxOpened":
                audioSource.PlayOneShot(audioClips[0]);
                break;
            case "gatcha":
                audioSource.PlayOneShot(audioClips[1]);
                fan.SetActive(true);
                collectBox.Gacha1.SetActive(true);
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
    }
}