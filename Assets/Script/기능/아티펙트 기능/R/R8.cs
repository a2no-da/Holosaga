using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 타코다치의 번쩍이는 엔젤링")]
public class R8 : ArtFunction
{
    public GameObject TakoPrefab;
    public float damage;
    public float upy;
    private Unit unit;

    public override void FunctionAti(Unit unit)
    {
        this.unit = unit;
        unit.StartCoroutine(InvokeFireTako());
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }

    private IEnumerator InvokeFireTako()
    {
        while (unit != null)
        {
            FireTako();
            yield return new WaitForSeconds(10f);
        }
    }

    public void FireTako()
    {
        if (unit == null || unit.isDead) return;

        Vector3 spawnPosition = new Vector3(unit.transform.position.x, unit.transform.position.y + upy, unit.transform.position.z);
        GameObject tako = Instantiate(TakoPrefab, spawnPosition, Quaternion.identity);
        R8_TakoBeam r8_TakoBeam = tako.GetComponent<R8_TakoBeam>();

        if (r8_TakoBeam != null)
        {
            r8_TakoBeam.Initialize(damage, unit);
        }
    }
}
