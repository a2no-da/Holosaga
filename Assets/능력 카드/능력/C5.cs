using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 길어진 팔")]
public class C5 : AbilityFunction
{
    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.ApplyBuff(towerGameObject, "기능카드5", tower.myB[16].duration, tower.myB[16].powerUp, tower.myB[16].speedUp, tower.myB[16].HpUp);
        tower.RangeX = tower.RangeX + 2;
        tower.toCollider();
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
