using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class AcPlate : MonoBehaviour
{
    public Tower tower;
    private LayerMask layerMask;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private float damage;
    public AudioSource po;
    public SkeletonAnimation skeletonAnimation;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    private bool canDamage = true; 
    private float disableTime;

    public void Initialize(float damage, Unit unit)
    {
        this.damage = damage;
        if (unit is Tower tower)
        {
            this.tower = tower;
        }
    }

    public void Start()
    {
        layerMask = LayerMask.GetMask("Enemy");
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        disableTime = Time.time + 0.2f;
    }

    public void Update()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, Vector2.zero, raycastDistance, layerMask);

        Vector2 raycastEnd = raycastStart - new Vector2(transform.right.x, transform.right.y) * raycastDistance;
        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        if (!canDamage)
            return;

        if (Time.time >= disableTime)
        {
            canDamage = false;
        }

        foreach (RaycastHit2D hit in hits)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null && !DamageEnemiess.Contains(hit.collider.gameObject))
            {
                enemy.UpdateAttackingTower(tower);
                enemy.TakeDamage(damage);
                DamageEnemiess.Add(hit.collider.gameObject);
            }
        }
    }

    public void SetAnimation(string animationName)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
        //po.Play();
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}