using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.EventSystems;

public class ArtiProfile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public Button myB;
    public GameObject buyU;
    public Arti arti;
    public int i;
    private int price;
    public Image my;
    public Image DG;
    public Sprite[] spr;
    private Grade grade;
    private Sprite spri;
    private Artifact artifac;
    public TMP_Text YaText;

    public void SetArtifact(Artifact artifact)
    {
        string key;
        string meg;
        my.GetComponent<Image>().sprite = artifact.sprit;
        i = artifact.index - 1;
        switch (artifact.grade)
        {
            case Grade.Normal:
                key = "Artifact-common";
                meg = "c";
                DG.GetComponent<Image>().sprite = spr[0];
                break;
            case Grade.Epic:
                key = "Artifact-rare";
                meg = "r";
                DG.GetComponent<Image>().sprite = spr[1];
                break;
            case Grade.Unique:
                key = "Artifact-unique";
                meg = "u";
                DG.GetComponent<Image>().sprite = spr[2];
                break;
            default:
                key = null;
                meg = null;
                break;
        }

        grade = artifact.grade;
        spri = artifact.sprit;
        artifac = artifact;

        var nameLocalizedString = new LocalizedString { TableReference = "Item_artifact", TableEntryReference = "name_" + meg + "_p" + artifact.index };
        nameLocalizedString.StringChanged += (string localizedText) =>
        {
            nameText.text = localizedText;
        };

        priceText.text = artifact.Price.ToString();

        var localizedString = new LocalizedString { TableReference = "UI", TableEntryReference = key };

        localizedString.StringChanged += (string localizedText) =>
        {
            descriptionText.text = localizedText;
        };

        //YaText.gameObject.SetActive(false);

        YaText.gameObject.SetActive(true);
        YaText.text = artifact.Quantity.ToString();

        price = artifact.Price;
    }

    public void quna()
    {
        YaText.gameObject.SetActive(true);
        YaText.text = artifac.Quantity.ToString();
    }

    public void Set(Arti artii)
    {
        arti = artii;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void UT()
    {
        arti.UPText(i, grade, spri, artifac);
    }

    public void Exit()
    {
        arti.NoneText();
    }

    public void BUTT()
    {
        arti.index = i;
        arti.selAti = artifac;
        arti.artiProfile = this;
        arti.Buyy();
    }
}
