using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop_Unit : MonoBehaviour
{
    public Holomoney holomoney;
    public List<Dicton> allUnits = new List<Dicton>();
    public GameObject buyU;
    public GameObject unitDisplayPrefab;
    public Transform unitDisplayParent;
    public GameObject NoMoney;
    public TMP_Text NeedText;
    public TMP_Text holomoneyText;
    public int index;
    private List<GameObject> unitDisplayPool = new List<GameObject>(); 
    public int poolSize = 10;
    public TMP_Text enoughText;

    void Start()
    {
        InitializePool();
        DisplayAll();
    }

    void InitializePool()
    {
        int index = 0;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject display = Instantiate(unitDisplayPrefab, unitDisplayParent);
            UnitProfile unitProfile = display.GetComponent<UnitProfile>();
            unitProfile.i = index++;
            unitProfile.Set(this);
            display.SetActive(false); 
            unitDisplayPool.Add(display);
        }
    }

    void ResetPool()
    {
        foreach (GameObject display in unitDisplayPool)
        {
            display.SetActive(false);
        }
    }

    public void DisplayAll()
    {
        ResetPool();

        int displayIndex = 0;

        foreach (var unit in allUnits)
        {
            if (displayIndex >= unitDisplayPool.Count) break;

            GameObject display = unitDisplayPool[displayIndex];
            display.SetActive(true); 

            UnitProfile unitProfile = display.GetComponent<UnitProfile>();
            if (unitProfile != null)
            {
                unitProfile.SetUnitInfo(unit);
                unitProfile.SetBuyU(buyU);
            }
            displayIndex++;
        }
    }

    public void DisplayAttacker()
    {
        DisplayUnitsByRole(DictonRole.Attacker);
    }

    public void DisplayProjectiler()
    {
        DisplayUnitsByRole(DictonRole.Projectiler);
    }

    public void DisplaySupporter()
    {
        DisplayUnitsByRole(DictonRole.Supporter);
    }

    public void DisplayUnitsByRole(DictonRole role)
    {
        ResetPool();

        List<Dicton> filteredUnits = allUnits.FindAll(unit => unit.role == role);

        foreach (var unit in filteredUnits)
        {
            int originalIndex = allUnits.IndexOf(unit);
            if (originalIndex >= unitDisplayPool.Count) break;

            GameObject display = unitDisplayPool[originalIndex];
            display.SetActive(true);

            UnitProfile unitProfile = display.GetComponent<UnitProfile>();
            if (unitProfile != null)
            {
                unitProfile.SetUnitInfo(unit);
                unitProfile.SetBuyU(buyU);
            }
        }
    }

    public void BuyUnit(int i)
    {
        i = index; 
        PlayerData data = holomoney.LoadPlayerData();

        if (data == null)
        {
            return;
        }

        if (data.holomoney < allUnits[i].MyPrice)
        {
            buyU.gameObject.SetActive(false);
            NoMoney.gameObject.SetActive(true);
            int N = allUnits[i].MyPrice - data.holomoney;
            //enoughText.text = N.ToString();
            return;
        }

        data.holomoney -= allUnits[i].MyPrice;
        holomoney.SavePlayerData(data);
        holomoneyText.text = "" + data.holomoney;
        allUnits[i].isPurchasable = false;

        UnitProfile[] unitProfiles = unitDisplayParent.GetComponentsInChildren<UnitProfile>();
        foreach (var unitProfile in unitProfiles)
        {
            if (unitProfile.i == i)
            {
                unitProfile.DeactivateButton();
            }
        }
        buyU.gameObject.SetActive(false);
        allUnits[i].SaveAll();
    }
}
