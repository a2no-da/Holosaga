using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Grade
{
    Normal,
    Epic,
    Unique
}

public enum Type
{
    체력추가,
    공속퍼추가,
    파워퍼추가,
    어택공속퍼추가,
    크리티컬추가
}

public enum MyType
{
    None,
    Active_Attack,
    Passive_Attack,
    SW
}

[CreateAssetMenu(menuName = "아티펙트")]

public class Artifact : ScriptableObject
{
    public string artifactId;
    public Grade grade;
    public Type type;
    public MyType mytype;
    public int index;
    public string artifactName;
    public float hpplus;
    public float hpIncrease;
    public float Speedplus;
    public float SpeedIncrease;
    public float Powerplus;
    public float PowerIncrease;
    public float ASpeedplus;
    public float ASpeedIncrease;
    public float CriIncrease;
    public int Quantity;
    public int Price;
    public Sprite sprit;
    public ArtFunction artfunction;
    public GameObject vfxarti;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
