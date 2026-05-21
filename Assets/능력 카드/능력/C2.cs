using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 거인화")]
public class C2 : AbilityFunction
{
    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.ApplyBuff(towerGameObject, "기능카드2", tower.myB[13].duration, tower.myB[13].powerUp, tower.myB[13].speedUp, tower.myB[13].HpUp);
        tower.skeletonAnimation.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
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
