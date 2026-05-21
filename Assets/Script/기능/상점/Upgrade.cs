using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using Spine.Unity;

public class Upgrade : MonoBehaviour
{
    public Holomoney holomoney;

    public GameObject NoMoney;
    public GameObject NoExp;
    public Button[] towerButtons;
    public Dicton[] towerInfos;

    public Image expFillImage;
    public Image PexpFillImage;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI powerinText;
    public TextMeshProUGUI hpinText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI Level2Text;
    public TextMeshProUGUI UpdateText;
    public TextMeshProUGUI RequestEXP;
    public TextMeshProUGUI RequestEXP2;
    public TextMeshProUGUI RequestLEXP;
    public TextMeshProUGUI updan;
    public TextMeshProUGUI LVupdan;
    public TextMeshProUGUI roleText;
    public TMP_Text holomoneyText;
    public TMP_Text enoughText;
    private GameObject currentAnimation;
    public GameObject up;
    public GameObject TowerBu;
    public GameObject TowerSp;
    public GameObject UpPanal;
    public Vector3 originalPosition;
    public Vector3 originalPosition2;

    private int index;
    private Unit currentUnit;
    public GameObject UpC;

    public GameObject[] units;
    public GameObject[] Stars;

    public GameObject Upgr;
    public GameObject FUpgr;

    public Image image;
    public P_Text p_Text;
    private int plus;
    private float p;
    private float h;
    private float pi;
    private float hi;

    private float orip;
    private float orih;
    private float oripi;
    private float orihi;

    public Unit uni;

    public Image roleT;
    public Sprite[] im;

    public TextMeshProUGUI[] UNLevelText;
    public Image[] UpFillImage;
    public TextMeshProUGUI[] UpLevelText;
    public TextMeshProUGUI[] UpLevel2Text;
    public TextMeshProUGUI[] UpperText;
    public GameObject[] buttonUpText;

    public GameObject atiopen;
    public GameObject noati;

    private float baseIncreasePercentage = 0.2f;
    private float IncreasePercentage = 0.05f;
    public ArtifactS artifacts;

    void Start()
    {
        Dicton unitInfo = towerInfos[index];

        if (towerInfos.Length > 0)
        {
            unitInfo.LoadLocalizedText(unitInfo.descriptionKey);

            foreach (Dicton dicton in towerInfos)
            {
                dicton.LoadLocalizedText(dicton.descriptionKey);
            }
        }
    }

    public void ShowInfo(Unit unit)
    {
        index = Array.IndexOf(units, unit.gameObject);

        Dicton unitInfo = towerInfos[index];
        UpC.SetActive(true);
        uni = unit;

        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
        }

        currentAnimation = Instantiate(unitInfo.prefab, originalPosition, Quaternion.identity, image.transform);

        nameText.text = unitInfo.Name;
        UpdateText.text = "UPGRADE " + unitInfo.LevelSS.ToString();
        PexpFillImage.gameObject.SetActive(true);
        PexpFillImage.fillAmount = (float)unitInfo.LevelSS / 3f;

        orip = unit.Power;
        orih = unit.MaxHealth;
        p = orip + (orip * baseIncreasePercentage * unitInfo.PowerUp);
        h = orih + (orih * baseIncreasePercentage * unitInfo.HpUp);

        orihi = unit.healthIncreasePerLevel;
        oripi = unit.powerIncreasePerLevel;

        pi = oripi + (oripi * IncreasePercentage * unitInfo.PowerInUp);
        hi = orihi + (orihi * IncreasePercentage * unitInfo.HpInUp);

        powerText.text = FormatValue(p);
        hpText.text = FormatValue(h);
        hpinText.text = FormatValue(hi);
        powerinText.text = FormatValue(pi);

