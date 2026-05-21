using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class GGE : MonoBehaviour
{
    public float slow;

    public void Initialize(float Slow)
    {
        this.slow = Slow;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SlowMon(slow);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ResetedSpeed();
        }
    }
}
