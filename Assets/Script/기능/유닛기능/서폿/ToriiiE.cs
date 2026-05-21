using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class ToriiiE : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        SP();
    }

    public void SP()
    {
        float localY = transform.position.y;

        if (Mathf.Approximately(localY, 0.6600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -2;
        }
        else if (Mathf.Approximately(localY, -1.36f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 0;
        }
        else if (Mathf.Approximately(localY, -3.38f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 2;
        }
        else if (Mathf.Approximately(localY, -5.4f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 4;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}

