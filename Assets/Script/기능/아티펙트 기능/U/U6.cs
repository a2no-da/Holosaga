using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(menuName = "기능 / 카엘라의 망치")]
public class U6 : ArtFunction
{
    public GameObject ThreePrefab;
    public float upy;

    public override void FunctionAti(Unit unit)
    {
        unit.StartCoroutine(InvokeThree(unit));
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }

    private IEnumerator InvokeThree(Unit unit)
    {
        while (unit != null)
        {
            unit.atiCount += 1;
            FireTako(unit);
            yield return new WaitForSeconds(3f);
        }
    }

    public void FireTako(Unit unit)
    {
        if (unit == null || unit.isDead) return;

        Vector3 spawnPosition = new Vector3(unit.transform.position.x, unit.transform.position.y + upy, unit.transform.position.z);
        GameObject tr = Instantiate(ThreePrefab, spawnPosition, Quaternion.identity);
        U6_Three u6_Three = tr.GetComponent<U6_Three>();

        if (u6_Three != null)
        {
            u6_Three.Initialize(unit, unit.atiCount);
            if (u6_Three.skeletonAnimation != null)
            {
                if (unit.atiCount >= 3)
                {
                    u6_Three.skeletonAnimation.state.SetAnimation(0, "ayame2", false);
                    unit.atiCount = 0;
                }
                else
                {
                    u6_Three.skeletonAnimation.state.SetAnimation(0, "ayame1", false);
                }
            }
        }
    }
}
