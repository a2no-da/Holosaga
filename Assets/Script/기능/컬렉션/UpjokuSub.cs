using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class UpjokuSub : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI subText;
    public GameObject DoJang;

    public void upho(Achievement achievement)
    {
        int i = achievement.num; 

        var nameLocalizedString = new LocalizedString { TableReference = "Item_Achievement", TableEntryReference = "name_ach_p" + i };
        nameLocalizedString.StringChanged += (string localizedText) =>
        {
            nameText.text = localizedText;
        };

        var localizedString = new LocalizedString { TableReference = "Item_Achievement", TableEntryReference = "desc_ach_p" + i };

        localizedString.StringChanged += (string localizedText) =>
        {
            subText.text = localizedText;
        };

        if(achievement.unlocked)
        {
            DoJang.SetActive(true);
        }
        else
        {
            DoJang.SetActive(false);
        }
    }
}
