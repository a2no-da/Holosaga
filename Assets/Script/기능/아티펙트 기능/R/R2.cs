using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 이로하 검기")]
public class R2 : ArtFunction
{
    public GameObject swordPrefab;
    public float damage;
    public float speed;
    public int hit;
    public float per; //0.1f
    public float upy;

    public override void FunctionAti(Unit unit)
    {
        if (Random.value <= per)
        {
            Vector3 spawnPosition = new Vector3(unit.transform.position.x, unit.transform.position.y + upy, unit.transform.position.z);
            GameObject swordgo = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
            R2_swordGi r2_swordGi = swordgo.GetComponent<R2_swordGi>();

            if (r2_swordGi != null)
            {
                r2_swordGi.Initialize(damage, speed, hit, unit);
            }
        }
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }
}
