using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class LaV62 : MonoBehaviour
{
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private bool Oi;
    private SkeletonAnimation skeletonAnimation;
    private float timer = 0f;
    private float delay = 0.2f;
    private float damage;

    public AudioSource spn;
    public AudioSource dam;

    public Vector2[] additionalBoxOffsets = new Vector2[]
{
    new Vector2(2, 0),
    new Vector2(-2, 0),
    new Vector2(0, 2),
    new Vector2(0, -2)
};

    public Vector2[] additionalBoxSizes = new Vector2[]
    {
    new Vector2(1, 1),
    new Vector2(1, 1),
    new Vector2(1, 1),
    new Vector2(1, 1)
    };

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
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern6":
                hitTargets.Clear();
                Oi = true;
                break;
            case "laplus_VFX_pattern6_summon":
                spn.Play();
                break;
            case "laplus_VFX_pattern6_damage":
                dam.Play();
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
        PerformBoxCast(raycastStart, boxSize);

        for (int i = 0; i < additionalBoxOffsets.Length; i++)
        {
            Vector2 additionalBoxStart = raycastStart + additionalBoxOffsets[i];
            Vector2 additionalBoxSize = additionalBoxSizes[i];

            PerformBoxCast(additionalBoxStart, additionalBoxSize);
        }
    }

    private void PerformBoxCast(Vector2 start, Vector2 size)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(start, size, 0f, raycastDirection, 0, layerMask);

        Vector2 topLeft = start + new Vector2(-size.x / 2, size.y / 2);
        Vector2 topRight = start + new Vector2(size.x / 2, size.y / 2);
        Vector2 bottomLeft = start + new Vector2(-size.x / 2, -size.y / 2);
        Vector2 bottomRight = start + new Vector2(size.x / 2, -size.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
            {
                GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
                Tower tower = parentObject.GetComponent<Tower>();
                Tile tile = parentObject.GetComponentInParent<Tile>();
                if (hitTargets.Contains(parentObject)) return;

                if (tower != null)
                {
                    hitTargets.Add(parentObject);
                    tower.TakeDamage(damage);
                }
            }
        }
    }
}
