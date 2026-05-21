using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ccoil : MonoBehaviour
{
    private float damage;
    public int hit_Limit;
    public Tower tower;

    public void Initialize(float damage, int hit_Limit, Tower tower)
    {
        this.damage = damage;
        this.hit_Limit = hit_Limit;
        this.tower = tower;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.UpdateAttackingTower(tower);
                enemy.TakeDamage(damage);
                hit_Limit--;

                if (hit_Limit <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
