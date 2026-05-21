using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using MonsterPattern;
using MonsterCooltime;
using System.Globalization;

public class Zom : Tower
{
    public Tower tower;
    public AudioSource audioSource;
    public GameObject P2effectPrefab;
    public int mN;
    public float moveSpeed;
    public float endPointX;
    public MonsterPattern.Data[] mP;
    public MonsterCooltime.Data[] mC;

    public override void Start()
    {
        isEx = true;
        base.Start();

        Stat();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (tower != null)
        {
            if (mN == 2)
            {
                Power = (int)(tower.Power * 0.5f);
                MaxHealth = (int)(tower.Health);
            }
            else
            {
                Power = (int)(tower.Power * 0.2f);
                MaxHealth = (int)(tower.Health * 0.1f);
            }
        }
        else
        {
            Power = 10;
            MaxHealth = 50;
            Health = MaxHealth;
        }

        SP();
        H_regen = 0;
        Health = MaxHealth;
        StartCoroutine(FadeInCoroutine(skeletonAnimation, 0.5f));
        HealthBarInstance.SetActive(false);
    }

    public void SP()
    {
        float localY = transform.localPosition.y;

        if (Mathf.Approximately(localY, 0.8600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(localY, -1.16f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(localY, -3.18f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(localY, -5.2f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    public override void Stat()
    {
        if (monsterStat == null)
        {
            monsterStat = CsvData.Read("MonsterStat");
        }
        var data = monsterStat[i];

        dscName = data["Desc_name"];
        centerX = float.Parse(data["CenterPos"].Split(',')[0], CultureInfo.InvariantCulture);
        centerY = float.Parse(data["CenterPos"].Split(',')[1], CultureInfo.InvariantCulture);
        RangeX = float.Parse(data["Range"].Split(',')[0], CultureInfo.InvariantCulture);
        RangeY = float.Parse(data["Range"].Split(',')[1], CultureInfo.InvariantCulture);
        moveSpeed = float.Parse(data["moveSpeed"], CultureInfo.InvariantCulture);
        MaxHealth = float.Parse(data["Hp"], CultureInfo.InvariantCulture);
        Power = int.Parse(data["Power"]);

        var monsterPatternData = CsvData.Read("MonsterPattern");
        var monsterCooltimeData = CsvData.Read("MonsterCooltime");

        mP = new MonsterPattern.Data[20];
        mC = new MonsterCooltime.Data[20];


        int Pindex = 1;

        foreach (var val in monsterPatternData)
        {
            if (dscName.Equals(val["Pattern_user"]))
            {
                mP[Pindex] = new MonsterPattern.Data
                {
                    ID = int.Parse(val["ID"]),
                    dscName = val["Desc_name"],
                    dmgCoe = float.Parse(val["Damage_CoE"], CultureInfo.InvariantCulture),
                    hitLim = int.Parse(val["HitLimit"]),
                    speed = float.Parse(val["Speed"], CultureInfo.InvariantCulture),
                    trigCount = int.Parse(val["Trig_Count"]),
                    trigProb = float.Parse(val["Trig_Prob"], CultureInfo.InvariantCulture),
                    stunTime = int.Parse(val["Stun_time"]),
                    P_user = val["Pattern_user"]
                };
                Pindex++;
            }
        }

        int Cindex = 1;

        foreach (var val in monsterCooltimeData)
        {
            if (dscName.Equals(val["Pattern_user"]))
            {
                mC[Cindex] = new MonsterCooltime.Data
                {
                    ID = int.Parse(val["ID"]),
                    dscName = val["Desc_name"],
                    IgnRange = int.Parse(val["IgnoreRange"]),
                    IsIndep = int.Parse(val["IsIndependent"]),
                    initCool = float.Parse(val["Cooltime_init"], CultureInfo.InvariantCulture),
                    cool = float.Parse(val["Cooltime"], CultureInfo.InvariantCulture),
                    P_user = val["Pattern_user"]
                };
                Cindex++;
            }
        }

        if (!Laplus)
        {
            Pattern_cooltime = mC[1].cool;
            attackCooldown = mC[1].initCool;
        }
    }

    public override void Update()
    {
        base.Update();
        if (targetEnemy == null)
        {
            MoveTowardsEndPoint();
        }

        if (endPointX < 0)
        {
            if (transform.position.x <= endPointX)
            {
                Destroy(HealthBarInstance);

                HealthBarImage = null;
                HealthBarInstance = null;
                Destroy(gameObject);
            }
        }
        else
        {
            if (transform.position.x >= endPointX)
            {
                Destroy(HealthBarInstance);

                HealthBarImage = null;
                HealthBarInstance = null;
                Destroy(gameObject);
            }
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                AttackEnemiesInRange(Power);
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }

    public override void Attack()
    {
        SetAnimation(0, AnimClip[1], false);
    }

    private void AttackEnemiesInRange(float power)
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = enemiesInRange[i];
            if (enemyObject == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }
            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyObject == null || enemyScript == null) continue;
            {
                enemyScript.UpdateAttackingTower(this);
                enemyScript.TakeDamage(power);
            }
        }
    }

    public void MoveTowardsEndPoint()
    {
        if (!ising && !isStunned)
        {
            Vector3 direction = Vector3.right;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            SetAnimation(0, AnimClip[2], true);
        }
    }

    protected override void Die()
    {
        base.Die();
        Destroy(HealthBarInstance);

        HealthBarImage = null;
        HealthBarInstance = null;
        Destroy(gameObject);
    }

    private IEnumerator FadeInCoroutine(SkeletonAnimation animation, float duration)
    {
        float startAlpha = 0f;
        float endAlpha = 1f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, currentTime / duration);

            foreach (var slot in animation.Skeleton.Slots)
            {
                slot.A = alpha;
            }

            yield return null;
        }

        foreach (var slot in animation.Skeleton.Slots)
        {
            slot.A = endAlpha;
        }
    }
}