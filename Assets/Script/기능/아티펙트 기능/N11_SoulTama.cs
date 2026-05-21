using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N11_SoulTama : MonoBehaviour
{
    public float lifeTime = 28f;
    public float damage;
    public float bulletSpeed = 10f;
    public int hit_Limit;
    public Tower tower;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(float damage, float speed, int hit, Unit unit)
    {
        this.bulletSpeed = speed;
        this.hit_Limit = hit;
        this.damage = damage;
        if (unit is Tower tower)
        {
            this.tower = tower;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (hit_Limit > 0)
                {
                    enemy.UpdateAttackingTower(tower);
                    enemy.TakeDamage(damage);
                    hit_Limit--;
                }
            }
        }
    }
}
