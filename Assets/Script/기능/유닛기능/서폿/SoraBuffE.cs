using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class SoraBuffE : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public bool ngna;
    private int currentSortingOrder;
    private float previousLocalY;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        previousLocalY = transform.parent.position.y;
        SP();
    }

    public void SP()
    {
        float localY = transform.parent.position.y;

        if (ngna)
        {
            if (Mathf.Approximately(localY, 0.8600001f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = -2;
            }
            else if (Mathf.Approximately(localY, -1.16f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = 0;
            }
            else if (Mathf.Approximately(localY, -3.18f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = 2;
            }
            else if (Mathf.Approximately(localY, -5.2f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = 4;
            }
            else
            {
                return;
            }
        }
        else
        {
            if (Mathf.Approximately(localY, 0.8600001f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = -4;
            }
            else if (Mathf.Approximately(localY, -1.16f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = -2;
            }
            else if (Mathf.Approximately(localY, -3.18f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = 0;
            }
            else if (Mathf.Approximately(localY, -5.2f))
            {
                skeletonAnimation.GetComponent<Renderer>().sortingOrder = 2;
            }
            else
            {
                return;
            }
        }

        previousLocalY = localY;
    }

    void Update()
    {
        float currentLocalY = transform.parent.position.y;

        if (currentLocalY != previousLocalY)
        {
            SP();
        }

        if (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "back_end" || trackEntry.Animation.Name == "front_end")
        {
            Destroy(gameObject); 
            return; 
        }

        if (ngna)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "front", true);
        }
        else
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "back", true);
        }
    }

    public void EEDD()
    {
        if (ngna)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "front_end", false);
        }
        else
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "back_end", false);
        }
    }
}

