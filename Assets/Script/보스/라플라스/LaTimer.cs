using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using TMPro;
using UnityEngine.UI;

public class LaTimer : Enemy
{
    private LayerMask layerMask;
    private float da;

    public AudioSource spn;
    public AudioSource tic;
    public AudioSource dam;

    public override void Start()
    {
        base.Start();
        layerMask = LayerMask.GetMask("Tower");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        Tower parentTower = transform.parent.GetComponent<Tower>();
        if (parentTower != null)
        {
            parentTower.Ptimer1 = this.gameObject;
        }
    }

    public void Initialize(float damage)
    {
        this.da = damage;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                AttackEnemiesInRange();
                break;
            case "laplus_VFX_pattern1_summon":
                spn.Play();
                break;
            case "laplus_VFX_pattern1_tick":
                tic.Play();
                break;
            case "laplus_VFX_pattern1_damage":
                dam.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    public override void Update()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, -transform.right, raycastDistance, layerMask);
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

        if (hit.collider != null && hit.collider.CompareTag("Tower"))
        {
            targetEnemy = hit.collider.transform.parent.gameObject;
        }
    }

    void AttackEnemiesInRange()
    {
        if (targetEnemy != null)
        {
            Tower tower = targetEnemy.GetComponent<Tower>();

            if(tower.isDead == false)
            {
                targetEnemy.GetComponent<Tower>().TakeDamage(da);
            }

            if (tower.Health <= 0)
            {
                ising = false;
                targetEnemy = null;
            }
        }
    }
}
