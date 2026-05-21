using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 시옷코의 머리핀")]
public class R9 : ArtFunction
{
    public GameObject jang;
    public float damage;
    public float per;

    public override void FunctionAti(Unit unit)
    {
        bool isjang = UnityEngine.Random.Range(0f, 100f) < per;

        if (isjang)
        {
            Vector3 spawnPosition = new Vector3(unit.transform.position.x, unit.transform.position.y, unit.transform.position.z);
            GameObject jangp = Instantiate(jang, spawnPosition, Quaternion.identity);
            R9_Jang r9_Jang = jangp.GetComponent<R9_Jang>();

            if (r9_Jang != null)
            {
                Debug.Log("생성");
                r9_Jang.Initialize(damage, unit);
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
