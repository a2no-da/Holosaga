using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using UnityEngine.Audio;

public class Shop : MonoBehaviour
{
    public Holomoney holomoney;
    public TMP_Text holomoneyText;
    public GameObject Gacha;
    public GameObject YesBuy;
    public GameObject NoMoney;

    public Button[] towerButtons;
    public Dicton[] towerInfos;
    public GameObject[] units;

    public Dicton[] towerInfos_Old;
    public Dicton[] towerInfos_New;

    private System.Random rand;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI name2Text;
    public Image expFillImage;
    public Vector3 originalPosition;
    private GameObject currentAnimation;
    public GameObject[] SkinsAnimation;
    public TextMeshProUGUI[] NeedHoloMo;
    public Butt[] butt;
    public TMP[] Text;
    public Canvas canvas;
    public Image[] canvasin;
    private int index;
    private int i;
    public TextMeshProUGUI[] SkinsName;
    public Vector3[] SkinPosition;
    public ScrollRect scrollRect;
    public AudioClip gacha;
    public AudioSource gachas;

    [System.Serializable]
    public class Butt
    {
        public Button YesButton;
        public Button BuyButton;    
    }

    [System.Serializable]
    public class TMP
    {
        public TMP_Text ingText;
        public TMP_Text notingText;
    }

