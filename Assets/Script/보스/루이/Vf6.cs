using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Vf6 : MonoBehaviour
{
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public bool Oi;
    private SkeletonAnimation skeletonAnimation;
    private float timer = 0f; 
    private float delay = 0.2f;
    private float damage;
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
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        layerMask = 1 << LayerMask.NameToLayer("Tower");
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage1":
                Oi = true;
                meshRenderer.sortingOrder = 5;
                break;
        }
    }

    void Update()
    {
        if (Oi)
        {
            CheckRaycast();
            timer += Time.deltaTime; 

            if (timer >= delay)
            {
                Oi = false; 
                timer = 0; 
            }
        }
    }

    private void CheckRaycast()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

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

    public void PlayAnimation(string animationName)
    {
        if (skeletonAnimation == null) return;
        Oi = true;
        skeletonAnimation.state.SetAnimation(0, animationName, false);
    }
}
