using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 비쥬의 보석")]
public class U8 : ArtFunction
{
    public GameObject JewelPrefab;
    public float damage;
    public float upy;
    private float lastJewelTime = 0f; 
    public float jewelCooldown = 0.5f;

    public override void FunctionAti(Unit unit)
    {
        if (unit is Tower tower)
        {
            Jewel(tower);
        }
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }

    public void Jewel(Tower tower)
    {
        if (tower == null || tower.isDead || Time.time < lastJewelTime + jewelCooldown) return;

        Vector3 spawnPosition = new Vector3(tower.transform.position.x, tower.transform.position.y + upy, tower.transform.position.z);
        GameObject yoyo = Instantiate(JewelPrefab, spawnPosition, Quaternion.identity);
        U8_Jewel u8_Jewel = yoyo.GetComponent<U8_Jewel>();

        if (u8_Jewel != null)
        {
            u8_Jewel.Initialize(tower.ArtHPoint, tower);
        }

        lastJewelTime = Time.time;
    }
}
