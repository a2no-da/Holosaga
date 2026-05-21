using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 오버클럭")]
public class C11 : AbilityFunction
{
    public GameObject WindPrefab;
    public GameObject WETPPrefab;
    public float damage;
    public float speed;
    public int hit;
    public float per;

    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
    }

    public override void FunctionDesCard(Tower tower)
    {
    }

    public override void FunctionPulsACard(Unit Unit)
    {
        Vector3 spawnPosition = Unit.transform.position + new Vector3(0, 0, 0f);
        GameObject wind = Instantiate(WindPrefab, spawnPosition, Quaternion.identity);
        Instantiate(WETPPrefab, Unit.transform.position, Quaternion.identity, Unit.transform);

        AcWind wd = wind.GetComponent<AcWind>();
        if (wd != null)
        {
            wd.Initialize(damage, speed, hit, Unit);
        }
    }

    public override void FunctionWorldCard()
    {
    }
}
