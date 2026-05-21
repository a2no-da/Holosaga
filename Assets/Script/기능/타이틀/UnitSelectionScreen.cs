using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionScreen : MonoBehaviour
{
    public static UnitSelectionScreen Instance { get; private set; }
    public GameObject[] unitSlots;
    public int selectedUnitCount = 0;
    public List<string> selectedUnitIds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            this.enabled = false;
        }
    }

    public List<string> GetSelectedUnitIds()
    {
        List<string> selectedUnitIds = new List<string>();

        foreach (var slot in unitSlots)
        {
            var sel = slot.GetComponent<Sel>();
            if (sel != null && sel.IsUnitDropped)
            {
                selectedUnitIds.Add(sel.UnitId);
            }
        }

        return selectedUnitIds;
    }

     public bool IsAllUnitsSelected()
    {
        selectedUnitCount = 0;
        selectedUnitIds = new List<string>();
        bool anySelected = false;

        foreach (var slot in unitSlots)
        {
            if (!slot.activeInHierarchy)
            {
                continue;
            }

            var sel = slot.GetComponent<Sel>();
            if (sel != null && sel.IsUnitDropped)
            {
                selectedUnitIds.Add(sel.UnitId);
                anySelected = true;

                selectedUnitCount++;
            }
        }
        return anySelected;
    }
}