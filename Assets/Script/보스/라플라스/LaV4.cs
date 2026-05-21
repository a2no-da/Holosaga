using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class LaV4 : MonoBehaviour
{
    public int mynum;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private SkeletonAnimation skeletonAnimation;
    private float maxDamage;
    public Tower targetTower;
    public float xspeed = 0f;
    public float yspeed = 0f;
    private bool eed = false;

    public AudioSource spn;
    public AudioSource idle;
    public AudioSource dam;

    public void Initialize(float damage, Tower targetTower)
    {
        this.maxDamage = damage;
        this.targetTower = targetTower;
    }

    void Update()
    {
        if (!eed)
        {
            if (CheckRaycastTower())
            {
                eed = true;
                skeletonAnimation.AnimationState.SetAnimation(0, "damage", false);
                CheckRaycast();
            }
            else
            {
                Acceleration(targetTower);

                transform.position += new Vector3(xspeed, 0, 0) * Time.deltaTime;
                transform.position += new Vector3(0, yspeed, 0) * Time.deltaTime;

            }
        }
    }

    void Acceleration(Tower targetTower)
    {
        //x
        if (targetTower.transform.position.x > this.transform.position.x)
        {
            xspeed += 0.02f;
        }
        else
        {
            xspeed -= 0.02f;
        }

        //y
        if (targetTower.transform.position.y > this.transform.position.y)
        {
            yspeed += 0.01f;
        }
        else
        {
            yspeed -= 0.01f;
        }
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        layerMask = 1 << LayerMask.NameToLayer("Tower");
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "spawn")
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", false);
        }

        if (trackEntry.Animation.Name == "idle" || trackEntry.Animation.Name == "damage")
        {
            Destroy(gameObject);
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                CheckRaycast();
                break;
            case "laplus_VFX_pattern4_summon":
                spn.Play();
                break;
            case "laplus_VFX_pattern4_idle":
                idle.Play();
                break;
            case "laplus_VFX_pattern4_damage":
                dam.Play();
                break;
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
            Tower tower = parentObject.GetComponent<Tower>();

            if (tower != null)
            {
                float randomDamage = Random.Range(0.01f, 1.0f) * maxDamage;
                tower.TakeDamage(randomDamage);
            }
        }
    }

    private bool CheckRaycastTower()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.blue);
        Debug.DrawLine(topRight, bottomRight, Color.blue);
        Debug.DrawLine(bottomRight, bottomLeft, Color.blue);
        Debug.DrawLine(bottomLeft, topLeft, Color.blue);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
        {
            return true;
        }

        else { return false; }
    }
}
