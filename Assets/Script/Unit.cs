using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System;
using System.Reflection;
using TowerPattern;
using TowerCooltime;
using TowerExPattern;
using System.Globalization;

public enum UnitType
{
    Enemy,
    Tower
}

namespace TowerPattern
{
    public partial class Data
    {
        public int ID;
        public string dscName;
        public float dmgCoe;
        public int hitLim;
        public float speed;
        public int trigCount;
        public float trigProb;
        public int stunTime;
        public float slowTime;
        public float slowPower;
        public float addForce;
        public string P_user;
    }
}

namespace TowerCooltime
{
    public partial class Data
    {
        public int ID;
        public string dscName;
        public int IgnRange;
        public int IsIndep;
        public float initCool;
        public float cool;
        public string P_user;
    }
}

namespace TowerExPattern
{
    public partial class Data
    {
        public int ID;
        public string dscName;
        public float dmgCoe;
        public int hitLim;
        public float speed;
        public int trigCount;
        public float trigProb;
        public int stunTime;
        public float slowTime;
        public float slowPower;
        public float addForce;
        public string P_user;
    }
}

public class Unit : MonoBehaviour
{
    public int i;
    public UnitType unitType;
    public Dicton dicton;
    public AbilityCard[] abilityCards;
    public float Health; 
    public float MaxHealth;
    public float Power;
    public float attackCooldown;
    public float Pattern_cooltime;
    public float ActiveCooldown;
    public float Active_cooltime;
    public float inActive_cooltime;
    public float Critical;
    public float RangeX;
    public float RangeY;
    public float centerX;
    public float centerY;

    public bool ising;
    public bool isStart = true;
    public bool isDead = false;
    public bool isStunned = false;
    public bool Laplus;
    public bool isEx;
    public bool LTimer;
    protected GameObject targetEnemy;

    public GameObject hpBarPrefab;
    public GameObject hpCanvas;

