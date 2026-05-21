using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class DoogPunch : MonoBehaviour
{
    private float damage;
    private SkeletonAnimation skeletonAnimation;
    private float animationSpeed;
    public Tower tower;

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.timeScale = animationSpeed;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    public void Initialize(float Power, float animationSpeed, Tower tower)
    {
        this.damage = Power;
        this.animationSpeed = animationSpeed;
        this.tower = tower;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.UpdateAttackingTower(tower);
            enemy.TakeDamage(damage);
        }
    }
}
