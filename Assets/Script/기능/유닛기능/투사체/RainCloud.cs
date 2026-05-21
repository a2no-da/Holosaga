using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RainCloud : MonoBehaviour
{
    public float damage;
    public float damage2;
    public float stunT;
    public int cot;
    public SkeletonAnimation back;
    public SkeletonAnimation rain;
    public SkeletonAnimation cloud;
    public SkeletonAnimation count;
    public SkeletonAnimation timer;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemies = new HashSet<GameObject>();
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    public Kobo kobo;
    private bool AE = false;
    private bool EE = false;
    public AudioSource A1;

    public void Initialize(float damage, float damage2, float stunT, Kobo kobo)
    {
        this.damage = damage;
        this.damage2 = damage2;
        this.stunT = stunT;
        this.kobo = kobo;
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        timer.AnimationState.Complete += HandleAnimationComplete;
        cloud.AnimationState.Complete += HandleAnimationCompletee;
        cloud.AnimationState.Event += HandleAnimationEvent;
        rain.AnimationState.Event += HandleAnimationEventt;
        cot = 3;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !AE) 
        {
            AE = true;
            OnMouseClick();
        }
        MoveWithMouse();
    }

    private void MoveWithMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; 

        transform.position = mouseWorldPosition;
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        kobo.stop();
        kobo.cloud = null;
        Destroy(gameObject);
    }

    private void HandleAnimationCompletee(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "front_thunder")
        {
            AE = false;
            if(EE)
            {
                kobo.stop();
                Destroy(gameObject);
            }
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage&stun_thunder":
                DamageEnemiess.Clear();
                Damage2();
                A1.Play();
                break;
        }
    }

    private void OnMouseClick()
    {
        if (!EE)
        {
            if (cot > 0)
            {
                cot--;
                if (cot <= 0)
                {
                    cloud.AnimationState.SetAnimation(0, "front_thunder", false);
                    count.gameObject.SetActive(false);
                    EE = true;
                }
                else
                {
                    cloud.AnimationState.SetAnimation(0, "front_thunder", false);
                    count.AnimationState.SetAnimation(0, "thunder" + cot, true);
                }
            }
        }
    }


    void Damage()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        Collider2D[] hitResults = Physics2D.OverlapCircleAll(raycastStart, raycastDistance, layerMask);

        foreach (Collider2D singleHit in hitResults)
        {
            if (singleHit != null && singleHit.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemies.Contains(singleHit.gameObject))
                {
                    enemyScript.UpdateAttackingTower(kobo);
                    enemyScript.TakeDamage(damage);
                    DamageEnemies.Add(singleHit.gameObject);
                }
            }
        }
    }

    void Damage2()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        Collider2D[] hitResults = Physics2D.OverlapCircleAll(raycastStart, raycastDistance, layerMask);

        foreach (Collider2D singleHit in hitResults)
        {
            if (singleHit != null && singleHit.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiess.Contains(singleHit.gameObject))
                {
                    enemyScript.UpdateAttackingTower(kobo);
                    enemyScript.Stun(stunT);
                    enemyScript.TakeDamage(damage2);
                    DamageEnemiess.Add(singleHit.gameObject);
                }
            }
        }
    }


    void HandleAnimationEventt(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_rain":
                DamageEnemies.Clear();
                Damage();
                break;
        }
    }
}
