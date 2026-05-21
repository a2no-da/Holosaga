using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Blood : MonoBehaviour
{
    public event Action OnAnimationEnd;
    private SkeletonAnimation skeletonAnimation;
    private float damage;
    private Collider2D bloodCollider;
    public GameObject healPrefab;
    private bool effectApplied = false;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        bloodCollider = GetComponent<Collider2D>();
    }

    public void Initialize(float damage)
    {
        this.damage = damage;
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

        AfterMoveCompleted();
    }

    private void AfterMoveCompleted()
    {
        if (bloodCollider != null)
        {
            bloodCollider.isTrigger = true;
            StartCoroutine(DestroyAfterSeconds(3f));
        }
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
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!effectApplied && bloodCollider.isTrigger)
        {
            if (other.CompareTag("Tower") || other.CompareTag("Enemy"))
            {
                Component targetComponent = other.CompareTag("Tower") ? (Component)other.GetComponentInParent<Tower>() : other.GetComponent<Enemy>();

                if (targetComponent != null)
                {
                    if (targetComponent is Tower tower)
                    {
                        tower.Heal(damage, tower); 
                    }
                    else if (targetComponent is Enemy enemy)
                    {
                        enemy.Heal(damage, null);
                    }
                    Vector3 targetPosition = other.transform.position;
                    GameObject instance = Instantiate(healPrefab, Vector3.zero, Quaternion.identity, other.transform);
                    instance.transform.localPosition = Vector3.zero; 
                    effectApplied = true;
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}

