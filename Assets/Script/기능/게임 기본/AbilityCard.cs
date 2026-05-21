using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    SimpleBuff,
    PlusAttack,
    HelpObject,
    PlusActive
}

public enum NeedType
{
    None,
    범위공격,
    투사체,
    서포터
}

[CreateAssetMenu(menuName = "능력 카드")]

public class AbilityCard : ScriptableObject
{
    public AbilityType abilitytype;
    public NeedType needtype;
    public int index;
    public string CardName;
    public int needtower;
    public Sprite sprit;
    public AbilityFunction abilityFunction;
}


