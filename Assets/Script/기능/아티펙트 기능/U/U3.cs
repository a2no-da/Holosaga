using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 코코의 발자국 화석")]
public class U3 : ArtFunction
{
    public GameObject FootPrefab;
    public float damage;
    public float upy;
    private bool olla = false; 

    public override void FunctionAti(Unit unit)
    {
        if (unit is Tower tower)
        {
            unit.StartCoroutine(HoodCoroutine(tower));
        }
    }

    private IEnumerator HoodCoroutine(Tower tower)
    {
        while (tower != null)
        {
            while (tower.isDead)
            {
                yield return null;
            }

            while (olla)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            olla = true;
        }
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
        if (unit.isDead) { return; }

        if (unit != null)
        {
            if (olla)
            {
                Vector3 spawnPosition = new Vector3(unit.transform.position.x, unit.transform.position.y + upy, unit.transform.position.z);
                GameObject foot = Instantiate(FootPrefab, spawnPosition, Quaternion.identity);
                U3_Foot u3_Foot = foot.GetComponent<U3_Foot>();

                if (u3_Foot != null)
                {
                    u3_Foot.Initialize(unit.MaxHealth * 0.2f, unit);
                }

                Camera.main.GetComponent<CameraShake>().StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.2f, 0.1f)); 

                olla = false;
            }
        }
    }
}
