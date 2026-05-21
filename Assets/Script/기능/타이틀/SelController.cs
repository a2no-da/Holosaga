using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelController : MonoBehaviour
{
    public List<Sel> selObjects = new List<Sel>();
    public GameObject[] Job;

    private void Start()
    {
        for (int i = 0; i < selObjects.Count; i++)
        {
            selObjects[i].index = i;
        }
    }

    public bool IsDuplicate(string unitId)
    {
        foreach (var sel in selObjects)
        {
            if (sel.UnitId == unitId) 
            {
                return true; 
            }
        }
        return false; 
    }
}
