using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KnockbackInfo
{
    public GameObject enemy; 
    public float knockbackEndTime; 
    public Vector2 force; 

    public KnockbackInfo(GameObject enemy, float duration, Vector2 force)
    {
        this.enemy = enemy;
        this.knockbackEndTime = Time.time + duration;
        this.force = force;
    }
}

public class Knockback : MonoBehaviour
{
    private List<KnockbackInfo> knockbackInfos = new List<KnockbackInfo>();

    public void ApplyKnockback(GameObject enemy, float duration, Vector2 force)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            Debug.Log("¾Ó");
            var existingInfo = knockbackInfos.FirstOrDefault(k => k.enemy == enemy);
            if (existingInfo != null)
            {
                Debug.Log("¾Ó1");
                existingInfo.knockbackEndTime = Mathf.Max(existingInfo.knockbackEndTime, Time.time + duration);
                Debug.Log($"[ApplyKnockback] ³Ë¹é °»½Å: {enemy.name}, ½Ã°£: {existingInfo.knockbackEndTime}, Èû: {force}");
            }
            else
            {
                knockbackInfos.Add(new KnockbackInfo(enemy, duration, force));
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Debug.Log("¾Ó2");
                    enemyRb.AddForce(force, ForceMode2D.Impulse);
                }
                Debug.Log($"[ApplyKnockback] ³Ë¹é Àû¿ë: {enemy.name}, Áö¼Ó½Ã°£: {duration}, Èû: {force}");
            }
        }
        else
        {
            Debug.Log("¾Ó???");
        }
    }

    public void UpdateKnockback()
    {
        for (int i = knockbackInfos.Count - 1; i >= 0; i--)
        {
            KnockbackInfo info = knockbackInfos[i];
            if (Time.time >= info.knockbackEndTime)
            {
                Rigidbody2D enemyRb = info.enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.velocity = Vector2.zero; 
                }

                Enemy enemyComponent = info.enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.isPushedOrPulled = true;
                }
                knockbackInfos.RemoveAt(i);
                Debug.Log($"[UpdateKnockback] ³Ë¹é Á¤º¸ Á¦°Å: {info.enemy.name}");
            }
            else
            {
                Rigidbody2D enemyRb = info.enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(info.force * Time.deltaTime, ForceMode2D.Force);
                    Debug.Log($"[UpdateKnockback] ³Ë¹é Èû Àû¿ë: {info.enemy.name}, Èû: {info.force}");
                }
            }
        }
    }
}