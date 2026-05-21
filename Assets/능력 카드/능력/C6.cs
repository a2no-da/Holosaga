using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "카드 / 최고의 파트너")]
public class C6 : AbilityFunction
{
    public GameObject Red;
    public GameObject Blue;
    public int myi = 0;

    public override void FunctionCard(Tower tower)
    {
        if (tower.isEx) { return; }
    }

    public override void FunctionDesCard(Tower tower)
    {
        if (tower.isEx) { return; }
        Vector3 spawnPosition = tower.transform.position + new Vector3(0, 0, 0f);
        switch (myi)
        {
            case 0:
                GameObject red = Instantiate(Red, spawnPosition, Quaternion.identity, tower.transform);
                AoAka raoaka = red.GetComponent<AoAka>();
                if (raoaka != null)
                {
                    raoaka.Initialize(tower);
                }
                myi = 1;
                break;
            case 1:
                GameObject blue = Instantiate(Blue, spawnPosition, Quaternion.identity, tower.transform);
                AoAka baoaka = blue.GetComponent<AoAka>();
                if (baoaka != null)
                {
                    baoaka.Initialize(tower);
                }
                myi = 0;
                break;
        }
    }

    public override void FunctionPulsACard(Unit Unit)
    {
    }

    public override void FunctionWorldCard()
    {
    }
}
