using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class WaterDrop : MonoBehaviour
{
    public float speed = 10f;
    public float speed2 = 10f;
    public float damage;
    public int hit_Limit;

    private Enemy target;
    public Tower tower;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private List<Enemy> hitEnemies = new List<Enemy>();
    public SkeletonAnimation skeletonAnimation;
    private bool isOutsideCamera = false;
    private bool isOutsi = false;
    private bool istw = false;
    private bool isre = false;
    private bool isEd = false;
    public AudioSource a1;

    public void Initialize(float damage, float speed, float speed2, int hit, Tower tower)
    {
        this.damage = damage;
        this.speed = speed;
        this.speed2 = speed2;
        this.hit_Limit = hit;
        this.tower = tower;
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        if(tower.LevelS > 1)
        {
            istw = true;
        }
        else
        {
            StartCoroutine(DestroyAfterSeconds(10f));
        }
    }

    void Update()
    {
        if (!isEd)
        {
            if (istw)
            {
                if (!isOutsideCamera)
                {
                    transform.Translate(Vector3.right * speed * Time.deltaTime);
                    CheckIfOutsideCamera();
                }
                else
                {
                    if (!isre)
                    {
                        hitEnemies.Clear();
                        isre = true;
                    }
                    MoveTowardsTower();
                }
            }
            else
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }

        if (hit_Limit <= 0)
        {
            Destroy(gameObject);
        }

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

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemyScript != null && !hitEnemies.Contains(enemyScript))
            {
                if (hit_Limit > 0)
                {
                    hitEnemies.Add(enemyScript);
                    enemyScript.UpdateAttackingTower(tower);
                    if(isOutsideCamera)
                    {
                        enemyScript.TakeDamage(damage * 2f);
                        enemyScript.SlowEffect(tower.myP[2].slowPower, tower.myP[2].slowTime);//(slow, slowT);
                    }
                    else
                    {
                        enemyScript.TakeDamage(damage);
                    }
                    hit_Limit--;
                }
            }
        }
    }

    private void CheckIfOutsideCamera()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            if (!isOutsi) 
            {
                isOutsi = true;
                StartCoroutine(HandleOutsideCamera());
            }
        }
    }

    private IEnumerator HandleOutsideCamera()
    {
        yield return new WaitForSeconds(0.2f);

        isOutsideCamera = true;
        skeletonAnimation.AnimationState.SetAnimation(0, "2", true);
        speed = speed2;
        raycastStartOffset.x = 0.5f;
        boxSize.x = 1.2f;
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    private void MoveTowardsTower()
    {
        Vector3 towerPosition = tower.transform.position;
        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, towerPosition, step);

        if (Vector3.Distance(transform.position, towerPosition) < 0.1f)
        {
            isEd = true;
            skeletonAnimation.AnimationState.ClearTracks();
            transform.SetParent(tower.transform);
            transform.localPosition = Vector3.zero;
            skeletonAnimation.AnimationState.SetAnimation(0, "pattern2_end", false); 
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "kobo_VFX_pattern2":
                a1.Play();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "pattern2_end")
        {
            Destroy(gameObject); 
        }
    }
}
