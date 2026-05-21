using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class BlackHole : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public float damage;
    public Tower tower;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    public float yOffset;
    protected List<GameObject> enemiesInRange = new List<GameObject>();
    private Vector3 endPosition = Vector3.zero;
    public AudioSource a1;
    public AudioSource a2;
    private float pullDuration = 4f;
    private float pullElapsedTime = 0f; 
    private bool isPullingEnemies = false;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        StartPullingEnemies();
    }

    public void Initialize(float damage, Tower tower)
    {
        this.damage = damage;
        this.tower = tower;
    }

    void Update()
    {
        if (isPullingEnemies)
        {
            PullEnemies();
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_and_pull":
                DamageEnemiess.Clear();
                Damage();
                a1.Play();
                break;
            case "towa_VFX_pattern3_start":
                a2.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void Damage()
    {
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
                    enemyScript.TakeDamage(damage);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    public void StartPullingEnemies()
    {
        if (!isPullingEnemies)
        {
            isPullingEnemies = true;
            pullElapsedTime = 0f; 
            FindEnemiesInRange(); 
            float endPositionY = GetEndPositionY(transform.position.y);
            endPosition = new Vector3(transform.position.x, endPositionY, 0);
        }
    }

    private void FindEnemiesInRange()
    {
        Vector3 center = transform.position + new Vector3(0, yOffset, 0);
        Collider2D[] allEnemies = Physics2D.OverlapBoxAll(center, boxSize, 0f);

        enemiesInRange.Clear();
        foreach (Collider2D enemy in allEnemies)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (!enemyScript.Laplus)
                {
                    enemiesInRange.Add(enemy.gameObject);
                }
            }
        }
    }

    private void PullEnemies()
    {
        pullElapsedTime += Time.deltaTime;

        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = enemiesInRange[i];
            if (enemyObject == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DisableMovement();
                float t = pullElapsedTime / pullDuration;
                enemy.transform.position = Vector3.Lerp(enemy.transform.position, endPosition, t);

                if (t >= 1f)
                {
                    enemy.EnableMovement();
                }
            }
        }

        if (pullElapsedTime >= pullDuration)
        {
            isPullingEnemies = false;
            EndPosition(endPosition);
        }
    }

    private void EndPosition(Vector3 endPosition)
    {
        foreach (GameObject enemyObject in enemiesInRange)
        {
            if (enemyObject != null)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (!enemy.Laplus)
                {
                    if (enemy != null)
                    {
                        enemy.transform.position = endPosition;
                    }
                }
            }
        }
    }

    private float GetEndPositionY(float currentY)
    {
        if (Mathf.Approximately(currentY, 0.6100001f))
        {
            return 0.660003f;
        }
        else if (Mathf.Approximately(currentY, -1.41f))
        {
            return -1.36f;
        }
        else if (Mathf.Approximately(currentY, -3.43f))
        {
            return -3.38f;
        }
        else if (Mathf.Approximately(currentY, -5.45f))
        {
            return -5.4f;
        }
        return currentY; 
    }
}
