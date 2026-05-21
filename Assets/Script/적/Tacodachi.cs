using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Tacodachi : Enemy
{
    public Laser laser;
    public LayerMask towerLayer;
    private float laserActiveTime;
    public AudioClip charge;
    public AudioClip fire;
    public AudioSource Source;

    public override void Start()
    {
        base.Start();
        laser.gameObject.SetActive(false);
        towerLayer = LayerMask.GetMask("Tower");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "tacodachi_charge":
                SoundCh();
                break;
            case "tacodachi_fire":
                AttackEnemiesInRange();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[1].name)
        {
            attackCooldown = Pattern_cooltime;
            SetAnimation(0, AnimClip[0], true);
            ising = false;
        }
    }

    public override void Update()
    {
        base.Update();

        bool shouldIgnoreRange1 = mC[1].IgnRange == 0;

        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, -transform.right, raycastDistance, towerLayer);
        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        Vector2 raycastEnd = raycastStart - new Vector2(transform.right.x, transform.right.y) * raycastDistance;
        Debug.DrawLine(raycastStart, raycastEnd, Color.blue);

        if (targetEnemy == null && !kabedong)
        {
            if (!ising && !isStunned)
            {
                moveSpeed = initialMoveSpeed;
            }
        }
        else if (targetEnemy != null || kabedong)
        {
            moveSpeed = 0;
        }

        if (hit.collider != null)
        {
            if (!ising && attackCooldown <= 0 && (hit.collider.CompareTag("Tower") || !shouldIgnoreRange1))
            {
                ising = true;
                attackCooldown = Pattern_cooltime;
                SetAnimation(0, AnimClip[1], false);
            }
        }

        if (Time.time >= laserActiveTime + 0.2f)  
        {
            laser.gameObject.SetActive(false);
        }

        MoveTowardsEndPoint();
    }

    public override void Attack()
    {
        base.Attack();
        PerformAttack();
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

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private void PerformAttack()
    {
        SetAnimation(0, AnimClip[1], false);
    }

    void AttackEnemiesInRange()
    {
        Source.PlayOneShot(fire);
        laser.SetDamage(Power * mP[1].dmgCoe);
        laser.gameObject.SetActive(true);
        laserActiveTime = Time.time;  
    }

    private void SoundCh()
    {
        Source.PlayOneShot(charge);
    }
}