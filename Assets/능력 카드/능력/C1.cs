using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 원맨쇼")]
public class C1 : AbilityFunction
{
    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.ApplyBuff(towerGameObject, "기능카드1", tower.myB[12].duration, tower.myB[12].powerUp, tower.myB[12].speedUp, tower.myB[12].HpUp);
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
