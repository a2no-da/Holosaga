using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robosa : Enemy
{
    public float telCooldown;
    private float tel_cooltime = 2f;
    public AudioSource A1;
    public AudioSource A2;

    public override void Start()
    {
        base.Start();

        //tel_cooltime = mC[2].cool;
        //telCooldown = mC[2].initCool;

        tel_cooltime = 7;
        telCooldown = 5;

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
            case "teleport1":
                A1.Play();
                break;
            case "teleport2":
                A2.Play();
                Telpo();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[1].name || trackEntry.Animation.Name == AnimClip[3].name)
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

        if (telCooldown > 0 && !isStunned)
        {
            telCooldown -= Time.deltaTime;
        }

        if (!ising && telCooldown <= 0)
        {
            ising = true;
            telCooldown = tel_cooltime;
            SetAnimation(0, AnimClip[3], false);
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

    void Telpo()
    {
        Vector3 newPosition = transform.position - new Vector3(3.3f, 0, 0);
        transform.position = newPosition;
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
