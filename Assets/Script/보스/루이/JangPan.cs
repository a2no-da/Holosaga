using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class JangPan : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public int hit_Limit;
    public int Times;
    public Lui lui;
    private SkeletonAnimation skeletonAnimation;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private bool Oi;
    private float timer = 0f;
    private float delay = 0.2f;
    public AudioSource A1;

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Tower");
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
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

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage1":
                A1.Play();
                Oi = true;
                break;
        }
    }

    public void Initialize(float damage, float speed, int hit_Limit, int Time)
    {
        this.damage = damage;
        this.speed = speed;
        this.Times = Time;
        this.hit_Limit = hit_Limit;
    }

    private void CheckRaycast()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.green);
        Debug.DrawLine(topRight, bottomRight, Color.green);
        Debug.DrawLine(bottomRight, bottomLeft, Color.green);
        Debug.DrawLine(bottomLeft, topLeft, Color.green);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
        {
            GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
            Tower tower = parentObject.GetComponent<Tower>();

            if (tower != null && !hitTargets.Contains(parentObject))
            {
                hitTargets.Add(parentObject);
                tower.TakeDamage(damage);
                tower.Stun(Times);
            }
        }
    }
}
