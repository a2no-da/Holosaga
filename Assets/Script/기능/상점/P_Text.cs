using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using UnityEngine.Localization;

public class P_Text : MonoBehaviour
{
    public string Key;

    public Image type;
    public Sprite[] sprite;
    public TextMeshProUGUI needText;
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI snameText;
    public TextMeshProUGUI pnameText;
    public TextMeshProUGUI upText;
    public TextMeshProUGUI pText;
    public GameObject unlock;
    public GameObject unlock3;
    public Dicton towerinfo;
    public TextMeshProUGUI[] dangyeText;

    private int i = 1;
    private int j = 0;

    public void LoadLocalizedText(int i)
    {
        Key = towerinfo.descriptionKey;
        var localizedString = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = Key + "_name_p" + i };

        localizedString.StringChanged += (string localizedText) =>
        {
            snameText.text = localizedText;
        };

        var localizedItemPN = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = "Pattern_" + towerinfo.PatternList[i - 1].pattern };

        localizedItemPN.StringChanged += (string localizedText) =>
        {
            pnameText.text = localizedText;
        };

        var localizedItemUp = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = "Upgrade_" + towerinfo.PatternList[i - 1].num.ToString() };

        localizedItemUp.StringChanged += (string localizedText) =>
        {
            upText.text = localizedText;
        };

        var localizedItemPS = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = Key + "_desc_p" + i };

        localizedItemPS.StringChanged += (string localizedText) =>
        {
            pText.text = localizedText;
        };

        type.gameObject.SetActive(true);
        string pattern = towerinfo.PatternList[i - 1].pattern;
        switch (pattern)
        {
            case "default":
                type.sprite = sprite[0]; 
                break;
            case "passive":
                type.sprite = sprite[1]; 
                break;
            case "active":
                type.sprite = sprite[2]; 
                break;
            default:
                Debug.LogWarning("알 수 없는 패턴: " + pattern);
                break;
        }

        FT();
        unlock.SetActive(false);
        unlock3.SetActive(true);

        if (towerinfo.LevelSS < 2 && towerinfo.PatternList[i - 1].num == 2)
        {
            unlock.SetActive(true);

            var localizedItemDG = new LocalizedString { TableReference = "UI", TableEntryReference = "Shop-unlockAt2"};

            localizedItemDG.StringChanged += (string localizedText) =>
            {
                dangyeText[0].text = localizedText;
                dangyeText[1].text = localizedText;
            };
        }

        if (towerinfo.LevelSS < 3 && towerinfo.PatternList[i - 1].num == 3)
        {
            unlock.SetActive(true);

            var localizedItemDG = new LocalizedString { TableReference = "UI", TableEntryReference = "Shop-unlockAt3"};

            localizedItemDG.StringChanged += (string localizedText) =>
            {
                dangyeText[0].text = localizedText;
                dangyeText[1].text = localizedText;
            };
        }

        if (towerinfo.LevelSS > 2)
        {
            unlock3.SetActive(false);
        }

        j = i;
    }

    public void UpG()
    {
        Key = towerinfo.descriptionKey;
        var localizedString = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = Key + "_name_p" + j };

        localizedString.StringChanged += (string localizedText) =>
        {
            snameText.text = localizedText;
        };

        var localizedItemPN = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = "Pattern_" + towerinfo.PatternList[j - 1].pattern };

        localizedItemPN.StringChanged += (string localizedText) =>
        {
            pnameText.text = localizedText;
        };

        var localizedItemUp = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = "Upgrade_" + towerinfo.PatternList[j - 1].num.ToString() };

        localizedItemUp.StringChanged += (string localizedText) =>
        {
            upText.text = localizedText;
        };

        var localizedItemPS = new LocalizedString { TableReference = "Item_unit_pattern", TableEntryReference = Key + "_desc_p" + j };

        localizedItemPS.StringChanged += (string localizedText) =>
        {
            pText.text = localizedText;
        };

        unlock.SetActive(false);
        unlock3.SetActive(true);

        if (towerinfo.LevelSS < 2 && towerinfo.PatternList[i - 1].num == 2)
        {
            unlock.SetActive(true);

            var localizedItemDG = new LocalizedString { TableReference = "UI", TableEntryReference = "Shop-unlockAt2" };

            localizedItemDG.StringChanged += (string localizedText) =>
            {
                dangyeText[0].text = localizedText;
                dangyeText[1].text = localizedText;
            };
        }

        if (towerinfo.LevelSS < 3 && towerinfo.PatternList[i - 1].num == 3)
        {
            unlock.SetActive(true);

            var localizedItemDG = new LocalizedString { TableReference = "UI", TableEntryReference = "Shop-unlockAt3" };

            localizedItemDG.StringChanged += (string localizedText) =>
            {
                dangyeText[0].text = localizedText;
                dangyeText[1].text = localizedText;
            };
        }

        if (towerinfo.LevelSS > 2)
        {
            unlock3.SetActive(false);
        }
    }

    public void plus()
    {
        if (i < towerinfo.PatternList.Length) 
        {
            i++;
            numberText.text = i.ToString() + " / " + towerinfo.PatternList.Length;
            LoadLocalizedText(i);
        }
    }

    public void FT()
    {
        if (i < towerinfo.PatternList.Length)
        {
            numberText.text = i.ToString() + " / " + towerinfo.PatternList.Length;
        }
    }

    public void minus()
    {
        if (i > 1)
        {
            i--;
            numberText.text = i.ToString() + " / " + towerinfo.PatternList.Length;
            LoadLocalizedText(i);
        }
    }

    public void Re()
    {
        snameText.text = "";
        pnameText.text = "";
        upText.text = "";
        pText.text = "";
        unlock.SetActive(false);
        unlock3.SetActive(true);
        type.gameObject.SetActive(false);
        type.sprite = sprite[0];

        i = 1;
        numberText.text = i.ToString() + " / 1";
    }
}
