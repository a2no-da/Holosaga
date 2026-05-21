using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityFunction : ScriptableObject
{
    public abstract void FunctionCard(Tower tower);

    public abstract void FunctionDesCard(Tower tower);

    public abstract void FunctionPulsACard(Unit Unit);

    public abstract void FunctionWorldCard();
}
