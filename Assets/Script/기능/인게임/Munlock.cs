using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Munlock : MonoBehaviour
{
    public GameManager gameManager;
    public Dicton[] enemyInfos;

    void Update()
    {
        List<string> targetIds = GetTargetIds(gameManager.mainStage, gameManager.subStage);
        foreach (string targetId in targetIds)
        {
            if (!string.IsNullOrEmpty(targetId))
            {
                foreach (Dicton enemyInfo in enemyInfos)
                {
                    if (enemyInfo.id == targetId && enemyInfo.isPurchasable)
                    {
                        enemyInfo.isPurchasable = false;
                        enemyInfo.SaveAll();
                        break;
                    }
                }
            }
        }
    }

    List<string> GetTargetIds(int mainStage, int subStage)
    {
        List<string> targetIds = new List<string>();

        if (mainStage == 1)
        {
            if (subStage == 1 && CheckPurchasable("001")) targetIds.Add("001");
            if (subStage == 3 && CheckPurchasable("004")) targetIds.Add("004");
            if (subStage == 5 && CheckPurchasable("007")) targetIds.Add("007");
        }
        else if (mainStage == 2)
        {
            if (subStage == 1 && CheckPurchasable("002")) targetIds.Add("002");
            if (subStage == 4 && CheckPurchasable("005")) targetIds.Add("005");
            if (subStage == 5 && CheckPurchasable("008")) targetIds.Add("008");
        }
        else if (mainStage == 3)
        {
            if (subStage == 1 && CheckPurchasable("003")) targetIds.Add("003");
            if (subStage == 3)
            {
                if (CheckPurchasable("006")) targetIds.Add("006");
                if (CheckPurchasable("013")) targetIds.Add("013");
                if (CheckPurchasable("015")) targetIds.Add("015");
            }
            if (subStage == 5 && CheckPurchasable("009")) targetIds.Add("009");
        }
        else if (mainStage == 4)
        {
            if (subStage == 1 && CheckPurchasable("010")) targetIds.Add("010");
            if (subStage == 2 && CheckPurchasable("016")) targetIds.Add("016");
            if (subStage == 5 && CheckPurchasable("011")) targetIds.Add("011");
        }
        else if (mainStage == 5)
        {
            if (CheckPurchasable("012")) targetIds.Add("012");
        }
        else if (mainStage == 10)
        {
            if (subStage == 1 && CheckPurchasable("014")) targetIds.Add("014");
        }

        return targetIds;
    }

    bool CheckPurchasable(string id)
    {
        foreach (Dicton enemyInfo in enemyInfos)
        {
            if (enemyInfo.id == id)
            {
                return enemyInfo.isPurchasable;
            }
        }
        return false; 
    }
}