    public Image HealthBarImage;
    public Transform HealthBarPosition;
    public GameObject HealthBarInstance;

    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] AnimClip; 
    protected string CurrentAnimation;
    public GameObject damageTextPrefab;
    private Color originalColor;
    public Coroutine stunCoroutine;
    public AnimationHandler Handler;
    public GameObject deathvfxPrefab;
    public float powerIncreasePerLevel;
    public float healthIncreasePerLevel;
    public float expIncreasePerLevel = 0.69f;
    public float initialHealth;
    public float initialPower;
    public int initialEXP = 100;
    public int H_regen;
    public int respawnTime;
    public float respawnTimeIncreasePerLevel;
    public int atiCount = 0;

    public string dscName;
    public TowerPattern.Data[] myP;
    public TowerCooltime.Data[] myC;
    public TowerExPattern.Data[] myEP;

    private static List<Dictionary<string, string>> towerData;
    public static List<Dictionary<string, string>> monsterStat;

    public InPlusStat inPlusStat;
    public bool Stop = false;
    public float ArtHPoint = 0;

    [System.Serializable]
    public class InPlusStat
    {
        public float IpHp;
        public float IpPower;
        public float IpSpeed;
        public float Imcool;
        public float cardcool;
    }

    public virtual void Start()
    {
        if (!LTimer)
        {
            i -= 1;

            Stat();

            if (isStart)
            {
                hpCanvas = GameObject.Find("Hp");
                if (hpCanvas != null)
                {
                    HealthBarInstance = Instantiate(hpBarPrefab, hpCanvas.transform);
                    HealthBarImage = HealthBarInstance.transform.Find("Image/HPBar").GetComponent<Image>();
                    HealthBarImage.fillAmount = Health / MaxHealth;
                }
            }
        }
        originalColor = Color.white;
        abilityCards = new AbilityCard[2];
    }


    public virtual void Stat()
    {
        towerData = CsvData.Read("TowerStat");

        var data = towerData[i];
        dscName = data["Desc_name"];
        centerX = float.Parse(data["CenterPos"].Split(',')[0], CultureInfo.InvariantCulture);
        centerY = float.Parse(data["CenterPos"].Split(',')[1], CultureInfo.InvariantCulture);
        RangeX = float.Parse(data["Range"].Split(',')[0], CultureInfo.InvariantCulture);
        RangeY = float.Parse(data["Range"].Split(',')[1], CultureInfo.InvariantCulture);
        MaxHealth = float.Parse(data["Hp"], CultureInfo.InvariantCulture);
        Health = MaxHealth;
        Power = int.Parse(data["Power"]);
        healthIncreasePerLevel = float.Parse(data["Hp_inc"], CultureInfo.InvariantCulture);
        powerIncreasePerLevel = float.Parse(data["Power_inc"], CultureInfo.InvariantCulture);
        initialEXP = int.Parse(data["Exp"]);
        expIncreasePerLevel = float.Parse(data["Exp_inc"], CultureInfo.InvariantCulture);
        respawnTime = int.Parse(data["Respawn_time"]);
        respawnTimeIncreasePerLevel = float.Parse(data["Respawn_inc"], CultureInfo.InvariantCulture);
        H_regen = int.Parse(data["H_regen"]);

        var towerPatternData = CsvData.Read("TowerPattern");
        var towerCooltimeData = CsvData.Read("TowerCooltime");

        myP = new TowerPattern.Data[20];
        myC = new TowerCooltime.Data[20];

        int Pindex = 1;

        foreach (var val in towerPatternData)
        {
            if (dscName.Equals(val["Pattern_user"]))
            {
                myP[Pindex] = new TowerPattern.Data
                {
                    ID = int.Parse(val["ID"]),
                    dscName = val["Desc_name"],
                    dmgCoe = float.Parse(val["Damage_CoE"], CultureInfo.InvariantCulture),
                    hitLim = int.Parse(val["HitLimit"]),
                    speed = float.Parse(val["Speed"], CultureInfo.InvariantCulture),
                    trigCount = int.Parse(val["Trig_Count"]),
                    trigProb = float.Parse(val["Trig_Prob"], CultureInfo.InvariantCulture),
                    stunTime = int.Parse(val["Stun_time"]),
                    slowTime = float.Parse(val["Slow_time"], CultureInfo.InvariantCulture),
                    slowPower = float.Parse(val["Slow_power"], CultureInfo.InvariantCulture),
                    addForce = float.Parse(val["AddedForce"], CultureInfo.InvariantCulture),
                    P_user = val["Pattern_user"]
                };
                Pindex++;
            }
        }

        int Cindex = 1;

        foreach (var val in towerCooltimeData)
        {
            if (dscName.Equals(val["Pattern_user"]))
            {
                myC[Cindex] = new TowerCooltime.Data
                {
                    ID = int.Parse(val["ID"]),
                    dscName = val["Desc_name"],
                    IgnRange = int.Parse(val["IgnoreRange"]),
                    IsIndep = int.Parse(val["IsIndependent"]),
                    cool = float.Parse(val["Cooltime"], CultureInfo.InvariantCulture),
                    P_user = val["Pattern_user"]
                };
                Cindex++;
            }
        }

        if (myC[1] != null)
        {
            Pattern_cooltime = myC[1].cool;
        }
    }

    public virtual void Update()
    {
        if (!LTimer)
        {
            if (HealthBarImage != null)
            {
                HealthBarImage.fillAmount = Health / MaxHealth;

                if (Health <= 0)
                {
                    if (unitType != UnitType.Tower)
                    {
                    }
                }
                else
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(HealthBarPosition.position);
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hpCanvas.GetComponent<RectTransform>(),
                    screenPos,
                    Camera.main, out localPoint))
                    {
                        RectTransform instanceRect = HealthBarInstance.GetComponent<RectTransform>();
                        instanceRect.anchoredPosition = localPoint;
                    }
                }
            }

            if (!Stop)
            {
                if (isStunned)
                {
                    if (skeletonAnimation != null)
                    {
                        var skeleton = skeletonAnimation.Skeleton;
                        if (skeleton != null)
                        {
                            skeleton.A = 0.5f;
                        }
                    }
                }
                else
                {
                    if (skeletonAnimation != null)
                    {
                        var skeleton = skeletonAnimation.Skeleton;
                        if (skeleton != null)
                        {
                            skeleton.A = 1f;
                        }
                    }
                }
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (Health >= damage)
        {
            Health -= damage;
        }
        else
        {
            Health = 0;
        }
        StartCoroutine(ChangeColorOverTime());
        ShowDamageText(damage);

        if (unitType == UnitType.Tower)
        {
            if (Health <= 0)
            {
                Die();
            }
        }
    }

    public void ShowDamageText(float damage)
    {
        GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        damageText.GetComponent<DamageText>().SetDamage((int)Math.Round(damage));
    }

    public virtual void Heal(float amount, Tower tower)
    {
        if (unitType != UnitType.Tower)
        {
            tower = null;
        }

        if (!LTimer)
        {
            if (amount >= 0)
            {
                Health += amount;

                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }
            }
        }


        if (unitType == UnitType.Tower)
        {
            if (dicton != null)
            {
                if (dicton.myartifact != null)
                {
                    if (dicton.myartifact.artifactId == "au8")
                    {
                        ArtHPoint = amount * 0.2f;
                        dicton.myartifact.artfunction.FunctionAti(this);
                    }
                }
            }
        }
    }

    protected virtual void Die()
    {
        if (!Laplus)
        {
            if (!LTimer)
            {
                GameObject deathEffect = Instantiate(deathvfxPrefab, transform.position, Quaternion.identity);
                if (unitType != UnitType.Tower)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    protected void SetAnimation(int trackIndex, AnimationReferenceAsset animation, bool loop)
    {
        // 현재 재생하려는 애니메이션이 이미 재생 중인 애니메이션과 같다면 애니메이션 변경을 하지 않고 메서드를 종료
        if (animation.name.Equals(CurrentAnimation))
            return;
        // trackIndex는 애니메이션 트랙을 지정하며, animation.name은 재생할 애니메이션의 이름
        skeletonAnimation.AnimationState.SetAnimation(trackIndex, animation.name, loop);
        // 다음 번 애니메이션 재생 시에 현재 재생하려는 애니메이션이 이미 재생 중인 애니메이션인지 확인
        CurrentAnimation = animation.name;
    }

    public IEnumerator ChangeColorOverTime()
    {
        float duration = 0.2f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            skeletonAnimation.Skeleton.SetColor(Color.Lerp(Color.red, originalColor, normalizedTime));
            yield return null;
        }

        skeletonAnimation.Skeleton.SetColor(originalColor);
    }

    public virtual void Stun(float stunDuration)
    {
        if (Laplus) { return; }
        if (LTimer) { return; }
        isStunned = true;

        if (Handler == null)
        {
            Handler = hpBarPrefab.GetComponent<AnimationHandler>();
        }

        if (!hpBarPrefab.activeInHierarchy)
        {
            Handler.position.SetActive(true);
        }

        if (Handler.stunCoroutine != null)
        {
            StopCoroutine(Handler.stunCoroutine);
        }

        Handler.stunCoroutine = StartCoroutine(Handler.StunAnimationSequence(stunDuration));

        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunDuration(stunDuration));
    }

    public virtual IEnumerator StunDuration(float stunDuration)
    {
        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        stunCoroutine = null;
    }

    public virtual void Attack()
    {
        if(isStunned)
        {
            return;
        }

        if (unitType == UnitType.Tower && !isEx)
        {
            if (dicton.myartifact != null)
            {
                if (dicton.myartifact.mytype == MyType.Active_Attack)
                {
                    dicton.myartifact.artfunction.FunctionAti(this);
                }
            }

            if (abilityCards[0] != null)
            {
                if(abilityCards[0].abilitytype != AbilityType.PlusActive)
                {
                    abilityCards[0].abilityFunction.FunctionPulsACard(this);
                }
            }

            if (abilityCards[1] != null)
            {
                if (abilityCards[1].abilitytype != AbilityType.PlusActive)
                {
                    abilityCards[1].abilityFunction.FunctionPulsACard(this);
                }
            }
        }
    }

    public virtual void Attack2()
    {
        if (isStunned)
        {
            return;
        }
    }

    public virtual void Attack3()
    {
        if (isStunned)
        {
            return;
        }
    }
}
