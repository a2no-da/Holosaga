using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ä«µå / ²ö²öÀÌ")]
public class C3 : AbilityFunction
{
    public GameObject GGEE;

    public override void FunctionCard(Tower tower)
    {
    }

    public override void FunctionDesCard(Tower tower)
    {
    }

    public override void FunctionPulsACard(Unit Unit)
    {
    }

    public override void FunctionWorldCard()
    {
        Vector3 spawnPosition = new Vector3(7.52f, -1.6f, 0f);
        Instantiate(GGEE, spawnPosition, Quaternion.identity);
    }
}
