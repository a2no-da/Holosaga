using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public Gura gura;
    public float damage;
    protected List<GameObject> enemiesInRange = new List<GameObject>();

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Add(other.gameObject);
            enemy.UpdateAttackingTower(gura);
            enemy.TakeDamage(damage);
        }
    }
}