        roleText.text = unitInfo.rolePlay;
        TowerBu.SetActive(true);
        TowerSp.SetActive(true);
        RequestEXP.text = unitInfo.Holomo.ToString();
        RequestLEXP.text = unitInfo.HolomoL.ToString();
        p_Text.towerinfo = unitInfo;
        p_Text.Re();
        p_Text.LoadLocalizedText(1);

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
            Upgr.SetActive(true);
        }

        if (unitInfo.LevelSS == 2)
        {
            Upgr.SetActive(true);
            Stars[0].SetActive(true);
            Stars[1].SetActive(false);
        }

        if (unitInfo.LevelSS == 3)
        {
            Stars[0].SetActive(true);
            Stars[1].SetActive(true);
            Upgr.SetActive(false);
            PexpFillImage.fillAmount = 1 / 1;
        }

        updaover(unitInfo);
        mstat(unitInfo);
    }

    public void updaover(Dicton unitInfo)
    {
        if (unitInfo.HpUp == 5)
        {
            buttonUpText[0].SetActive(false);
        }
        else
        {
            buttonUpText[0].SetActive(true);
        }

        if (unitInfo.PowerUp == 5)
        {
            buttonUpText[1].SetActive(false);
        }
        else
        {
            buttonUpText[1].SetActive(true);
        }

        if (unitInfo.HpInUp == 5)
        {
            buttonUpText[2].SetActive(false);
        }
        else
        {
            buttonUpText[2].SetActive(true);
        }

        if (unitInfo.PowerInUp == 5)
        {
            buttonUpText[3].SetActive(false);
        }
        else
        {
            buttonUpText[3].SetActive(true);
        }
    }

    public void ShowInfoAt(Unit unit)
    {
        index = Array.IndexOf(units, unit.gameObject);

        Dicton unitInfo = towerInfos[index];
        UpC.SetActive(true);
        atiopen.SetActive(true);
        uni = unit;
        artifacts.SelTower = uni;

        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
        }

        currentAnimation = Instantiate(unitInfo.prefab, originalPosition, Quaternion.identity, image.transform);

        nameText.text = unitInfo.Name;

        orip = unit.Power;
        orih = unit.MaxHealth;
        p = orip + (orip * baseIncreasePercentage * unitInfo.PowerUp);
        h = orih + (orih * baseIncreasePercentage * unitInfo.HpUp);

        orihi = unit.healthIncreasePerLevel;
        oripi = unit.powerIncreasePerLevel;

        pi = oripi + (oripi * IncreasePercentage * unitInfo.PowerInUp);
        hi = orihi + (orihi * IncreasePercentage * unitInfo.HpInUp);

        powerText.text = FormatValue(p);
        hpText.text = FormatValue(h);
        hpinText.text = FormatValue(hi);
        powerinText.text = FormatValue(pi);

        TowerBu.SetActive(true);
        TowerSp.SetActive(true);
        artifacts.dicton = unitInfo;
        artifacts.first(unitInfo.myartifact);

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

        mstatAt(unitInfo);
    }

    private string FormatValue(float value)
    {
        float truncatedValue = (float)(Math.Floor(value * 1000) / 1000);
        return truncatedValue.ToString("0.###");
    }

    public void PlText()
    {
        float hh = h;
        float pp = p;

        Dicton unitInfo = towerInfos[index];

        if (unitInfo.myartifact.hpIncrease != 0)
        {
            hh = hh + (hh * unitInfo.myartifact.hpIncrease / 100);
        }

        if (unitInfo.myartifact.PowerIncrease != 0)
        {
            pp = pp + (pp * unitInfo.myartifact.PowerIncrease / 100);
        }

        if (unitInfo.myartifact.hpplus != 0)
        {
            hh = hh + unitInfo.myartifact.hpplus;
        }

        if (unitInfo.myartifact.Powerplus != 0)
        {
            pp = pp + unitInfo.myartifact.Powerplus;
        }

        powerText.text = pp.ToString();
        hpText.text = hh.ToString();
    }

    public void MuText()
    {
        Dicton unitInfo = towerInfos[index];

        powerText.text = p.ToString();
        hpText.text = h.ToString();
    }

    public void mstat(Dicton unitInfo) 
    {
        string[] upgradeValues = { "500", "1000", "1500", "2000", "2500", "2500"};
        int[] upgradeLevels = { unitInfo.HpUp, unitInfo.PowerUp, unitInfo.HpInUp, unitInfo.PowerInUp };

        for (int j = 0; j < upgradeLevels.Length; j++)
        {
            if (upgradeLevels[j] >= 0 && upgradeLevels[j] < upgradeValues.Length)
            {
                UNLevelText[j].text = upgradeValues[upgradeLevels[j]];
            }

            UpFillImage[j].fillAmount = upgradeLevels[j] / 5f;

            UpLevelText[j].text = "LV" + upgradeLevels[j].ToString();
            UpLevel2Text[j].text = "LV " + upgradeLevels[j].ToString();

            if (j == 0 || j == 1)
            {
                UpperText[j].text = (upgradeLevels[j] * 20).ToString() + " %";
            }
            else
            {
                UpperText[j].text = (upgradeLevels[j] * 5).ToString() + " %";
            }
        }
    }

    public void mstatAt(Dicton unitInfo)
    {
        int[] upgradeLevels = { unitInfo.HpUp, unitInfo.PowerUp, unitInfo.HpInUp, unitInfo.PowerInUp };

        for (int j = 0; j < upgradeLevels.Length; j++)
        {
            UpLevel2Text[j].text = "LV " + upgradeLevels[j].ToString();
        }
    }

    public void ResetInfo()
    {
        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
            currentAnimation = null;
        }
        UpC.gameObject.SetActive(false);
        PexpFillImage.gameObject.SetActive(false);
        nameText.text = "";
        powerText.text = "";
        hpText.text = "";
        p_Text.Re();

        for (int i = 0; i < UpLevelText.Length; i++)
        {
            UpLevelText[i].text = "LV0"; 
        }

        for (int i = 0; i < UNLevelText.Length; i++)
        {
            UNLevelText[i].text = "500"; 
        }

        for (int i = 0; i < UpFillImage.Length; i++)
        {
            UpFillImage[i].fillAmount = 0f; 
        }
    }

    public void ResetInfoAt()
    {
        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
            currentAnimation = null;
        }
        UpC.gameObject.SetActive(false);
        noati.SetActive(false);
        atiopen.SetActive(true);
        artifacts.Reset();
        nameText.text = "";
        powerText.text = "";
        hpText.text = "";
    }

    public void YesUP()
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        if (unitInfo.LevelSS == 3)
        {
            return;
        }

        if (unitInfo.Holomo > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            int nM = unitInfo.Holomo - data.holomoney;
            //enoughText.text = nM.ToString();
            return;
        }

        if (unitInfo.LevelSS == 2)
        {
            data.holomoney -= unitInfo.Holomo;
            unitInfo.LevelSS = 3;
            Upgr.SetActive(false);
            PexpFillImage.fillAmount = 1 / 1;
        }

        if (unitInfo.LevelSS == 1)
        {
            data.holomoney -= unitInfo.Holomo;
            unitInfo.Holomo += 300;
            unitInfo.LevelSS = 2;

            PexpFillImage.fillAmount = (float)unitInfo.LevelSS / 3f;
        }

        GameObject spawnedUp = Instantiate(up, currentAnimation.transform.position, Quaternion.identity);
        spawnedUp.transform.SetParent(currentAnimation.transform, false);
        spawnedUp.transform.localScale = new Vector3(1, 1, 1);
        upep ep = spawnedUp.GetComponent<upep>();
        ep.Uone();

        SkeletonGraphic skeletonGraphic = spawnedUp.GetComponent<SkeletonGraphic>();
        skeletonGraphic.AnimationState.SetAnimation(0, "upgrade", false);

        Up(currentUnit);
        p_Text.UpG();
        RequestEXP.text = unitInfo.Holomo.ToString();
        holomoneyText.text = "" + data.holomoney;

        unitInfo.SaveAll();
        holomoney.SavePlayerData(data);
    }

    public void REST()
    {
        if (uni == null) return;

        if (uni.dicton.myartifact != null)
        {
            artifacts.BackReset(false);
        }
        else
        {
            artifacts.BackReset(true);
        }
    }

    public void Up(Unit unit)
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        if (unitInfo.LevelSS >= 3)
        {
            Stars[1].SetActive(true);
        }

        if (unitInfo.LevelSS >= 2)
        {
            Stars[0].SetActive(true);
        }
    }

    public void HUP()
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        int Ned = (unitInfo.HpUp + 1) * 500;

        if (unitInfo.HpUp >= 5)
        {
            return;
        }

        if (Ned > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            int nM = Ned - data.holomoney;
            enoughText.text = nM.ToString();
            return;
        }

        data.holomoney -= Ned;
        holomoneyText.text = "" + data.holomoney;

        unitInfo.HpUp += 1;

        h = orih + (orih * baseIncreasePercentage * unitInfo.HpUp);
        hpText.text = FormatValue(h);

        updaover(unitInfo);
        mstat(unitInfo);
        unitInfo.SaveAll();
        holomoney.SavePlayerData(data);
    }

    public void PUP()
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        int Ned = (unitInfo.PowerUp + 1) * 500;

        if (unitInfo.PowerUp >= 5)
        {
            return;
        }

        if (Ned > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            int nM = Ned - data.holomoney;
            enoughText.text = nM.ToString();
            return;
        }

        data.holomoney -= Ned;
        holomoneyText.text = "" + data.holomoney;

        unitInfo.PowerUp += 1;

        p = orip + (orip * baseIncreasePercentage * unitInfo.PowerUp);
        powerText.text = FormatValue(p);

        updaover(unitInfo);
        mstat(unitInfo);
        unitInfo.SaveAll();
        holomoney.SavePlayerData(data);
    }

    public void HinUP()
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        int Ned = (unitInfo.HpInUp + 1) * 500;

        if (unitInfo.HpInUp >= 5)
        {
            return;
        }

        if (Ned > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            int nM = Ned - data.holomoney;
            enoughText.text = nM.ToString();
            return;
        }

        data.holomoney -= Ned;
        holomoneyText.text = "" + data.holomoney;

        unitInfo.HpInUp += 1;

        hi = orihi + (orihi * IncreasePercentage * unitInfo.HpInUp);
        hpinText.text = FormatValue(hi);

        updaover(unitInfo);
        mstat(unitInfo);
        unitInfo.SaveAll();
        holomoney.SavePlayerData(data);
    }

    public void PPAP()
    {
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        int Ned = (unitInfo.PowerInUp + 1) * 500;

        if (unitInfo.PowerInUp >= 5)
        {
            return;
        }

        if (Ned > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            int nM = Ned - data.holomoney;
            enoughText.text = nM.ToString();
            return;
        }

        data.holomoney -= Ned;
        holomoneyText.text = "" + data.holomoney;

        unitInfo.PowerInUp += 1;

        pi = oripi + (oripi * IncreasePercentage * unitInfo.PowerInUp);
        powerinText.text = FormatValue(pi);

        updaover(unitInfo);
        mstat(unitInfo);
        unitInfo.SaveAll();
        holomoney.SavePlayerData(data);
    }
}
