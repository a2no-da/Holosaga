using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulTama : MonoBehaviour
{
    public float lifeTime = 28f;
    public float damage;
    public float bulletSpeed = 10f;
    public int hit_Limit;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(float damage, float speed, int hit)
    {
        this.damage = damage;
        this.bulletSpeed = speed;
        this.hit_Limit = hit;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * bulletSpeed * Time.deltaTime);
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tower"))
        {
            if (other.transform.parent)
            {
                Tower tower = other.transform.parent.GetComponent<Tower>();
                if (tower != null)
                {
                    if (hit_Limit > 0)
                    {
                        tower.TakeDamage(damage);
                        hit_Limit--;
                    }
                }
            }
        }
    }
}
