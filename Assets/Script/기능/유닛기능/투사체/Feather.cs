using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public float damage2;
    public float damage3;
    public int hit_Limit;

    private Enemy target;
    public Tower tower;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    private List<Enemy> hitEnemies = new List<Enemy>();
    public GameObject featherSubPrefab;
    public GameObject BombPrefab;
    public bool isp3;
    private Vector3 Eneposition;

    public void Initialize(float damage, float speed, int hit, Tower tower, float damage2, float damage3, bool isp3)
    {
        this.damage = damage;
        this.speed = speed;
        this.hit_Limit = hit;
        this.tower = tower;
        this.damage2 = damage2;
        this.damage3 = damage3;
        this.isp3 = isp3;
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(5f));
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
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
                    Eneposition = enemyScript.spawnPoint.position;

                    float offsetX = UnityEngine.Random.Range(-0.7f, 0f);
                    float offsetY = UnityEngine.Random.Range(-0.4f, 0.4f);
                    Vector3 offset = new Vector3(offsetX, offsetY, 0);

                    Vector3 position = enemyScript.spawnPoint.position + offset;
                    float rotationZ = Mathf.Clamp(-offsetY * 100f, -45f, 45f);

                    Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

                    GameObject featherSub = Instantiate(featherSubPrefab, position, rotation);
                    featherSub.transform.parent = enemyScript.transform;
                    enemyScript.AddFeatherSub(featherSub);

                    if (enemyScript.GetFeatherSubCount() > 2) 
                    {
                        GameObject featherBomb = Instantiate(BombPrefab, enemyScript.transform.position, Quaternion.identity);
                        OverlapBomb Overlap = featherBomb.GetComponent<OverlapBomb>();

                        if (Overlap != null)
                        {
                            Overlap.Eneposition = Eneposition;
                            Overlap.Initialize(damage2, damage3, tower, isp3);
                        }

                        enemyScript.resetF();
                    }
                    enemyScript.TakeDamage(damage);

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
