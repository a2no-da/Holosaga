using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcWind : MonoBehaviour
{
    public GameObject Mee;
    public float speed = 10f;
    public float damage;
    public int hit_Limit;

    private Enemy target;
    public Tower tower;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();

    public void Initialize(float damage, float speed, int hit, Unit unit)
    {
        this.damage = damage;
        this.speed = speed;
        this.hit_Limit = hit;
        if (unit is Tower tower)
        {
            this.tower = tower;
        }
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(5f));
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (hit_Limit <= 0)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.right * speed * Time.deltaTime);

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
            if (enemyScript != null && !DamageEnemiess.Contains(hit.collider.gameObject))
            {
                if (hit_Limit > 0)
                {
                    enemyScript.UpdateAttackingTower(tower);
                    enemyScript.TakeDamage(damage);
                    DamageEnemiess.Add(hit.collider.gameObject);
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
