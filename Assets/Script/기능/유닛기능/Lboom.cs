using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Lboom : MonoBehaviour
{
    public Lamy lamy;
    public float Bdamage;
    public float BSdamage;

    public SkeletonAnimation skeletonAnimation;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);

    public Vector2 raycastDirection2 = Vector2.right;
    public Vector2 raycastStartOffset2 = Vector2.zero;
    public Vector2 boxSize2 = new Vector2(1f, 1f);

    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    public void Initialize(float Power, float Power1, Lamy lamy)
    {
        this.Bdamage = Power;
        this.BSdamage = Power1;
        this.lamy = lamy;
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "p1Boom_damage":
                DG(Bdamage);
                break;
            case "p2Boom_damage":
                DDG(BSdamage);
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }


    public void DG(float dama)
    {
        DamageEnemiess.Clear();
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (enemy != null && !DamageEnemiess.Contains(hit.collider.gameObject))
                {
                    enemy.UpdateAttackingTower(lamy);
                    enemy.TakeDamage(dama);
                    DamageEnemiess.Add(hit.collider.gameObject);
                }
            }
        }
    }

    public void DDG(float dama)
    {
        DamageEnemiess.Clear();
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset2.x, transform.position.y + raycastStartOffset2.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize2, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize2.x / 2, boxSize2.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize2.x / 2, boxSize2.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize2.x / 2, -boxSize2.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize2.x / 2, -boxSize2.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.blue);
        Debug.DrawLine(topRight, bottomRight, Color.blue);
        Debug.DrawLine(bottomRight, bottomLeft, Color.blue);
        Debug.DrawLine(bottomLeft, topLeft, Color.blue);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (enemy != null && !DamageEnemiess.Contains(hit.collider.gameObject))
                {
                    enemy.UpdateAttackingTower(lamy);
                    enemy.TakeDamage(dama);
                    DamageEnemiess.Add(hit.collider.gameObject);
                }
            }
        }
    }
}
