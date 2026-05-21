using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.EventSystems;

public class UnitProfile : MonoBehaviour
{
    public TMP_Text nameText; 
    public TMP_Text descriptionText; 
    public TMP_Text priceText;
    public TMP_Text guideText;
    public GameObject purchasableIndicator;
    public GameObject guide;
    public Transform prefabParent;
    public GameObject NSel;
    public GameObject Sel;
    public Button myB;
    public GameObject buyU;
    public Shop_Unit shop_Unit;
    public int i;
    private int price;
    private GameObject instantiatedPrefab;

    public void SetUnitInfo(Dicton unit)
    {
        var nameKey = unit.descriptionKey; 
        var nameLocalizedString = new LocalizedString { TableReference = "Item_unit_name", TableEntryReference = nameKey };
        nameLocalizedString.StringChanged += (string localizedText) =>
        {
            nameText.text = localizedText;
        };

        var guideLocalizedString = new LocalizedString { TableReference = "Item_unit_semiDescription", TableEntryReference = nameKey };
        guideLocalizedString.StringChanged += (string localizedText) =>
        {
            guideText.text = localizedText;
        };

        //guideText.text = unit.MyPrice.ToString();
        guide.SetActive(false);
        string key;

        switch (unit.role)
        {
            case DictonRole.Attacker:
                key = "Unit-Role2";
                break;
            case DictonRole.Projectiler:
                key = "Unit-Role1";
                break;
            case DictonRole.Supporter:
                key = "Unit-Role3";
                break;
            default:
                key = null;
                break;
        }

        var localizedString = new LocalizedString { TableReference = "UI", TableEntryReference = key };

        localizedString.StringChanged += (string localizedText) =>
        {
            descriptionText.text = localizedText;
        };

        if (instantiatedPrefab != null)
        {
            instantiatedPrefab.SetActive(false);
        }

        if (instantiatedPrefab == null || null != unit.originprefab)
        {
            instantiatedPrefab = Instantiate(unit.originprefab, prefabParent, false);
            instantiatedPrefab.transform.localPosition = new Vector3(0, -185/*-115f*/, 0);
        }
        else
        {
            instantiatedPrefab.SetActive(true);
        }

        if (unit.isPurchasable == false) 
        {
            myB.interactable = false;
            NSel.SetActive(false);
            Sel.SetActive(true);
        }

        price = unit.MyPrice;
    }

    public void Ubuy()
    {
        buyU.SetActive(true);
        shop_Unit.index = i;
        shop_Unit.NeedText.text = "HOLOMONEY " + price.ToString();
    }

    public void SetBuyU(GameObject buyUObject)
    {
        buyU = buyUObject;      
    }

    public void Set(Shop_Unit shop)
    {
        shop_Unit = shop;
    }

    public void DeactivateButton()
    {
        myB.interactable = false;
        NSel.gameObject.SetActive(false);
        Sel.gameObject.SetActive(true);
    }

    public void ShowGuide()
    {
        guide.SetActive(true); 
    }

    public void HideGuide()
    {
        guide.SetActive(false);
    }
}