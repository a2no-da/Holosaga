using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 자리 사수")]
public class C4 : AbilityFunction
{
    public override void FunctionCard(Tower tower)
    {
        tower.isSW = true;
        tower.isCardSW = true;

        if (tower.isEx) { return; }
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.ApplyBuff(towerGameObject, "기능카드4", tower.myB[15].duration, tower.myB[15].powerUp, tower.myB[15].speedUp, tower.myB[15].HpUp);
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
