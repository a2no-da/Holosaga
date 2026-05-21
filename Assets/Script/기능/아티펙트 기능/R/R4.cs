using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(menuName = "기능 / 클로에의 후드")]
public class R4 : ArtFunction
{
    private Unit unit;
    private Tower tower;
    public GameObject vfx;

    public override void FunctionAti(Unit unit)
    {
        this.unit = unit;
        if (unit is Tower tower)
        {
            this.tower = tower;
        }
        unit.StartCoroutine(HoodCoroutine());
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }

    private IEnumerator HoodCoroutine()
    {
        while (unit != null)
        {
            while (unit.isDead)
            {
                yield return null;
            }

            SetSkeletonAlpha(150 / 255f);
            tower.Body.SetActive(false);
            unit.Stop = true;
            Instantiate(vfx, unit.transform.position, Quaternion.identity, unit.transform);

            yield return new WaitForSeconds(2f);

            SetSkeletonAlpha(1f);
            tower.Body.SetActive(true);
            unit.Stop = false;

            yield return new WaitForSeconds(6f); 
        }

        SetSkeletonAlpha(1f);
        tower.Body.SetActive(true);
        unit.Stop = false;
    }

    private void SetSkeletonAlpha(float alpha)
    {
        if (unit.skeletonAnimation != null)
        {
            unit.skeletonAnimation.skeleton.SetColor(new Color(1f, 1f, 1f, alpha)); 
            unit.skeletonAnimation.Update(0);
        }
    }
}
