using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class LunaP22 : MonoBehaviour
{
    public Luna luna;
    private SkeletonAnimation skeletonAnimation;

    private int one;
    private float two;
    private float three;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private Tile tile;
    private bool hime;

    public AudioSource A1;
    public AudioSource A2;

    public void Initialize(float Power, float Power2, float time, Luna luna)
    {
        this.one = (int)Power;
        this.two = (int)Power2;
        this.three = time;
        this.luna = luna;
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        layerMask = 1 << LayerMask.NameToLayer("Enemy");

        hime = false;
        SP();
    }

    public void SP()
    {
        float localY = transform.position.y;

        if (Mathf.Approximately(localY, 0.6600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(localY, -1.36f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(localY, -3.38f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(localY, -5.4f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                P2(one);
                A1.Play();
                break;
            case "damage_stun":
                raycastStartOffset = new Vector2(0f, 1f);
                boxSize = new Vector2(4.95f, 6.06f);
                hime = true;
                P2(two);
                A2.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    public void P2(float damage)
    {
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

                if (enemy != null)
                {
                    enemy.UpdateAttackingTower(luna);
                    enemy.TakeDamage(damage);
                    if (hime)
                    {
                        enemy.Stun(three);
                    }
                }
            }
        }
    }
}
