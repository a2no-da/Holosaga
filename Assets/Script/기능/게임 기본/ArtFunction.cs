using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtFunction : ScriptableObject
{
    public abstract void FunctionAti(Unit unit);

    public abstract void FunctionDesAti(Unit unit);

    public abstract void FunctionAlltime(Unit unit);
}
