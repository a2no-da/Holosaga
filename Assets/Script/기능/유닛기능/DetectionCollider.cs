using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCollider : MonoBehaviour
{
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public GameObject GetLeftmostEnemy()
    {
        GameObject leftmostEnemy = null;
        float leftmostX = Mathf.Infinity;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            if (enemy.transform.position.x < leftmostX)
            {
                leftmostX = enemy.transform.position.x;
                leftmostEnemy = enemy;
            }
        }

        return leftmostEnemy;
    }
}
