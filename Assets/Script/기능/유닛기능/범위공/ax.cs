using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class ax : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Tower tower;
    public float damage;
    public float speed;
    public float hit_Limit;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private Vector2 currentDirection;
    public AudioSource a1;

    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();

    public void Initialize(float damage, float speed, int hit_Limit, Tower tower)
    {
        this.damage = damage;
        this.speed = speed;
        this.hit_Limit = hit_Limit;
        this.tower = tower;
        currentDirection = Vector3.right;
        SetSkinFromTower();
    }

    void Start()
    {
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        Destroy(gameObject, 7f);
    }

    void Update()
    {
        Dama();
    }

    public void Dama()
    {
        transform.Translate(currentDirection * speed * Time.deltaTime);

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

        CheckBoundary();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "suisei_VFX_pattern2":
                a1.Play();
                break;
        }
    }


    private void CheckBoundary()
    {
        Camera camera = Camera.main;
        Vector3 screenPosition = camera.WorldToViewportPoint(transform.position);

        if (screenPosition.x < 0 || screenPosition.x > 1 || screenPosition.y < 0 || screenPosition.y > 1)
        {
            Vector2 hitPoint = transform.position; 
            currentDirection = GetRandomDirectionTowardsCamera(hitPoint);
        }
    }

    private Vector2 GetRandomDirectionTowardsCamera(Vector2 hitPoint)
    {
        Vector2 cameraPosition = Camera.main.transform.position;

        Vector2 directionToCamera = (cameraPosition - hitPoint).normalized;

        float randomAngle = Random.Range(-90f, 90f);

        Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * directionToCamera;

        DamageEnemiess.Clear();
        return randomDirection.normalized; 
    }

    private void SetSkinFromTower()
    {
        if (tower != null && tower.skeletonAnimation != null)
        {
            string skinName = tower.skeletonAnimation.initialSkinName;

            if (string.IsNullOrEmpty(skinName)) return;
            var skeletonData = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true);
            var skin = skeletonData.FindSkin(skinName);
            if (skin == null) return; 

            skeletonAnimation.initialSkinName = skinName;
            skeletonAnimation.Skeleton.SetSkin(skin);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
        }
    }
}
