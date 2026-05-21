using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Kanata : Tower
{
    public int GC;
    public GameObject oneEffect;
    public GameObject threeEffect;

    public GameObject syuriken;
    public GameObject syurikeni;
    private bool Stopu;

    public override void Start()
    {
        base.Start();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 2)
        {
            myActive = true;
            this.ApplyBuff("카나타");
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2_cooltime = myC[2].cool;
            P2T.text = myC[2].cool.ToString();
        }
        syurikeni.SetActive(false);
        Stopu = false;

        Act();
    }

    public override void Update()
    {
        base.Update();

        if (P2Cooldown > 0 && !isStunned && !syurikeni.activeSelf && LevelS > 1)
        {
            P2Cooldown -= Time.deltaTime;
            P2T.text = ((int)P2Cooldown).ToString();
        }

        if (P2Cooldown <= 0 && !isStunned && !syurikeni.activeSelf && LevelS > 1)
        {
            Stopu = true;
            syurikeni.SetActive(true);
            P2Cooldown = P2_cooltime;
            P2T.text = ((int)P2Cooldown).ToString();
        }
        else if (isStunned)
        {
            ising = false;
            P2Cooldown = P2_cooltime;
        }

        if(!Stopu && syurikeni.activeSelf)
        {
            syurikeni.SetActive(false);
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_p1":
                AttackEnemiesInRange(Power * myP[1].dmgCoe);
                if (oneEffect != null)
                {
                    Instantiate(oneEffect, transform.position, transform.rotation);
                }

                syurikeni.SetActive(false);

                if (LevelS > 1 && Stopu)
                {
                    GameObject currentSyuriken = Instantiate(syuriken, transform.position, transform.rotation);
                    currentSyuriken.GetComponent<shuriken>().Initialize(this, Power * myP[2].dmgCoe);
                }
                Stopu = false;
                break;
            case "damage_p3":
                float p;
                p = (Power * myP[3].dmgCoe) + (GC * 6);
                AttackEnemiesInRange((Power * myP[3].dmgCoe) + (GC * 6));
                GameObject threeEffectInstance = Instantiate(threeEffect, transform.position, transform.rotation);
                Spine.Unity.SkeletonAnimation skeletonAnimation = threeEffectInstance.GetComponent<Spine.Unity.SkeletonAnimation>();
                if (p >= 1)
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, "overDamage", false);
                }
                else
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, "default", false);
                }

                syurikeni.SetActive(false);

                if (LevelS > 2 && Stopu)
                {
                    GameObject currentSyuriken = Instantiate(syuriken, transform.position, transform.rotation);
                    currentSyuriken.GetComponent<shuriken>().Initialize(this, Power * myP[2].dmgCoe);
                }
                Stopu = false;
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[3].name)
        {
            ising = false;
            act = false;
            GC = 0;
            SetAnimation(0, AnimClip[0], true);
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                SetAnimation(0, AnimClip[3], false);
                act = false;
            }
            else
            {
                SetAnimation(0, AnimClip[0], true);
                ising = false;
                act = false;
            }
        }
    }

    public override void Attack()
    {
        base.Attack();

        SetAnimation(0, AnimClip[1], false);
        act = false;
    }

    public override void Active()
    {
        act = true;
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            this.ApplyBuff("카나타0");
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[3], false);

            ResetAct();
        }
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

                if (LevelS > 2)
                {
                    Gorilla();
                }
            }
        }
    }

    public void Gorilla()
    {
        if (LevelS > 2)
        {
            this.ApplyBuff("카나타");
        }
    }

    protected override void Die()
    {
        base.Die();

        if (LevelS > 2)
        {
            this.ApplyBuff("카나타0");
        }
    }
}
