using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Olie : Tower
{
    public GameObject oneEffect;
    public GameObject oneTEffect;
    public GameObject TwoEffect;
    public GameObject ThEffect;

    public override void Start()
    {
        base.Start();

        if (LevelS > 2)
        {
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
            myActive = true;
        }

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 1)
        {
            attackCount1 = 1;
            P2T.gameObject.SetActive(true);
            P2T.text = (myP[2].trigCount - 1).ToString();
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                AttackEnemiesInRange(Power * myP[1].dmgCoe);
                if (oneEffect != null)
                {
                    Instantiate(oneEffect, transform.position, transform.rotation);
                }
                break;
            case "summon_pattern2":
                Zomrade();
                break;
            case "summon_pattern3":
                rip();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[3].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            act = false;
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

    public override void Update()
    {
        base.Update();
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            SetAnimation(0, AnimClip[3], false);
            ActiveCooldown = Active_cooltime;
            act = true;

            ResetAct();
        }
    }

    public override void Attack()
    {
        base.Attack();

        if (LevelS > 1)
        {
            attackCount1 += 1;
            P2T.text = (myP[2].trigCount - attackCount1).ToString();
        }

        if (attackCount1 >= myP[2].trigCount +1)
        {
            attackCount1 = 1;
            P2T.text = (myP[2].trigCount - attackCount1).ToString();
            SetAnimation(0, AnimClip[2], false);
        }
        else
        {
            SetAnimation(0, AnimClip[1], false);
        }

        act = false;
    }

    public void Zomrade()
    {
        GameObject zom = Instantiate(TwoEffect, transform.position, transform.rotation);
        Zomdare zomdareComponent = zom.GetComponent<Zomdare>();

        if (zomdareComponent != null)
        {
            zomdareComponent.tower = this;
        }
    }

    public void rip()
    {
        int num;
        float randomValue = Random.value; 
        if (randomValue <= 0.8f) 
        {
            num = 1;
        }
        else 
        {
            num = 2;
        }

        GameObject Bs = Instantiate(ThEffect, transform.position, transform.rotation);
        Bs.GetComponent<TombStone>().Initialize(this, num);
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
}

