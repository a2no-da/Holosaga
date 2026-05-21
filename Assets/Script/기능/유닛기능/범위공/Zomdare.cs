using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Zomdare : Tower
{
    public Tower tower;
    public float speed;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    public GameObject up;
    private bool BOOM = false;
    private float timer = 0f;
    private bool animationTriggered = false;
    public AudioSource a1;
    public AudioSource a2;

    public override void Start()
    {
        isEx = true;
        base.Start();

        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (tower != null)
        {
            MaxHealth = (tower.MaxHealth * 0.3f);
            Health = MaxHealth;
        }
        else
        {
            MaxHealth = 50;
            Health = MaxHealth;
        }

        Health = MaxHealth;
        SetAnimation(0, AnimClip[0], false);

        H_regen = 0;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.sortingOrder = 20;
        UpdateHealthBarSortingOrder(7);
        SP();
        HealthBarInstance.SetActive(false);
    }

    public override void Attack()
    {
    }

    public override void Update()
    {
        base.Update();

        if (!BOOM)
        {
            if (!animationTriggered)
            {
                timer += Time.deltaTime;
                if (timer >= 7f)
                {
                    SetAnimation(0, AnimClip[4], false);
                    BOOM = true;
                    animationTriggered = true;
                }
            }
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                Damage();
                a1.Play();
                break;
            case "olie_VFX_pattern2_start":
                a2.Play();
                break;
        }
    }

    public void SP()
    {
        float localY = transform.localPosition.y;

        if (Mathf.Approximately(localY, 0.8600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -2;
        }
        else if (Mathf.Approximately(localY, -1.16f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 0;
        }
        else if (Mathf.Approximately(localY, -3.18f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 2;
        }
        else if (Mathf.Approximately(localY, -5.2f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 4;
        }
    }

    void Damage()
    {
        DamageEnemiess.Clear();
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
                    enemyScript.TakeDamage(tower.Power * tower.myP[2].dmgCoe);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    protected override void Die()
    {
    }

    public override void TakeDamage(float damage)
    {
        if (!BOOM)
        {
            base.TakeDamage(damage);

            float healthPercentage = Health / MaxHealth;

            if (healthPercentage < 0.5f)
            {
                SetAnimation(0, AnimClip[2], true);
                Instantiate(up, transform.position, transform.rotation);
            }

            if (Health <= 0)
            {
                SetAnimation(0, AnimClip[3], false);
                Instantiate(up, transform.position, transform.rotation);
                BOOM = true;
            }
        }
    }

    public new void toCollider()
    {
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[0].name)
        {
            SetAnimation(0, AnimClip[1], true);
        }

        if (trackEntry.Animation.Name == AnimClip[3].name)
        {
            SetAnimation(0, AnimClip[4], false);
        }

        if (trackEntry.Animation.Name == AnimClip[4].name)
        {
            Destroy(HealthBarInstance);

            HealthBarImage = null;
            HealthBarInstance = null;
            Destroy(gameObject);
        }
    }

    public override void Stun(float stunDuration)
    {
    }
}
