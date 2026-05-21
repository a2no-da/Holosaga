using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public string UnitId;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector3 originalPosition;
    private bool droppedOnTarget;
    private Vector3 initialPosition;
    private int siblingIndex;
    public bool isUnlocked = false;
    public GameObject unlockPanel;
    public Dicton towerInfo;
    public Sprite purchasableIcon;
    public GameObject unit;
    public Image[] starImages;
    public GameObject Spanel;
    public Sel currentSel;
    private int originalOrderInLayer; 
    public int dragOrderInLayer = 91;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.position;
        siblingIndex = transform.GetSiblingIndex();
        originalOrderInLayer = canvas.sortingOrder; 
    }

    void Start()
    { 
        if(towerInfo != null)
        {
            initialPosition = rectTransform.position;
            UpdateState(towerInfo.isPurchasable, towerInfo.LevelSS);
            unit = towerInfo.prefab;
        }
    }

    void UpdateState(bool isPurchasable, int LevelSS)
    {
        Transform existingIcon = this.gameObject.transform.Find("PurchasableIcon");
        if (isPurchasable)
        {
            if (existingIcon == null)
            {
                isUnlocked = false;
                GameObject icon = new GameObject("PurchasableIcon");
                Image iconImage = icon.AddComponent<Image>();
                iconImage.sprite = purchasableIcon;
                icon.transform.SetParent(this.gameObject.transform, false);

                iconImage.SetNativeSize();

                RectTransform rectTransform = icon.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
            }
            Spanel.gameObject.SetActive(false);
        }
        else
        {
            if (existingIcon != null)
            {
                Destroy(existingIcon.gameObject);
            }
        }

        foreach (var starImage in starImages)
        {
            starImage.gameObject.SetActive(true);
        }

        if (LevelSS == 1)
        {
            for (int i = 1; i < starImages.Length; i++)
            {
                starImages[i].gameObject.SetActive(false);
            }
        }
        else if (LevelSS == 2)
        {
            for (int i = 2; i < starImages.Length; i++)
            {
                starImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            return;
        }
        droppedOnTarget = false;
        originalPosition = rectTransform.position;
        if (towerInfo.descriptionKey == "Miko")
        {
            originalPosition = new Vector3(-3.33f, -3.97f, 90f);
        }

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();

        if (currentSel != null)
        {
            currentSel.ClearDroppedObject();
            currentSel = null;
        }
        canvas.sortingOrder = dragOrderInLayer;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isUnlocked) 
        {
            return;
        }
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!isUnlocked)
        {
            rectTransform.position = initialPosition;
            return;
        }

        if (!droppedOnTarget)
        {
            rectTransform.position = initialPosition;
            if (towerInfo.descriptionKey == "Miko")
            {
                rectTransform.position = new Vector3(-3.33f, -3.97f, 90f);
            }
        }
        else
        {
            if (currentSel != null)
            {
                rectTransform.position = currentSel.transform.position;
            }
        }

        if (currentSel != null)
        {
            rectTransform.position = currentSel.transform.position;
        }

        transform.SetSiblingIndex(siblingIndex);
        canvas.sortingOrder = originalOrderInLayer;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    public void SetDroppedOnTarget(bool isTarget)
    {
        droppedOnTarget = isTarget;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
