using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 포요요의 뿔")]
public class N11 : ArtFunction
{
    public GameObject PoyoyoPrefab;
    public float damage;
    public float speed;
    public int hit;
    public float upy;
    private Unit unit;

    public override void FunctionAti(Unit unit)
    {
        this.unit = unit;
        unit.StartCoroutine(InvokeFirePoyoyo());
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }

    private IEnumerator InvokeFirePoyoyo()
    {
        while (unit != null)
        {
            FirePoyoyo();
            yield return new WaitForSeconds(5f);
        }
    }

    public void FirePoyoyo()
    {
        if (unit == null || unit.isDead) return;

        Vector3 spawnPosition = new Vector3(unit.transform.position.x, unit.transform.position.y + upy, unit.transform.position.z);
        GameObject yoyo = Instantiate(PoyoyoPrefab, spawnPosition, Quaternion.identity);
        N11_SoulTama n11_SoulTama = yoyo.GetComponent<N11_SoulTama>();

        if (n11_SoulTama != null)
        {
            n11_SoulTama.Initialize(damage, speed, hit, unit);
        }
    }
}
