using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Padong : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public int hit_Limit;
    public int Times;
    public Lui lui;
    private float maxX;
    private Vector3 direction = Vector3.left;

    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    public void Initialize(float damage, float speed, int hit_Limit, int Time, Vector3 direction)
    {
        this.damage = damage;
        this.speed = speed;
        this.Times = Time;
        this.hit_Limit = hit_Limit;
        this.direction = direction.normalized;
    }

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(3f));
        maxX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        InitializePadodong(damage, hit_Limit); 
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    void InitializePadodong(float damage, int hitLimit)
    {
        Padodong padodong = GetComponentInChildren<Padodong>();
        if (padodong != null)
        {
            padodong.Initialize(damage, hitLimit);
        }
    }
}
