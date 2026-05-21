using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Tsunmire : Enemy
{
    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        SkeletonAnimation Ye = this.GetComponentInChildren<SkeletonAnimation>();
        if (Ye != null)
        {
            string[] skins = { "1", "2" };
            string randomSkin = skins[Random.Range(0, skins.Length)];
            Ye.skeleton.SetSkin(randomSkin);
            Ye.skeleton.SetSlotsToSetupPose();
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                AttackEnemiesInRange(Power);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        if (targetEnemy == null && !kabedong)
        {
            MoveTowardsEndPoint();
        }

        if (!ising && attackCooldown <= 0 && targetEnemy != null && !isStunned)
        {
            ising = true;
            attackCooldown = Pattern_cooltime;
            Attack();
        }

        if (isStunned)
        {
            SetAnimation(0, AnimClip[0], true);
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

    void AttackEnemiesInRange(float damage = 0)
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

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[1].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }
}