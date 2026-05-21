using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 묵직한 공격")]
public class C12 : AbilityFunction
{
    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.ApplyBuff(towerGameObject, "기능카드12", tower.myB[23].duration, tower.myB[23].powerUp, tower.myB[23].speedUp, tower.myB[23].HpUp);
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
