using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using System;

public class Buff
{
    public GameObject target;
    public string buffName;
    public float startTime;
    public float duration;
    public float powerIncrease;
    public float cooldownReduction;
    public float animationSpeedIncrease;
    public float healthcrease;
    public GameObject textObject;
    public float actualCooldownReduction = 0;
    public int stack;
}

[System.Serializable]
public class AngelTimerInfo
{
    public GameObject tower; 
    public float endTime; 

    public AngelTimerInfo(GameObject tower, float duration)
    {
        this.tower = tower;
        this.endTime = Time.time + duration;
    }
}

public class BuffTextInfo
{
    public GameObject TextObject;
    public float StartTime;
    public Image ImageIcon;

    public BuffTextInfo(GameObject textObject, float startTime)
    {
        this.TextObject = textObject;
        this.StartTime = startTime;
        this.ImageIcon = textObject.GetComponentInChildren<Image>();
    }
}

public class BuffManager : MonoBehaviour
{
    public List<Buff> activeBuffs = new List<Buff>();
    public List<BuffTextInfo> activeTexts = new List<BuffTextInfo>();

    public static BuffManager Instance { get; private set; }

    public GameObject buffTextContainerPrefab;
    public GameObject powerIncreaseTextPrefab;
    public GameObject speedIncreaseTextPrefab;
    public GameObject healthIncreaseTextPrefab;
    public GameObject kaliStackTextPrefab;
    public GameObject katataStackTextPrefab; 
    public GameObject AngelPrefab;
    public GameObject buffTextPrefab;
    public GameObject HealPrefab;
    public int Kcount;
    private List<string> nonRemovableBuffs = new List<string> { "칼리", "카나타" };
    private List<AngelTimerInfo> angelTimers = new List<AngelTimerInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        float currentTime = Time.time;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            if (currentTime >= buff.startTime + buff.duration)
            {
                RemoveBuff(buff);
            }
        }

        float duration = 2f; 
        for (int i = activeTexts.Count - 1; i >= 0; i--)
        {
            BuffTextInfo info = activeTexts[i];
            float timePassed = Time.time - info.StartTime;

            if (timePassed >= duration)
            {
                Destroy(info.TextObject);
                activeTexts.RemoveAt(i);
            }
        }

        for (int i = angelTimers.Count - 1; i >= 0; i--)
        {
            AngelTimerInfo timerInfo = angelTimers[i];

            float remainingTime = timerInfo.endTime - Time.time;

            if (Time.time >= timerInfo.endTime)
            {
                if(timerInfo != null)
                {
                    Tower tower = timerInfo.tower.GetComponent<Tower>();
                    tower.isAngel = true;
                    UpdateAngelText(timerInfo.tower.transform.Find("박스").gameObject, "AngelText", true);
                    angelTimers.RemoveAt(i);
                }
                else
                {
                    return;
                }
            }
        }
    }

    public int GetActiveBuffCount(GameObject target)
    {
        return activeBuffs.Count(buff => buff.target == target &&
                                         buff.buffName != "칼리" &&
                                         buff.buffName != "카나타" &&
                                         buff.buffName != "엔젤");
    }

    public void ReapplyBuffsForTarget(GameObject target)
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.target == target && buff.buffName != "칼리" && buff.buffName != "카나타")
            {
                ApplyBuffEffects(buff, true);
            }
        }
    }

    public void ApplyBuff(GameObject target, string buffName, float duration, float powerIncrease, float speedIncrease, float healthIncrease)
    {
        Tower to = target.GetComponent<Tower>();
        if (to.isEx) { return; }

        Buff existingBuff = activeBuffs.Find(b => b.buffName == buffName && b.target == target);

        if (existingBuff != null)
        {
            Tower tower = existingBuff.target.GetComponent<Tower>();

            existingBuff.startTime = Time.time;
            existingBuff.duration = duration;

            if (powerIncrease != 0)
            {
                CreateBuffText(tower, buffTextPrefab, "power");
            }
            else if (speedIncrease != 0)
            {
                CreateBuffText(tower, buffTextPrefab, "speed");
            }
            else if (healthIncrease != 0)
            {
                CreateBuffText(tower, buffTextPrefab, "hp");
            }
            UpdateBuffText(target, null, null);
            return;
        }

        Buff newBuff = new Buff
        {
            target = target,
            buffName = buffName,
            startTime = Time.time,
            duration = duration,
            powerIncrease = powerIncrease,
            cooldownReduction = speedIncrease,
            animationSpeedIncrease = speedIncrease,
            healthcrease = healthIncrease
        };

        activeBuffs.Add(newBuff);
        ApplyBuffEffects(newBuff);
        UpdateBuffText(target, null, null);
    }

    public void ApplyKaliBuff(GameObject target, string buffName, int Pstack)
    {
        Buff kaliBuff = activeBuffs.Find(b => b.buffName == "칼리" && b.target == target);
        Tower tower = target.GetComponent<Tower>();

        if (kaliBuff != null)
        {
            int previousStack = kaliBuff.stack;
            kaliBuff.stack += Pstack;

            for (int i = previousStack / 10; i < kaliBuff.stack / 10; i++)
            {
                tower.Power += 4;
            }

            UpdateBuffText(target, kaliBuff, null); 
        }
        else
        {

            Buff newBuff = new Buff
            {
                target = target,
                buffName = "칼리",
                stack = 0,
                startTime = Time.time,
                duration = float.MaxValue,
                powerIncrease = 0
            };

            activeBuffs.Add(newBuff);
            UpdateBuffText(target, newBuff, null);
        }
    }

    public void UpdateKailBuff(GameObject target, string buffName)
    {
        Buff kaliBuff = activeBuffs.Find(b => b.buffName == "칼리" && b.target == target);
        Tower tower = target.GetComponent<Tower>();

        if (kaliBuff != null)
        {
            int stackCount = kaliBuff.stack / 10;
            for (int i = 0; i < stackCount; i++)
            {
                tower.Power += 4;
            }

            UpdateBuffText(target, kaliBuff, null);
        }
    }

    public void ApplyKanataBuff(GameObject target, string buffName, int Kstack)
    {
        Buff kanataBuff = activeBuffs.Find(b => b.buffName == "카나타" && b.target == target);
        Kanata kanata = target.GetComponent<Kanata>();

        if (kanataBuff != null)
        {
            int previousStack = kanataBuff.stack;

            if (Kstack == 0)
            {
                kanataBuff.stack = 0;
                Kcount = kanata.GC;
            }
            else
            {
                kanataBuff.stack += Kstack;
                Kcount = kanataBuff.stack;
                kanata.GC = Kcount;
            }

            UpdateBuffText(target, null ,kanataBuff);
        }
        else
        {

            Buff newBuff = new Buff
            {
                target = target,
                buffName = "카나타",
                stack = 0,
                startTime = Time.time,
                duration = float.MaxValue,
            };

            activeBuffs.Add(newBuff);
            UpdateBuffText(target, null, newBuff);
        }
    }

    public void AngelBuff(GameObject target, string buffName)
    {
        Buff angelBuff = activeBuffs.Find(b => b.buffName == "엔젤" && b.target == target);
        Tower tower = target.GetComponent<Tower>();
        GameObject buffTextContainer = target.transform.Find("박스").gameObject;

        if (angelBuff != null)
        {
            if (!tower.isAngel)
            {
                Instantiate(HealPrefab, tower.transform.position, Quaternion.identity, tower.transform);
                GameObject towerGameObject = tower.gameObject;

                angelTimers.Add(new AngelTimerInfo(towerGameObject, 70f));
            }
            UpdateAngelText(buffTextContainer, "AngelText", tower.isAngel);
        }
        else
        {
            tower.isAngel = true;

            Buff newBuff = new Buff
            {
                target = target,
                buffName = "엔젤",
                startTime = Time.time,
                duration = float.MaxValue,
            };

            activeBuffs.Add(newBuff);
            UpdateAngelText(buffTextContainer, "AngelText", tower.isAngel);
        }
    }

    public void ApplyBuffEffects(Buff buff, bool isReapplying = false)
    {
        Tower tower = buff.target.GetComponent<Tower>();
        float healthRatio = (float)tower.Health / tower.MaxHealth;

        float totalPowerIncrease = 0;
        float totalCooldownReduction = 0;
        float totalHealthIncrease = 0;
        float mae;

        foreach (var activeBuff in activeBuffs.Where(b => b.target == buff.target))
        {
            totalPowerIncrease += activeBuff.powerIncrease;
            totalCooldownReduction += activeBuff.cooldownReduction;
            totalHealthIncrease += activeBuff.healthcrease;
        }

        if (isReapplying)
        {
            tower.Power = (int)(tower.initialPower + tower.initialPower * totalPowerIncrease);
            tower.MaxHealth = (int)(tower.initialHealth + Mathf.FloorToInt(tower.initialHealth * totalHealthIncrease));
            tower.Health = (int)((tower.MaxHealth) * healthRatio);

            /*tower.Pattern_cooltime = Mathf.Max(0, tower.originalPatternCooltime - (tower.originalPatternCooltime * totalCooldownReduction));
            tower.animationSpeed = tower.originalAnimationSpeed + totalCooldownReduction;
            if (tower.Pattern_cooltime != 0)
            {
                buff.actualCooldownReduction = tower.originalPatternCooltime * buff.cooldownReduction;
            }
            else
            {
                buff.actualCooldownReduction = 0;
            }*/

            //tower.myStatIn(false);
        }
        else
        {
            tower.Power = (int)(tower.initialPower + tower.initialPower * totalPowerIncrease);

            if (buff.cooldownReduction != 0)
            {
                tower.attackCooldown = 0;
            }

            mae = tower.Pattern_cooltime;
            tower.Pattern_cooltime = Mathf.Max(0, tower.originalPatternCooltime - (tower.originalPatternCooltime * totalCooldownReduction));
            tower.animationSpeed = tower.originalAnimationSpeed + totalCooldownReduction;

            if (tower.Pattern_cooltime != 0)
            {
                buff.actualCooldownReduction = tower.originalPatternCooltime * buff.cooldownReduction;
            }
            else
            {
                buff.actualCooldownReduction = mae;
            }

            float initialMaxHealth = tower.MaxHealth;
            tower.MaxHealth = Mathf.FloorToInt(tower.initialHealth + (int)(tower.initialHealth * totalHealthIncrease));
            tower.Health = (int)((tower.MaxHealth) * healthRatio);

            if (buff.powerIncrease != 0)
            {
                CreateBuffText(tower, buffTextPrefab, "power");
            }
            else if (buff.cooldownReduction != 0)
            {
                CreateBuffText(tower, buffTextPrefab, "speed");
            }
            else if (buff.healthcrease != 0)
            {
                CreateBuffText(tower, buffTextPrefab, "hp");
            }

            tower.myStatIn(true);
            tower.KAAKA();
        }
    }

    private bool IsFunctionCard(Buff buff)
    {
        return buff.buffName.Contains("기능카드");
    }

    public void RemoveBuff(Buff buff)
    {
        if (buff == null)
        {
            return;
        }

        if (IsFunctionCard(buff))
        {
            return;
        }

        if (nonRemovableBuffs.Contains(buff.buffName))
        {
            return;
        }

        if (buff.target == null)
        {
            return;
        }

        Tower tower = buff.target.GetComponent<Tower>();
        if (tower == null)
        {
            return;
        }
        float healthRatio = (float)tower.Health / tower.MaxHealth;

        float totalPowerIncrease = 0;
        float totalCooldownReduction = 0;
        float totalHealthIncrease = 0;
        activeBuffs.Remove(buff);
        UpdateBuffText(buff.target);

        foreach (var activeBuff in activeBuffs.Where(b => b.target == buff.target))
        {
            totalPowerIncrease += activeBuff.powerIncrease;
            totalCooldownReduction += activeBuff.cooldownReduction;
            totalHealthIncrease += activeBuff.healthcrease;
        }

        tower.Power = (int)(tower.initialPower + tower.initialPower * totalPowerIncrease);
        tower.Pattern_cooltime += buff.actualCooldownReduction;
        tower.animationSpeed -= buff.cooldownReduction;


        float initialMaxHealth = tower.MaxHealth;
        float healthDecreaseValue = tower.initialHealth * buff.healthcrease;

        int healthDecreaseInt = (healthDecreaseValue % 1 >= 0.6f) ? Mathf.RoundToInt(healthDecreaseValue) : Mathf.FloorToInt(healthDecreaseValue);
        float ot = totalHealthIncrease - buff.healthcrease;

        if (ot <= 0)
        {
            tower.MaxHealth = tower.initialHealth;
        }
        else
        {
            tower.MaxHealth = (tower.MaxHealth - healthDecreaseInt) - (30 * tower.inPlusStat.IpHp);
        }

        int newHealth = (int)(tower.Health - (initialMaxHealth - tower.MaxHealth) * healthRatio);

        tower.Health = newHealth <= 0 ? 1 : newHealth;


        tower.myStatIn(false);
        tower.KAAKA();

        if (buff.textObject != null)
        {
            Destroy(buff.textObject);
        }
    }

    private void EnsureBuffTextContainerExists(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (target.transform.Find("박스") == null)
        {
            if (buffTextContainerPrefab == null)
            {
                return;
            }

            GameObject container = Instantiate(buffTextContainerPrefab, target.transform);
            container.name = "박스";
            container.transform.localPosition = new Vector3(1.6f, 0.83f, 0);
        }
    }

    public void UpdateBuffText(GameObject target, Buff kaliBuff = null, Buff kanataBuff = null)
    {
        Tower tower = target.GetComponent<Tower>();

        EnsureBuffTextContainerExists(target);
        GameObject buffTextContainer = target.transform.Find("박스").gameObject;

        float totalPowerIncrease = activeBuffs.Where(b => b.target == target && b.buffName != "칼리" && b.buffName != "카나타").Sum(b => b.powerIncrease);
        float totalCooldownReduction = activeBuffs.Where(b => b.target == target && b.buffName != "칼리" && b.buffName != "카나타").Sum(b => b.cooldownReduction);
        float totalHealthReduction = activeBuffs.Where(b => b.target == target && b.buffName != "칼리" && b.buffName != "카나타").Sum(b => b.healthcrease);

        UpdateOrCreateText(buffTextContainer, "PowerIncreaseText", totalPowerIncrease);
        UpdateOrCreateText(buffTextContainer, "SpeedIncreaseText", totalCooldownReduction);
        UpdateOrCreateText(buffTextContainer, "HealthIncreaseText", totalHealthReduction);

        if (kaliBuff != null)
        {
            UpdateOrCreateTextK(buffTextContainer, "KaliStackText", kaliBuff.stack);
        }

        if (kanataBuff != null)
        {
            UpdateOrCreateTextG(buffTextContainer, "katataStackText", kanataBuff.stack);
        }

        bool isSpecialTarget = tower.dscName == "Kanata" || tower.dscName == "Kaliope";
    }

    private void UpdateOrCreateText(GameObject container, string textName, float value)
    {
        TextMeshProUGUI textComponent = container.transform.Find(textName)?.GetComponent<TextMeshProUGUI>();

        if (value != 0)
        {
            if (textComponent == null)
            {
                GameObject textObj = Instantiate(GetPrefabByName(textName), container.transform);
                textObj.name = textName;
                textComponent = textObj.GetComponent<TextMeshProUGUI>();
            }
            float increasePercentage = (int)Math.Round(value * 100);
            textComponent.text = $"{increasePercentage}%";

            textComponent.gameObject.SetActive(true);
        }
        else if (textComponent != null)
        {
            textComponent.gameObject.SetActive(false);
        }
    }

    private void UpdateOrCreateTextK(GameObject container, string textName, float value)
    {
        TextMeshProUGUI textComponent = container.transform.Find(textName)?.GetComponent<TextMeshProUGUI>();

        if (value >= 0)
        {
            if (textComponent == null)
            {
                GameObject textObj = Instantiate(GetPrefabByName(textName), container.transform);
                textObj.name = textName;
                textComponent = textObj.GetComponent<TextMeshProUGUI>();
            }

            if (textName == "KaliStackText")
            {
                textComponent.text = $"{value}";
            }

            textComponent.gameObject.SetActive(true);
        }
        else if (textComponent != null)
        {
            textComponent.gameObject.SetActive(false);
        }
    }

    private void UpdateOrCreateTextG(GameObject container, string textName, float value)
    {
        TextMeshProUGUI textComponent = container.transform.Find(textName)?.GetComponent<TextMeshProUGUI>();

        if (value >= 0)
        {
            if (textComponent == null)
            {
                GameObject textObj = Instantiate(GetPrefabByName(textName), container.transform);
                textObj.name = textName;
                textComponent = textObj.GetComponent<TextMeshProUGUI>();
            }

            if (textName == "katataStackText")
            {
                textComponent.text = $"{value}";
            }

            textComponent.gameObject.SetActive(true);
        }
        else if (textComponent != null)
        {
            textComponent.gameObject.SetActive(false);
        }
    }

    private void UpdateAngelText(GameObject container, string textName, bool isok)
    {
        TextMeshProUGUI textComponent = container.transform.Find(textName)?.GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            GameObject textObj = Instantiate(GetPrefabByName(textName), container.transform);
            textObj.name = textName;
            textComponent = textObj.GetComponent<TextMeshProUGUI>();
        }

        if (isok)
        {
            textComponent.gameObject.SetActive(true);
        }
        else
        {
            textComponent.gameObject.SetActive(false);
        }
    }

    private GameObject GetPrefabByName(string name)
    {
        switch (name)
        {
            case "PowerIncreaseText":
                return powerIncreaseTextPrefab;
            case "SpeedIncreaseText":
                return speedIncreaseTextPrefab;
            case "HealthIncreaseText":
                return healthIncreaseTextPrefab;
            case "KaliStackText": 
                return kaliStackTextPrefab;
            case "katataStackText":
                return katataStackTextPrefab;
            case "AngelText":
                return AngelPrefab;
            default:
                return null;
        }
    }

    public void CreateBuffText(Tower tower, GameObject buffTextPrefab, string animationName)
    {
        GameObject textObj = Instantiate(buffTextPrefab, tower.transform.position + new Vector3(0, 0, 0), Quaternion.identity, tower.transform);
        SkeletonAnimation skeletonAnimation = textObj.GetComponent<SkeletonAnimation>();

        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.SetAnimation(0, animationName, false);
        }

        //activeTexts.Add(new BuffTextInfo(textObj, Time.time));
    }
}