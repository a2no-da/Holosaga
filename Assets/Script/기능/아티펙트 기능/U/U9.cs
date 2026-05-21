using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 폴카의 서커스 모자")]
public class U9 : ArtFunction
{
    public GameObject Chama;

    public override void FunctionAti(Unit unit)
    {
        GameObject oo = Instantiate(Chama, Vector3.zero, Quaternion.identity);
        ITAi iT = oo.GetComponent<ITAi>();

        if (iT != null)
        {
            iT.Initialize(unit);
        }
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }
}
