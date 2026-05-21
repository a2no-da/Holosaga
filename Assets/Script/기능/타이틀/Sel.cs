using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sel : MonoBehaviour, IDropHandler
{
    public string UnitId { get; set; }
    public bool IsUnitDropped { get; private set; } = false;
    public GameObject droppedObject;
    public Transform panelTransform;
    public int index;
    private SelController selController;

    void Start()
    {
        selController = FindObjectOfType<SelController>(); 
    }

    void Update()
    {
        if (droppedObject == null)
        {
            ClearDroppedObject();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Drop drop = eventData.pointerDrag?.GetComponent<Drop>();
        if (drop != null && drop.isUnlocked && !selController.IsDuplicate(drop.UnitId))
        {
            SetDroppedObject(drop.unit, drop.UnitId);
            drop.currentSel = this;
        }
    }

    public void SetDroppedObject(GameObject unitPrefab, string unitId)
    {
        ClearDroppedObject();

        if (unitPrefab != null)
        {
            droppedObject = Instantiate(unitPrefab, panelTransform, false);
            IsUnitDropped = true;
            UnitId = unitId;
        }
    }

    public void ClearDroppedObject()
    {
        if (droppedObject != null)
        {
            Destroy(droppedObject);
            droppedObject = null;
        }
        IsUnitDropped = false;
        UnitId = null;
    }
} 

