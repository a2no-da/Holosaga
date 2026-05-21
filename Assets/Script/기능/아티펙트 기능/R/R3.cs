using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 코요리 장판")]
public class R3 : ArtFunction
{
    public GameObject damageFieldPrefab;
    public float cool;
    private List<GameObject> fields = new List<GameObject>();

    public override void FunctionAti(Unit unit)
    {
        GameObject field = Instantiate(damageFieldPrefab, unit.transform.position, Quaternion.identity, unit.transform);
        R3_Plate plate = field.GetComponent<R3_Plate>();

        if (plate != null)
        {
            plate.Initialize(unit, cool);
            fields.Add(field);
        }
    }

    public override void FunctionDesAti(Unit unit)
    {
        if (unit != null)
        {
            foreach (GameObject field in fields)
            {
                if (field != null)
                {
                    R3_Plate plate = field.GetComponent<R3_Plate>();
                    if (plate != null && unit.Equals(plate.GetUnit()))
                    {
                        if (field.activeSelf)
                        {
                            field.SetActive(false);
                        }
                        else
                        {
                            field.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public override void FunctionAlltime(Unit unit)
    {
    }
}
