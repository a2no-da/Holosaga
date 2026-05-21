using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Localization;

public class Shop_Skin : MonoBehaviour
{
    public Holomoney holomoney;
    public List<Dicton> allUnits = new List<Dicton>();
    public GameObject buyS;
    public GameObject skinDisplayPrefab;
    public Transform skinDisplayParent;
    public GameObject NoMoney;
    public TMP_Text NeedText;
    public TMP_Text holomoneyText;
    public int index;
    public string MK;
    private List<GameObject> skinDisplayPool = new List<GameObject>();
    public int poolSize = 20;
    public string MYs;
    public TMP_Text enoughText;

    public Dicton.SkinData selectedSkin;
    public string Key;

    private void Start()
    {
        InitializeSkinDisplayPool();
        DisplaySkins();
    }

    private void InitializeSkinDisplayPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject skinDisplay = Instantiate(skinDisplayPrefab, skinDisplayParent);
            SkinProfile skinProfile = skinDisplay.GetComponent<SkinProfile>();
            skinDisplay.SetActive(false);
            skinProfile.Set(this);
            skinDisplayPool.Add(skinDisplay);
        }

        int displayIndex = 0;

        foreach (var unit in allUnits)
        {
            foreach (var skin in unit.SkinList)
            {
                if (displayIndex >= skinDisplayPool.Count)
                {
                    break;
                }

                var skinProfile = skinDisplayPool[displayIndex].GetComponent<SkinProfile>();
                if (skinProfile != null)
                {
                    var nameKey = unit.descriptionKey;
                    var nameLocalizedString = new LocalizedString { TableReference = "Item_unit_name", TableEntryReference = nameKey };
                    nameLocalizedString.StringChanged += (string localizedText) =>
                    {
                        skinProfile.descriptionText.text = localizedText;
                    };
                }
                displayIndex++;
            }
        }
    }

    private void ResetSkinDisplayPool()
    {
        foreach (GameObject skinDisplay in skinDisplayPool)
        {
            skinDisplay.SetActive(false);
            MYs = "";
        }
    }

    public void DisplaySkins()
    {
        ResetSkinDisplayPool();

        int displayIndex = 0;

        foreach (var unit in allUnits)
        {
            foreach (var skin in unit.SkinList)
            {
                if (displayIndex >= skinDisplayPool.Count)
                {
                    break;
                }

                if (!unit.isPurchasable)
                {
                    var skinProfile = skinDisplayPool[displayIndex].GetComponent<SkinProfile>();
                    if (skinProfile != null)
                    {
                        skinProfile.SetSkinInfo(skin);
                        skinProfile.i = displayIndex;
                        skinProfile.SetBuyS(buyS);
                        skinProfile.id = unit.id;
                        skinProfile.descriptionText.text = unit.Name;
                    }
                    skinDisplayPool[displayIndex].SetActive(true);
                }
                displayIndex++;
            }
        }
    }

    public void ShowSkins(string dictonid)
    {
        ResetSkinDisplayPool();

        Dicton targetUnit = allUnits.Find(unit => unit.id == dictonid);

        if (targetUnit != null)
        {
            for (int i = 0; i < targetUnit.SkinList.Length; i++)
            {
                if (i >= skinDisplayPool.Count) break;

                GameObject skinDisplay = skinDisplayPool[i];
                skinDisplay.SetActive(true);

                SkinProfile skinProfile = skinDisplay.GetComponent<SkinProfile>();
                if (skinProfile != null)
                {
                    skinProfile.SetSkinInfo(targetUnit.SkinList[i]);
                    skinProfile.shop_Skin = this;
                    skinProfile.descriptionText.text = targetUnit.Name;
                    skinProfile.id = targetUnit.id;
                }
            }
        }
        MYs = dictonid;
    }

    public void SetSelectedSkin(Dicton.SkinData skin, string id)
    {
        selectedSkin = skin;
        UpdateSkinSelection(selectedSkin.skinKey, id);
    }

    public void BuySkin(int i)
    {
        i = index;
        PlayerData data = holomoney.LoadPlayerData();

        if (data == null)
        {
            return;
        }

        if (selectedSkin == null)
        {
            return;
        }

        if (data.holomoney < selectedSkin.price)
        {
            buyS.gameObject.SetActive(false);
            NoMoney.gameObject.SetActive(true);
            int N = selectedSkin.price - data.holomoney;
            //enoughText.text = N.ToString();
            return;
        }

        data.holomoney -= selectedSkin.price;
        holomoney.SavePlayerData(data);
        holomoneyText.text = data.holomoney.ToString();

        foreach (var unit in allUnits)
        {
            foreach (var skin in unit.SkinList)
            {
                if (skin.skinKey == selectedSkin.skinKey)
                {
                    skin.isUnlocked = false;
                    GameObject skinDisplay = skinDisplayPool.FirstOrDefault(sd => sd.GetComponent<SkinProfile>().skinKey == skin.skinKey);
                    if (skinDisplay != null)
                    {
                        skinDisplay.GetComponent<SkinProfile>().DeactivateButton();
                    }
                    unit.SaveAll();
                    break;
                }
            }
        }

        if (MYs != "")
        {
            ShowSkins(MYs);
        }
        else
        {
            DisplaySkins();
        }

        buyS.gameObject.SetActive(false);
    }

    public void UpdateSkinSelection(string selectedSkinKey, string ID)
    {
        foreach (var unit in allUnits)
        {
            if (unit.id == ID)
            {
                foreach (var skin in unit.SkinList)
                {
                    if (skin.skinKey == selectedSkinKey)
                    {
                        skin.isselected = true;
                        GameObject skinDisplay = skinDisplayPool.FirstOrDefault(sd => sd.GetComponent<SkinProfile>().skinKey == skin.skinKey);
                        if (skinDisplay != null)
                        {
                            skinDisplay.GetComponent<SkinProfile>().Select();
                        }
                    }
                    else
                    {
                        skin.isselected = false;
                        GameObject skinDisplay = skinDisplayPool.FirstOrDefault(sd => sd.GetComponent<SkinProfile>().skinKey == skin.skinKey);
                        if (skinDisplay != null)
                        {
                            skinDisplay.GetComponent<SkinProfile>().DeactivateButton();
                        }
                    }
                }
            }
        }

        if (MYs != "")
        {
            ShowSkins(MYs);
        }
        else
        {
            DisplaySkins();
        }
    }
}
