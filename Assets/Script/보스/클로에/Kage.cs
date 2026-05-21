using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Kage : MonoBehaviour
{
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public SkeletonAnimation skeletonAnimation;
    private float damage;

    public AudioSource spn;
    private MeshRenderer meshRenderer;

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        layerMask = 1 << LayerMask.NameToLayer("Tower");
        meshRenderer = GetComponent<MeshRenderer>();

        if (transform.position.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            raycastStartOffset.x = 8.25f;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                spn.Play();
                CheckRaycast();
                break;
        }
    }

    private void CheckRaycast()
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

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
            {
                GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
                if (hitTargets.Contains(parentObject)) return;

                Tower tower = parentObject.GetComponent<Tower>();
                Tile tile = parentObject.GetComponentInParent<Tile>();

                if (tower != null)
                {
                    hitTargets.Add(parentObject);
                    tower.TakeDamage(damage);
                }
            }
        }
    }
}
