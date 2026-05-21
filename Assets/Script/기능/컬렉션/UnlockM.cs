using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockM : MonoBehaviour
{
    public Units[] but;
    public Dicton[] towerInfos;
    public Dicton[] monsterInfos;
    public Sprite purchasableIcon;
    public bool ye;
    [System.Serializable]
    public class Units
    {
        public Button Towers;
        public Button Monsters;
    }

    void Start()
    {
        UpdateButtonT();
    }

    void Update()
    {

    }

    public void UpdateButtonT()
    {
        for (int i = 0; i < but.Length; i++)
        {
            if (i < towerInfos.Length)
            {
                var towerInfo = towerInfos[i];
                UpdateButtonState(but[i].Towers, towerInfo.isPurchasable);
            }
        }
    }

    public void UpdateButtonM()
    {
        for (int i = 0; i < but.Length; i++)
        {
            if (i < monsterInfos.Length)
            {
                var monsterInfo = monsterInfos[i];
                UpdateButtonState(but[i].Monsters, monsterInfo.isPurchasable);
            }
        }
    }


    void UpdateButtonState(Button button, bool isPurchasable)
    {
        button.interactable = !isPurchasable;

        Transform existingIcon = button.transform.Find("PurchasableIcon");
        if (isPurchasable)
        {
            if (existingIcon == null) 
            {
                GameObject icon = new GameObject("PurchasableIcon");
                Image iconImage = icon.AddComponent<Image>();
                iconImage.sprite = purchasableIcon;
                icon.transform.SetParent(button.transform, false);

                iconImage.SetNativeSize();

                RectTransform rectTransform = icon.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;

                if (ye)
                {
                    rectTransform.sizeDelta = new Vector2(210, 215);
                }
            }
        }
        else
        {
            if (existingIcon != null)
            {
                Destroy(existingIcon.gameObject); 
            }
        }
    }
}