using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shopunlock : MonoBehaviour
{
    public Unit[] buttons;
    public Dicton[] towerInfos;
    public Sprite purchasableIcon;

    [System.Serializable]
    public class Unit
    {
        public Button Tower;
    }

    void Start()
    {
        UpdateButtonStates();
    }

    void Update()
    {

    }

    void UpdateButtonStates()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < towerInfos.Length)
            {
                var towerInfo = towerInfos[i];
                UpdateButtonState(buttons[i].Tower, towerInfo.isPurchasable);
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