    void Start()
    {
        rand = new System.Random();
        PlayerData data = holomoney.LoadPlayerData();
        if (data != null)
        {
            holomoneyText.text = "" + data.holomoney;
        }
        else
        {
            holomoneyText.text = "0";
        }

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

    public void GachaGacha()
    {
        PlayerData data = holomoney.LoadPlayerData();
        if (data == null)
        {
            return;
        }

        if (data.holomoney < 100)
        {
            NoMoney.gameObject.SetActive(true);
            return;
        }

        data.holomoney -= 100;
        holomoney.SavePlayerData(data);
        holomoneyText.text = "" + data.holomoney;

        Gacha.gameObject.SetActive(true);
        gachas.PlayOneShot(gacha);

        Dicton pickedTower;

        if (rand.Next(0, 100) < 10)
        {
            var randomIndex = rand.Next(0, towerInfos_New.Length);
            pickedTower = towerInfos_New[randomIndex];
        }
        else
        {
            var randomIndex = rand.Next(0, towerInfos_Old.Length);
            pickedTower = towerInfos_Old[randomIndex];
        }
    
        if (pickedTower.isPurchasable)
        {
            pickedTower.isPurchasable = false;
        }
        else
        {
            pickedTower.MYexp += 100;
        }

        nameText.text = pickedTower.Name;
        expFillImage.fillAmount = (float)pickedTower.MYexp / pickedTower.exp;

        if (currentAnimation != null)
        {
            Destroy(currentAnimation);
        }

        currentAnimation = Instantiate(pickedTower.prefab, originalPosition, Quaternion.identity, canvas.transform);
    }

    public void Givememoney()
    {
        PlayerData data = holomoney.LoadPlayerData();
        data.holomoney += 1000;
        data.ANormal += 1000;
        data.AEpic += 1000;
        data.AUnique += 1000;
        holomoney.SavePlayerData(data);
        holomoneyText.text = "" + data.holomoney;
    }

    public void zeromemoney()
    {
        PlayerData data = holomoney.LoadPlayerData();
        data.holomoney = 0;
        holomoney.SavePlayerData(data);
        holomoneyText.text = "" + data.holomoney;
    }

    public void EndGcha()
    {
        Gacha.gameObject.SetActive(false);
        Destroy(currentAnimation);
    }

    public void ChangeC(Unit unit)
    {
        index = Array.IndexOf(units, unit.gameObject);
        Dicton unitInfo = towerInfos[index];

        for (int i = 0; i < butt.Length; i++)
        {
            butt[i].YesButton.gameObject.SetActive(false);
            butt[i].BuyButton.gameObject.SetActive(false);
            NeedHoloMo[i].text = "";
            SkinsName[i].text = "";
            Text[i].ingText.gameObject.SetActive(false);
            Text[i].notingText.gameObject.SetActive(false);
            if (SkinsAnimation[i] != null)
            {
                Destroy(SkinsAnimation[i]);
            }
        }

        for (int i = 0; i < unitInfo.SkinList.Length; i++)
        {
            var skin = unitInfo.SkinList[i];
            SkinsName[i].text = skin.skinName;

            if (!skin.isUnlocked)
            {
                if (skin.isselected)
                {
                    butt[i].YesButton.gameObject.SetActive(true);
                    butt[i].YesButton.interactable = false;
                    Text[i].ingText.gameObject.SetActive(false);
                    Text[i].notingText.gameObject.SetActive(true);
                }
                else
                {
                    butt[i].YesButton.gameObject.SetActive(true);
                    butt[i].YesButton.interactable = true;
                    Text[i].ingText.gameObject.SetActive(true);
                    Text[i].notingText.gameObject.SetActive(false);
                }
            }
            else
            {
                NeedHoloMo[i].text = "HOLOMONEY " + skin.price.ToString();
                butt[i].BuyButton.gameObject.SetActive(true);
            }

            SkinsAnimation[i] = Instantiate(skin.skinPrefab, SkinPosition[i], Quaternion.identity, canvasin[i % canvasin.Length].transform);
        }

        name2Text.text = unitInfo.Name;
    }

    public void ScrollRight()
    {
        float scrollAmount = 1f;
        float newPosition = scrollRect.horizontalNormalizedPosition + scrollAmount;
        if (newPosition > 1) newPosition = 1;
        scrollRect.horizontalNormalizedPosition = newPosition;
    }

    public void ScrollLeft()
    {
        float scrollAmount = 1f;
        float newPosition = scrollRect.horizontalNormalizedPosition - scrollAmount;
        if (newPosition < 0) newPosition = 0;
        scrollRect.horizontalNormalizedPosition = newPosition;
    }

    public void BuySkin(int dex)
    {
        i = dex;
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();

        if (unitInfo.SkinList[i].price > data.holomoney)
        {
            NoMoney.gameObject.SetActive(true);
            return;
        }

        YesBuy.gameObject.SetActive(true);
    }

    public void BuyYesSkin()
    {      
        Dicton unitInfo = towerInfos[index];
        PlayerData data = holomoney.LoadPlayerData();
        if (unitInfo.SkinList[i].price > data.holomoney)
        {
            return;
        }

        data.holomoney -= unitInfo.SkinList[i].price;
        unitInfo.SkinList[i].isUnlocked = false;
        unitInfo.SaveAll();
        holomoney.SavePlayerData(data);
        YesBuy.gameObject.SetActive(false);
        UpdateButtonStates();
        holomoneyText.text = data.holomoney.ToString();
    }

    private void UpdateButtonStates()
    {
        butt[i].YesButton.interactable = true;
        butt[i].BuyButton.gameObject.SetActive(false);
        butt[i].YesButton.gameObject.SetActive(true);
        NeedHoloMo[i].text = "";
        Text[i].ingText.gameObject.SetActive(true);
        Text[i].notingText.gameObject.SetActive(false);
    }

    public void SelectSkin(int dex)
    {
        i = dex;
        Dicton unitInfo = towerInfos[index];
        string selectedSkinName = unitInfo.SkinList[i].skinName;
        unitInfo.SelectSkin(selectedSkinName);
        unitInfo.prefab = unitInfo.SkinList[i].skinPrefab;
        butt[i].YesButton.interactable = false;
        Text[i].ingText.gameObject.SetActive(false);
        Text[i].notingText.gameObject.SetActive(true);

        for (int j = 0; j < unitInfo.SkinList.Length; j++)
        {
            if (j != i) 
            {
                butt[j].YesButton.interactable = true; 
                Text[j].ingText.gameObject.SetActive(true);
                Text[j].notingText.gameObject.SetActive(false);
            }
        }
    }
}