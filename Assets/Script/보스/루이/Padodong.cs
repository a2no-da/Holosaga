using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Padodong : MonoBehaviour
{
    private float damage;
    public int hit_Limit;

    public void Initialize(float damage, int hit_Limit)
    {
        this.damage = damage;
        this.hit_Limit = hit_Limit;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tower"))
        {
            Tower tower = other.GetComponentInParent<Tower>();

            if (tower != null)
            {
                tower.TakeDamage(damage);
                hit_Limit--;

                if (hit_Limit <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
