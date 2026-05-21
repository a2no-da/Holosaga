using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;
using UnityEngine.Localization;

public enum DictonRole
{
    Attacker,
    Projectiler,
    Supporter
}

[CreateAssetMenu(menuName = "설명")]

public class Dicton : ScriptableObject
{
    public string id;
    public string Name;
    public GameObject prefab;
    public GameObject originprefab;
    public int Power;
    public int Hp;
    public string unlock;
    public bool isPurchasable = false;
    public bool isfurea = false;
    public bool isop = false;
    public int exp;
    public int Holomo = 200;
    public int HolomoL = 50;
    public int MYexp;
    public int LevelSS = 1;
    public int Level = 1;
    public int MyPrice;

    public int HpUp;
    public int PowerUp;
    public int HpInUp;
    public int PowerInUp;

    public string descriptionKey;
    public SkinData[] SkinList;
    public PatternData[] PatternList;
    public DictonRole role;
    public string rolePlay;

    public Artifact myartifact;
    public string myArtifactId;

    [System.Serializable]
    public class SkinData
    {
        public string skinName;
        public string skinKey;
        public GameObject skinPrefab;
        public string skin;
        public bool isUnlocked; 
        public int price;
        public bool isselected;
    }

    [System.Serializable]
    public class PatternData
    {
        public int num;
        public string pattern;
    }

    [TextArea]
    public string sul;

    void Start()
    {
        LoadLocalizedText(descriptionKey);
        if (isfurea)
        {
            isPurchasable = true;
        }

        if(isop)
        {
            isPurchasable = false;
        }

        switch (role)
        {
            case DictonRole.Attacker:
                rolePlay = "��������";
                break;
            case DictonRole.Projectiler:
                rolePlay = "����ü";
                break;
            case DictonRole.Supporter:
                rolePlay = "������";
                break;
        }
    }

    public void SaveAll()
    {
        DictonData data = new DictonData();
        data.id = this.id;
        if (!isfurea || !isop)
        {
            data.isPurchasable = this.isPurchasable;
        }
        else
        {
            data.isPurchasable = false;
        }

        data.exp = this.exp;
        data.Holomo = this.Holomo;
        data.HolomoL = this.HolomoL;
        data.MYexp = this.MYexp;
        data.LevelSS = this.LevelSS;
        data.HpUp = this.HpUp;
        data.PowerUp = this.PowerUp;
        data.HpInUp = this.HpInUp;
        data.PowerInUp = this.PowerInUp;
        data.myartifact = this.myartifact;
        data.myArtifactId = this.myArtifactId;

        data.SkinList = new SkinData[this.SkinList.Length];

        if (data.Holomo < 200)
        {
            data.Holomo = 200;
        }
        if (data.HolomoL < 50)
        {
            data.HolomoL = 50;
        }
        if (data.LevelSS < 1)
        {
            data.LevelSS = 1;
        }

        for (int i = 0; i < this.SkinList.Length; i++)
        {
            data.SkinList[i] = new SkinData();
            data.SkinList[i].skinName = this.SkinList[i].skinName;
            data.SkinList[i].isUnlocked = this.SkinList[i].isUnlocked;
            data.SkinList[i].isselected = this.SkinList[i].isselected;
        }

        DictonManagers dictonManagers = FindObjectOfType<DictonManagers>();
        dictonManagers.SaveDictonData(data);
    }

    public void LoadLocalizedText(string key)
    {
        var localizedString = new LocalizedString { TableReference = "Item_unit_description", TableEntryReference = key };

        localizedString.StringChanged += (string localizedText) =>
        {
            sul = localizedText;
        };

        var localizedItemName = new LocalizedString { TableReference = "Item_unit_name", TableEntryReference = key };

        localizedItemName.StringChanged += (string localizedText) =>
        {
            Name = localizedText;
        };

        var localizedUnlockCondition = new LocalizedString { TableReference = "Item_unit_unlockCondition", TableEntryReference = key };

        localizedUnlockCondition.StringChanged += (string localizedText) =>
        {
            unlock = localizedText;
        };

        foreach (var skin in SkinList)
        {
            var localizedSkinName = new LocalizedString { TableReference = "Item_skin_name", TableEntryReference = skin.skinKey };

            localizedSkinName.StringChanged += (string localizedText) =>
            {
                skin.skinName = localizedText;
            };
        }
    }

