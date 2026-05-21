using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using UnityEngine.Audio;
using UnityEngine.UI;

public class upep : MonoBehaviour
{
    public AudioSource L;
    public AudioSource U;

    private SkeletonGraphic skeletonGraphic;

    void Start()
    {
        skeletonGraphic = GetComponent<SkeletonGraphic>();
        skeletonGraphic.AnimationState.Complete += HandleAnimationComplete;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    public void Lone()
    {
        L.Play();
    }

    public void Uone()
    {
        U.Play();
    }
}
