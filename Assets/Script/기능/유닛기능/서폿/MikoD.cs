using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class MikoD : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public float Fdamage;
    public float Kdamage;
    public float Kstime;
    public Miko miko;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    public AudioSource Ma;
    public AudioSource Ze;
    public AudioSource Li;

    public void Initialize(float damage, float damage2, float stunT, Miko tower)
    {
        this.miko = tower;
        this.Kdamage = damage;
        this.Fdamage = damage2;
        this.Kstime = stunT;
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        string[] animationNames = { "bad_magma", "bad_thunder", "bad_zenloss" };
        string selectedAnimation = animationNames[Random.Range(0, animationNames.Length)];

        skeletonAnimation.AnimationState.SetAnimation(0, selectedAnimation, false);
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_fire":
                Fire();
                break;
            case "damage_zenloss":
                Zen();
                break;
            case "miko_VFX_pattern2_magma":
                Ma.Play();
                break;
            case "miko_VFX_pattern2_zenLoss":
                Ze.Play();
                break;
            case "miko_VFX_pattern2_lightning":
                Li.Play();
                Kaminari();
                break;
        }
    }

    void Fire()
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
                    enemyScript.Fire();
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    void Zen()
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

        List<Enemy> enemiesInRange = new List<Enemy>(); 

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    enemiesInRange.Add(enemyScript); 
                }
            }
        }

        if (enemiesInRange.Count > 0)
        {
            int numberOfEnemiesToSelect = Mathf.Min(3, enemiesInRange.Count); 
            List<Enemy> selectedEnemies = new List<Enemy>();

            while (selectedEnemies.Count < numberOfEnemiesToSelect)
            {
                int randomIndex = Random.Range(0, enemiesInRange.Count);
                Enemy selectedEnemy = enemiesInRange[randomIndex];

                if (!selectedEnemies.Contains(selectedEnemy))
                {
                    selectedEnemies.Add(selectedEnemy);
                    selectedEnemy.ZenLoss(); 
                }
            }
        }
    }

    void Kaminari()
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
                    enemyScript.UpdateAttackingTower(miko);
                    enemyScript.TakeDamage(Kdamage); 
                    enemyScript.Stun(Kstime);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (miko.LevelS > 2)
        {
            Vector3 toriiPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            Vector3 roadPosition = new Vector3(0, transform.position.y - 0.2f, transform.position.z);
            GameObject torii = Instantiate(miko.toriiPrefab, transform.position, Quaternion.identity);
            GameObject road = Instantiate(miko.roadPrefab, roadPosition, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