    public void MLoadLocalizedText(string key)
    {

        var localizedString = new LocalizedString { TableReference = "Item_monster_description", TableEntryReference = key };

        localizedString.StringChanged += (string localizedText) =>
        {
            sul = localizedText;
        };

        var localizedItemName = new LocalizedString { TableReference = "Item_monster_name", TableEntryReference = key };

        localizedItemName.StringChanged += (string localizedText) =>
        {
            Name = localizedText;
        };

        var localizedUnlockCondition = new LocalizedString { TableReference = "Item_monster_unlockCondition", TableEntryReference = key };

        localizedUnlockCondition.StringChanged += (string localizedText) =>
        {
            unlock = localizedText;
        };
    }

    public void LoadAll()
    {
        DictonManagers dictonManagers = FindObjectOfType<DictonManagers>();
        DictonDataCollection dataCollection = dictonManagers.LoadDictonData();
        if (dataCollection != null && dataCollection.dictonDatas.Count > 0)
        {
            DictonData data = dataCollection.dictonDatas.FirstOrDefault(d => d.id == id); 

            if (data != null)
            {
                if (!isfurea)
                {
                    this.isPurchasable = data.isPurchasable;
                }
                else
                {
                    this.isPurchasable = true;
                }

                if(isop)
                {
                    this.isPurchasable = false;
                }
                this.exp = data.exp;
                this.Holomo = data.Holomo;
                this.HolomoL = data.HolomoL;
                this.MYexp = data.MYexp;
                this.LevelSS = data.LevelSS;
                this.Level = data.Level;
                this.HpUp = data.HpUp;
                this.PowerUp = data.PowerUp;
                this.HpInUp = data.HpInUp;
                this.PowerInUp = data.PowerInUp;
                this.myartifact = data.myartifact;
                this.myArtifactId = data.myArtifactId;

                if (data.SkinList != null && this.SkinList != null && data.SkinList.Length <= this.SkinList.Length)
                {
                    for (int i = 0; i < data.SkinList.Length; i++)
                    {
                        this.SkinList[i].skinName = data.SkinList[i].skinName;
                        this.SkinList[i].isUnlocked = data.SkinList[i].isUnlocked;
                        this.SkinList[i].isselected = data.SkinList[i].isselected;

                        if (this.SkinList[i].isselected)
                        {
                            this.prefab = this.SkinList[i].skinPrefab;
                            continue; 
                        }
                    }
                }
            }
        }
    }

    public void Initialize()
    {
        Holomo = 200;
        HolomoL = 50;
        MYexp = 0;
        LevelSS = 1;
        HpUp = 0;
        PowerUp = 0;
        HpInUp = 0;
        PowerInUp = 0;

        if (SkinList.Length > 0)
        {
            foreach (var skin in SkinList)
            {
                skin.isUnlocked = true;
                skin.isselected = false;

                SkinList[0].isselected = true;
                SkinList[0].isUnlocked = false;
            }
        }

        SaveAll();
    }

    public void InitializeM()
    {
        Holomo = 0;
        HolomoL = 0;
        MYexp = 0;
        LevelSS = 0;
        isPurchasable = true;
    }

    public void SelectSkin(string selectedSkinName)
    {
        foreach (var skin in SkinList)
        {
            if (skin.skinName == selectedSkinName)
            {
                skin.isselected = true;
            }
            else
            {
                skin.isselected = false;
            }
        }
        SaveAll();
    }

    public string GetSelectedSkinName()
    {
        foreach (var skin in SkinList)
        {
            if (skin.isselected)
            {
                return skin.skin;
            }
        }
        return null; 
    }
}
