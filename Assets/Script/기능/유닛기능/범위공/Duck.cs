using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    public float speed;
    public float damage;
    public int hit_Limit;
    public float Pull;

    private Enemy target;
    public Tower tower;
    private float maxX;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private List<KnockbackInfo> knockbackInfos = new List<KnockbackInfo>();
    HashSet<GameObject> knockedBackEnemies = new HashSet<GameObject>();
    public Force forceComponent;

    public void Initialize(float damage, float speed, int hit, float full, Tower tower)
    {
        this.damage = damage;
        this.speed = speed;
        this.hit_Limit = hit;
        this.Pull = full;
        this.tower = tower;
    }

    public class KnockbackInfo
    {
        public GameObject enemy;
        public float knockbackEndTime;

        public KnockbackInfo(GameObject enemy, float duration)
        {
            this.enemy = enemy;
            this.knockbackEndTime = Time.time + duration;
        }
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(3f));
        maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (forceComponent == null)
        {
            forceComponent = gameObject.AddComponent<Force>();
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > maxX || hit_Limit <= 0)
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
            Rigidbody2D enemyRb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
            if (enemyScript != null && !knockedBackEnemies.Contains(hit.collider.gameObject))
            {
                if (hit_Limit > 0)
                {
                    enemyScript.UpdateAttackingTower(tower);
                    enemyScript.TakeDamage(damage);
                    hit_Limit--;

                    if (forceComponent != null)
                    {
                        forceComponent.PublicApplyForce(enemyRb, new Vector2(Pull, 0));
                    }
                    knockedBackEnemies.Add(hit.collider.gameObject);
                }
            }
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        foreach (GameObject enemyObj in knockedBackEnemies)
        {
            if (enemyObj != null) 
            {
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                Rigidbody2D enemyRb = enemyObj.GetComponent<Rigidbody2D>();
                if (enemyRb != null && enemyScript != null)
                {
                    enemyRb.velocity = Vector2.zero; 
                    enemyScript.isPushedOrPulled = false; 
                }
            }
        }
    }
}
