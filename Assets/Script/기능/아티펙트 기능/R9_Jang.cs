using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class R9_Jang : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public float damage;
    public Tower tower;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public AudioSource a1;

    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();

    public void Initialize(float damage, Unit unit)
    {
        this.damage = damage;
        if (unit is Tower tower)
        {
            this.tower = tower;
        }
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "shioko1_damage":
                Dama();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    public void Dama()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    enemyScript.UpdateAttackingTower(tower);
                    enemyScript.TakeDamage(damage);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }
}
