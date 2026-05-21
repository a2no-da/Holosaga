using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikoP : Enemy
{
    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                AttackEnemiesInRange();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[1].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }

    public override void Update()
    {
        base.Update();

        if (targetEnemy == null && !kabedong)
        {
            MoveTowardsEndPoint();
        }

        bool shouldIgnoreRange1 = mC[1].IgnRange == 0;

        if (!ising && attackCooldown <= 0 && (targetEnemy != null || !shouldIgnoreRange1) && !isStunned)
        {
            ising = true;
            attackCooldown = Pattern_cooltime;
            Attack();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower"))
        {
            if (collider.transform.parent != null)
            {
                targetEnemy = collider.transform.parent.gameObject;
                Tower tower = targetEnemy.GetComponent<Tower>();
            }
        }

        if (collider.gameObject.CompareTag("Kabe"))
        {
            kabedong = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower"))
        {
            if (collider.transform.parent != null && collider.transform.parent.gameObject == targetEnemy)
            {
                ising = false;
                targetEnemy = null;
                MoveTowardsEndPoint();
            }
        }

        if (collider.gameObject.CompareTag("Kabe"))
        {
            kabedong = false;
        }
    }

    private void HandleTowerMoved(Tower tower)
    {
        if (targetEnemy == tower.gameObject)
        {
            ising = false;
            targetEnemy = null;
        }
    }

    public override void Attack()
    {
        SetAnimation(0, AnimClip[1], false);
    }

    void AttackEnemiesInRange()
    {
        if (targetEnemy != null)
        {
            Tower tower = targetEnemy.GetComponent<Tower>();
            targetEnemy.GetComponent<Tower>().TakeDamage(Power * mP[1].dmgCoe);

            if (tower.Health <= 0)
            {
                ising = false;
                targetEnemy = null;
            }
        }
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            SetAnimation(0, AnimClip[2], true);
        }
    }
}

