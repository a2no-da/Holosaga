using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ä«µå / ¾ÞÄÝ")]
public class C8 : AbilityFunction
{
    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.AngelBuff(towerGameObject, "¿£Á©");
    }

    public override void FunctionDesCard(Tower tower)
    {
    }

    public override void FunctionPulsACard(Unit Unit)
    {
    }

    public override void FunctionWorldCard()
    {
    }
}
