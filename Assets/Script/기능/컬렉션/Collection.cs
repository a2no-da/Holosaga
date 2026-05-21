using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using Spine.Unity;

public class Collection : MonoBehaviour
{
    public Holomoney holomoney;

    public GameObject NoMoney;
    public GameObject NoExp;
    public Button[] towerButtons;
    public Button[] enemyButtons;
    public Dicton[] towerInfos;
    public Dicton[] enemyInfos;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI unlockText;
    public TextMeshProUGUI Lo;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI coldasi;
    public GameObject Tanime;
    public GameObject Eanime;
    private GameObject currentAnimation;
    public GameObject TowerSp;
    public Vector3 originalPosition;
    public Vector3 originalPosition2;
    public Image roleT;
    public Sprite[] im;

    private int index;
    private Unit currentUnit;

    public GameObject[] units;
    public GameObject[] enemys;
    public GameObject[] Stars;
    public CategoryButtonManager cate;
    public GameObject[] Page;
    public TextMeshProUGUI NPageT;
    private int currentPageIndex = 0;

    void Start()
    {
        Dicton unitInfo = towerInfos[index];
        Dicton enemyInfo = enemyInfos[index];
        Lo.gameObject.SetActive(false);

        if (towerInfos.Length > 0)
        {
            unitInfo.LoadLocalizedText(unitInfo.descriptionKey);

            foreach (Dicton dicton in towerInfos)
            {
                dicton.LoadLocalizedText(dicton.descriptionKey);
            }
        }

        if (enemyInfos.Length > 0)
        {
            enemyInfo.MLoadLocalizedText(enemyInfo.descriptionKey);

            foreach (Dicton dicton in enemyInfos)
            {
                dicton.MLoadLocalizedText(dicton.descriptionKey);
            }
        }
        Col();
    }

    public void ShowInfo(Unit unit)
    {
        Lo.gameObject.SetActive(true);

        index = Array.IndexOf(units, unit.gameObject);

        Dicton unitInfo = towerInfos[index];

        if (unitInfo.isPurchasable)
        {
        }
        else
        {
            if (currentAnimation != null)
            {
                Destroy(currentAnimation);
            }

            currentAnimation = Instantiate(unitInfo.originprefab, originalPosition, Quaternion.identity, Tanime.transform);

            nameText.text = unitInfo.Name;
            powerText.text = $"POWER {unit.Power}";
            hpText.text = $"HP {unit.MaxHealth}";
            unlockText.text = unitInfo.unlock;
            descriptionText.text = unitInfo.sul;
            TowerSp.SetActive(true);

            switch (unitInfo.role)
            {
                case DictonRole.Attacker:
                    roleT.sprite = im[0];
                    break;
                case DictonRole.Projectiler:
                    roleT.sprite = im[1];
                    break;
                case DictonRole.Supporter:
                    roleT.sprite = im[2];
                    break;
                default:
                    break;
            }

            if (unitInfo.LevelSS == 1)
            {
                Stars[0].SetActive(false);
                Stars[1].SetActive(false);
            }

            if (unitInfo.LevelSS == 2)
            {
                Stars[0].SetActive(true);
                Stars[1].SetActive(false);
            }

            if (unitInfo.LevelSS == 3)
            {
                Stars[0].SetActive(true);
                Stars[1].SetActive(true);
            }
        }
    }

    public void UpPanaltrue()
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        if (unitInfo.MYexp < unitInfo.exp)
        {
            NoExp.gameObject.SetActive(true);
            return;
        }

        if (unitInfo.Holomo > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            return;
        }

    }

    public void ShowInfoEnemy(Unit unit)
    {
        Lo.gameObject.SetActive(true);
        index = Array.IndexOf(enemys, unit.gameObject);

        if (index < 0)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        Dicton unitInfo = enemyInfos[index];
        unlockText.text = unitInfo.unlock;

        if (unitInfo.isPurchasable)
        {
        }
        else
        {
            if (currentAnimation != null)
            {
                Destroy(currentAnimation);
            }

            nameText.text = unitInfo.Name;

            Vector3 spawnPosition = originalPosition2;
            /*if (unitInfo.id == "012")
            {
                spawnPosition = new Vector3(6.73f, -1.4f, 0);
            }*/

            currentAnimation = Instantiate(unitInfo.prefab, spawnPosition, Quaternion.identity, Eanime.transform);

            if (unitInfo.descriptionKey == "Tsumire")
            {
                var skeletonAnimation = currentAnimation.GetComponent<SkeletonAnimation>();
                if (skeletonAnimation != null)
                {
                    string randomSkinName = UnityEngine.Random.Range(0, 2) == 0 ? "1" : "2";
                    skeletonAnimation.skeleton.SetSkin(randomSkinName);
                    skeletonAnimation.skeleton.SetSlotsToSetupPose();
                }
            }

            powerText.text = $"POWER {unit.Power}";
            hpText.text = $"HP {unit.MaxHealth}";
            Lo.gameObject.SetActive(true);

            descriptionText.text = unitInfo.sul;
        }
    }

    public void ResetInfo()
    {
        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
            currentAnimation = null;
        }
        Lo.gameObject.SetActive(false);
        nameText.text = "";
        powerText.text = "";
        hpText.text = "";
        unlockText.text = "";
        descriptionText.text = "";
    }

    public void Col()
    {
        Dicton[] currentInfos;
        int totalCount;

        if (cate.windows[0].activeSelf)
        {
            currentInfos = towerInfos;
            totalCount = currentInfos.Length;
            //totalCount -= 1;
        }
        else
        {
            currentInfos = enemyInfos;
            totalCount = currentInfos.Length;
        }

        int notPurchasableCount = 0;

        foreach (var item in currentInfos)
        {
            if (!item.isPurchasable)
            {
                notPurchasableCount++;
            }
        }


        coldasi.text = $"{notPurchasableCount} / {totalCount}";
    }

    public void test()
    {
        Dicton unitInfo7 = towerInfos[7];
        Dicton unitInfo8 = towerInfos[8];

        PlayerData data = holomoney.LoadPlayerData();

        unitInfo7.isPurchasable = false;
        unitInfo8.isPurchasable = false;

        unitInfo7.SaveAll();
        unitInfo8.SaveAll();
    }

    public void Ja()
    {
        if (currentPageIndex - 1 >= 0)
        {
            currentPageIndex--;
            UpdatePage();
        }
    }

    public void Wu()
    {
        if (currentPageIndex + 1 < Page.Length)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        bool allInactive = true;
        foreach (GameObject page in Page)
        {
            if (page.activeSelf)
            {
                allInactive = false;
                break;
            }
        }

        if (allInactive)
        {
            return;
        }

        NPageT.text = (currentPageIndex + 1).ToString() + " / " + Page.Length;

        for (int i = 0; i < Page.Length; i++)
        {
            Page[i].SetActive(i == currentPageIndex);
        }
    }

    public void ResetPage(bool oneok)
    {
        currentPageIndex = 0;

        if (oneok)
        {
            for (int i = 0; i < Page.Length; i++)
            {
                Page[i].SetActive(false);
                NPageT.text = (currentPageIndex + 1).ToString() + " / " + "1";
            }
        }
        else
        {
            for (int i = 0; i < Page.Length; i++)
            {
                Page[i].SetActive(i == currentPageIndex);
                NPageT.text = (currentPageIndex + 1).ToString() + " / " + Page.Length;
            }
        }
    }
}
