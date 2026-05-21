using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Pyoyo : Enemy
{
    public GameObject soulBulletPrefab;
    public LayerMask towerLayer;
    public GameObject SPoint;
    public AudioClip charge;
    public AudioClip fire;
    public AudioSource Source;

    public override void Start()
    {
        base.Start();
        towerLayer = LayerMask.GetMask("Tower");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "poyoyo_charge":
                SoundCh();
                break;
            case "poyoyo_fire":
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
                SetAnimation(0, AnimClip[2], true);
            }
        }
        else if (targetEnemy != null || kabedong)
        {
            moveSpeed = 0;
            if (!ising)
            {
                SetAnimation(0, AnimClip[0], true);
            }
        }

        bool shouldIgnoreRange1 = mC[1].IgnRange == 0;

        if (hit.collider != null)
        {
            if (!ising && attackCooldown <= 0 && (hit.collider.CompareTag("Tower") || !shouldIgnoreRange1) && !isStunned)
            {
                ising = true;
                attackCooldown = Pattern_cooltime;
                SetAnimation(0, AnimClip[1], false);
            }
        }

        if (isStunned)
        {
            SetAnimation(0, AnimClip[0], true);
        }

        MoveTowardsEndPoint();
    }

    public override void Attack()
    {
        SetAnimation(0, AnimClip[1], true);
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

    private void AttackEnemiesInRange()
    {
        Source.PlayOneShot(fire);
        GameObject soulBullet = Instantiate(soulBulletPrefab, SPoint.transform.position, Quaternion.identity);
        SoulTama soulTama = soulBullet.GetComponent<SoulTama>();
        Vector2 direction = Vector2.left;
        if (soulTama != null)
        {
            soulTama.Initialize(Power * mP[1].dmgCoe, mP[1].speed, mP[1].hitLim);
        }
    }

    private void SoundCh()
    {
        Source.PlayOneShot(charge);
    }
}
