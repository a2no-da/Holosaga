using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Potion : MonoBehaviour
{
    public event Action OnAnimationEnd;
    private SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    public void MoveTo(Vector3 destination, float duration)
    {
        StartCoroutine(MoveToCoroutine(destination, duration));
    }

    private IEnumerator MoveToCoroutine(Vector3 destination, float duration)
    {
        Vector3 startPosition = transform.position;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            yield return null;
        }

        transform.position = destination;
    }

    public void SetAnimation(string animationName)
    {
        skeletonAnimation.AnimationState.Complete -= HandleAnimationComplete;
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        OnAnimationEnd?.Invoke();
        skeletonAnimation.AnimationState.Complete -= HandleAnimationComplete;
        Destroy(gameObject);
    }
}
