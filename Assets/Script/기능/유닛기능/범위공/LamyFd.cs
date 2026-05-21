using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class LamyFd : MonoBehaviour
{
    public Lamy lamy;
    public float damage;
    public float Bdamage;
    public float Sdamage;
    public float BSdamage;
    public bool isDrink;
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
    private float drinkDuration = 10f; 
    public float drinkTimer = 0f;
    public GameObject wan;
    public GameObject too;
    private MeshRenderer meshRenderer;
    public AudioSource A1;
    public AudioSource A2;
    public AudioSource A3;

    public void Initialize(float Power, float Power1, float Power2, float Power3, Lamy lamy)
    {
        this.damage = Power;
        this.Bdamage = Power2;
        this.Sdamage = Power3;
        this.BSdamage = Power3;
        this.lamy = lamy;
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent; 
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        meshRenderer = GetComponent<MeshRenderer>();
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (isDrink)
        {
            drinkTimer += Time.deltaTime; 

            if (drinkTimer >= drinkDuration)
            {
                isDrink = false;
                skeletonAnimation.AnimationState.SetAnimation(0, "pattern1_field", true);
                drinkTimer = 0f; 
            }
        }

        if (lamy.Power != damage)
        {
            damage = lamy.Power * lamy.myP[1].dmgCoe;
            Bdamage = lamy.Power * lamy.myP[2].dmgCoe;
            Sdamage = lamy.Power * lamy.myP[3].dmgCoe;
            BSdamage = lamy.Power * lamy.myP[4].dmgCoe;
        }

        meshRenderer.sortingOrder = -10;
    }

    private void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "p1Field_damage":
                DG(damage);
                break;
            case "p1Boom_damage":
                DG(Bdamage);
                A2.Play();
                break;
            case "p2Field_damage":
                DDG(Sdamage);
                break;
            case "p2Boom_damage":
                DDG(BSdamage);
                A3.Play();
                break;
            case "rami_p2":
                A1.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (isDrink)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "pattern2_field", true);
        }
        else
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "pattern1_field", true);
        }
    }

    public void boom()
    {
        GameObject bobob = isDrink ? too : wan; 
        GameObject ttt = Instantiate(bobob, transform.position, Quaternion.identity);
        Lboom lboom = ttt.GetComponent<Lboom>();

        if (lboom != null)
        {
            lboom.Initialize(Bdamage, BSdamage, lamy);
        }
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
