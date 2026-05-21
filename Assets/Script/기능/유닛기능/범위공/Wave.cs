using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    private Enemy target;
    public Gura gura;

    public void Initialize(float damage, Gura gura)
    {
        this.damage = damage;
        this.gura = gura;
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(4f));
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = other.gameObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.UpdateAttackingTower(gura);
                enemyScript.TakeDamage(damage);
            }
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
