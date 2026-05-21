using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class OverlapBomb : MonoBehaviour
{
    public float damage;
    public float damage2;

    private Enemy target;
    public Tower tower;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private List<Enemy> hitEnemies = new List<Enemy>();
    public bool isp3;

    public SkeletonAnimation skeletonAnimation;
    public GameObject subF;
    public Vector3 Eneposition;
    private bool hasSpawned = false;

    public void Initialize(float damage, float damage2, Tower tower, bool isp3)
    {
        this.damage = damage;
        this.damage2 = damage2;
        this.tower = tower;
        this.isp3 = isp3;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                CheckRaycast();
                break;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;

        int randomAnimIndex = UnityEngine.Random.Range(1, 4);

        string selectedAnimIndex;

        if (randomAnimIndex == 1)
        {
            selectedAnimIndex = "1";
        }
        else if (randomAnimIndex == 2)
        {
            selectedAnimIndex = "2";
        }
        else
        {
            selectedAnimIndex = "3";
        }

        skeletonAnimation.AnimationState.SetAnimation(0, selectedAnimIndex, false);
    }

    void CheckRaycast()
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

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemyScript != null && !hitEnemies.Contains(enemyScript))
            {
                hitEnemies.Add(enemyScript);
                enemyScript.UpdateAttackingTower(tower);
                if(isp3)
                {
                    enemyScript.TakeDamage(damage2);
                    if (!hasSpawned)
                    {
                        SpawnSubF(enemyScript.spawnPoint.position, enemyScript);
                        hasSpawned = true;
                    }
                }
                else
                {
                    enemyScript.TakeDamage(damage);
                }
            }
        }
        else
        {
            if (Eneposition != null)
            {
                if (!hasSpawned)
                {
                    if (isp3)
                    {
                        SpawnSubF(Eneposition, null);
                        hasSpawned = true;
                    }
                }
            }
        }
    }

    private void SpawnSubF(Vector3 position, Enemy enemy)
    {
        float[] rotationZValues = { 11.25f, 5.625f, 0f, -5.625f, -11.25f };

        for (int i = 0; i < rotationZValues.Length; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, rotationZValues[i]);

            GameObject f2 = Instantiate(subF, position, rotation);
            Feather2 feather2 = f2.GetComponent<Feather2>();

            if (feather2 != null)
            {
                feather2.Initialize(tower.Power * tower.myP[5].dmgCoe, tower.myP[5].speed, tower.myP[5].hitLim, tower, enemy);
            }
        }
    }
}
