using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 탄창 보급")]
public class C10 : AbilityFunction
{
    public GameObject MbulletPrefab;
    public float perdamage;
    public float speed;
    public int hit;
    public float delay; 

    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
    }

    public override void FunctionDesCard(Tower tower)
    {
    }

    public override void FunctionPulsACard(Unit Unit)
    {
        if (Unit.dicton.rolePlay == "투사체")
        {
            SpawnBullet(Unit);
        }
    }

    private void SpawnBullet(Unit Unit)
    {
        Vector3 spawnPosition = Unit.transform.position + new Vector3(-0.5f, 0, 0f);
        GameObject manab = Instantiate(MbulletPrefab, spawnPosition, Quaternion.identity);

        MaBullet mb = manab.GetComponent<MaBullet>();
        if (mb != null)
        {
            mb.Initialize(Unit.Power * perdamage, speed, hit, Unit, false, delay);
        }

        GameObject bullet2 = Instantiate(MbulletPrefab, spawnPosition, Quaternion.identity);
        MaBullet mb2 = bullet2.GetComponent<MaBullet>();
        if (mb2 != null)
        {
            mb2.Initialize(Unit.Power * perdamage, speed, hit, Unit, true, delay);
        }

    }

    public override void FunctionWorldCard()
    {
    }
}
