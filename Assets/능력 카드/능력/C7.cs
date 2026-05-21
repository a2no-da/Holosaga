using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ä«µå / ¹öÅß!")]
public class C7 : AbilityFunction
{
    public int plus;

    public override void FunctionCard(Tower tower)
    {
        tower.H_regen = tower.H_regen + plus;
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
