using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Grenade : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public int hit_Limit;
    public Tower tower;
    public GameObject vf3;
    public AudioSource a1;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private List<Enemy> hitEnemies = new List<Enemy>();
    public SkeletonAnimation skeletonAnimation;

    public void Initialize(float damage, float speed, int hit_Limit, Tower tower)
    {
        this.damage = damage;
        this.speed = speed;
        this.hit_Limit = hit_Limit;
        this.tower = tower;
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(5f));
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "towa_VFX_pattern3_clink":
                a1.Play();
                break;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

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
                    enemyScript.TakeDamage(damage);

                    Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
                    GameObject vf3GO = Instantiate(vf3, spawnPosition, Quaternion.identity);
                    BlackHole blackHole = vf3GO.GetComponent<BlackHole>();

                    if (blackHole != null)
                    {
                        blackHole.Initialize(damage, tower);
                    }

                    hit_Limit--;
                }
            }
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
