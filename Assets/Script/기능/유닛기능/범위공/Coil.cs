using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coil : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public int hit_Limit;
    private Vector3 direction = Vector3.left;
    public Tower tower;

    public void Initialize(float damage, float speed, int hit_Limit, Vector3 direction, Tower tower)
    {
        this.damage = damage;
        this.speed = speed;
        this.hit_Limit = hit_Limit;
        this.direction = direction.normalized;
        this.tower = tower;
    }

    void Start()
    {
        InitializeCcoil(damage, hit_Limit);
        StartCoroutine(DestroyAfterSeconds(4f));
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void InitializeCcoil(float damage, int hitLimit)
    {
        Ccoil ccoil = GetComponentInChildren<Ccoil>();
        if (ccoil != null)
        {
            ccoil.Initialize(damage, hitLimit, tower);
        }
    }
}
