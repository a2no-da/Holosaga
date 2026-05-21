using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "기능 / 라플라스의 거대한 뿔")]
public class U7 : ArtFunction
{
    public GameObject P1Pre;
    public GameObject P2Pre;
    public GameObject P3Pre;
    public float damage;
    public float upy;

    public override void FunctionAti(Unit unit)
    {
        if (unit is Tower tower)
        {
            float randomValue = Random.value; 

            if (randomValue <= 0.65f) 
            {
                P1(tower);
            }
            else if (randomValue <= 0.85f)
            {
                P2(tower);
            }
            else 
            {
                P3(tower);
            }
        }
    }

    public override void FunctionDesAti(Unit unit)
    {
    }

    public override void FunctionAlltime(Unit unit)
    {
    }

    public void P1(Tower tower)
    {
        GameObject towerGameObject = tower.gameObject;
        BuffManager.Instance.ApplyBuff(towerGameObject, "라플뿔벞1", 5, 0.3f, 0, 0.3f);
        GameObject p11 = Instantiate(P1Pre, tower.transform.position, Quaternion.identity, tower.transform);
    }

    public void P2(Tower tower)
    {
        GameObject p22 = Instantiate(P2Pre, tower.transform.position, Quaternion.identity, tower.transform);
        tower.StartCoroutine(ApplyCooldownReduction(tower));
    }

    private IEnumerator ApplyCooldownReduction(Tower tower)
    {
        yield return new WaitForSeconds(0.1f); 
        tower.ActiveCooldown = tower.ActiveCooldown * 0.5f; 
    }

    public void P3(Tower tower)
    {
        Vector3 spawnPosition = new Vector3(tower.transform.position.x, tower.transform.position.y + upy, tower.transform.position.z);
        GameObject pp = Instantiate(P3Pre, spawnPosition, Quaternion.identity);
        U7_LaPB u7_La = pp.GetComponent<U7_LaPB>();

        if (u7_La != null)
        {
            u7_La.Initialize(tower);
        }
    }
}
