using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Localization;

public class SkinProfile : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public GameObject purchasableIndicator;
    public Transform prefabParent;
    public GameObject NSel;
    public GameObject Sel;
    public Button BuyB;
    public Button BuyedB;
    public TMP_Text BBText;
    public TMP_Text BSText;
    public GameObject buyS;
    public Shop_Skin shop_Skin;
    public Dicton.SkinData data;
    public string myK;
    public int i;
    public string id;
    private int price;
    private GameObject instantiatedPrefab;
    public string rolePlay;
    public string skinKey;

    public void SetSkinInfo(Dicton.SkinData skin)
    {
        var nameKey = skin.skinKey;
        var nameLocalizedString = new LocalizedString { TableReference = "Item_skin_name", TableEntryReference = nameKey };
        nameLocalizedString.StringChanged += (string localizedText) =>
        {
            nameText.text = localizedText;
        };
        data = skin;
        myK = skin.skinKey;
        priceText.text = skin.price.ToString();
        price = skin.price;
        BuyB.gameObject.SetActive(skin.isUnlocked);
        BuyedB.gameObject.SetActive(!skin.isUnlocked);

        if (skin.isselected)
        {
            BBText.gameObject.SetActive(false);
            BSText.gameObject.SetActive(true);
            BuyedB.GetComponent<Button>().interactable = false; 
        }
        else
        {
            BBText.gameObject.SetActive(true);
            BSText.gameObject.SetActive(false);
            BuyedB.GetComponent<Button>().interactable = true; 
        }

        if (instantiatedPrefab != null)
        {
            instantiatedPrefab.SetActive(false);
        }

        if (instantiatedPrefab == null || instantiatedPrefab.name != skin.skinPrefab.name)
        {
            instantiatedPrefab = Instantiate(skin.skinPrefab, prefabParent);
            instantiatedPrefab.transform.localPosition = new Vector3(0f, -185, 0);
            instantiatedPrefab.name = skin.skinPrefab.name;
        }
        else
        {
            instantiatedPrefab.SetActive(true);
        }
    }

    public void Sbuy()
    {
        shop_Skin.SetSelectedSkin(data, myK);
        shop_Skin.index = i;
        shop_Skin.MK = myK;

        buyS.SetActive(true);
        shop_Skin.NeedText.text = "HOLOMONEY " + price.ToString();
    }

    public void SetBuyS(GameObject buySObject)
    {
        buyS = buySObject;
    }

    public void Set(Shop_Skin shop)
    {
        shop_Skin = shop;
    }

    public void SelectSkin()
    {
        Debug.Log("¿¡¾Æ");
        shop_Skin.UpdateSkinSelection(data.skinKey, id);
    }

    public void DeactivateButton()
    {
        BuyedB.interactable = true;
        BBText.gameObject.SetActive(true);
        BSText.gameObject.SetActive(false);
    }

    public void Select()
    {
        BuyedB.interactable = false;
        BBText.gameObject.SetActive(false);
        BSText.gameObject.SetActive(true);
    }
}
