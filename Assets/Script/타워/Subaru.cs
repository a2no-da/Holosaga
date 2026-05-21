using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Subaru : Tower
{
    private float aforce;
    public AudioSource AttackS1;
    public AudioSource AttackS2;
    public GameObject duckPrefab;
    public Transform duckSpawnPoint;
    private Force forceComponent;
    public GameObject oneEffect;

    public override void Start()
    {
        base.Start();
        if (LevelS > 1)
        {
            myActive = true;
            Active_cooltime = myC[2].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }
        forceComponent = gameObject.AddComponent<Force>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "pattern1_damage1":
                PerformAttack();
                AttackS1.Play();
                break;
            case "pattern1_damage2":
                PerformEndAttack();
                AttackS2.Play();
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

    public override void Update()
    {
        base.Update();
    }


    public override void Attack()
    {
        base.Attack();
        if (LevelS > 2)
        {
            SetAnimation(0, AnimClip[2], false);
        }
        else
        {
            SetAnimation(0, AnimClip[1], false);
        }
    }

    private void PerformAttack()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = enemiesInRange[i];
            if (enemyObject == null)
            {
                enemiesInRange.RemoveAt(i);
                return;
            }
            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                Rigidbody2D enemyRb = enemyObject.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyScript.isPushedOrPulled = true;
                    Vector2 force = LevelS > 2 ? new Vector2((myP[1].addForce * 1.5f), 0) : new Vector2(myP[1].addForce, 0);
                    forceComponent.PublicApplyForce(enemyRb, force);
                    minAnimationIndex = 9;
                    maxAnimationIndex = 9;
                    enemyScript.UpdateAttackingTower(this);
                    enemyScript.TakeDamage(Power * myP[1].dmgCoe);
                }
            }
        }
    }

    private void PerformEndAttack()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = enemiesInRange[i];
            if (enemyObject == null)
            {
                enemiesInRange.RemoveAt(i);
                return;
            }
            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                Rigidbody2D enemyRb = enemyObject.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    //enemyScript.isPushedOrPulled = true;
                    //Vector2 force = LevelS > 2 ? new Vector2((-myP[2].addForce * 1.5f), 0) : new Vector2(-myP[2].addForce, 0);
                    //forceComponent.PublicApplyForce(enemyRb, force);
                    minAnimationIndex = 10;
                    maxAnimationIndex = 10;
                    enemyScript.UpdateAttackingTower(this);
                    if(LevelS > 2)
                    {
                        enemyScript.TakeDamage(Power * myP[2].dmgCoe);
                    }
                    else
                    {
                        enemyScript.TakeDamage(Power * myP[1].dmgCoe);
                    }
                }
            }
        }
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 1)
        {
            ising = true;

            if (oneEffect != null)
            {
                Instantiate(oneEffect, transform.position, transform.rotation);
            }

            GameObject DGO = Instantiate(duckPrefab, duckSpawnPoint.position, Quaternion.identity);
            Duck duck = DGO.GetComponent<Duck>();

            if (duck != null)
            {
                duck.Initialize(Power * myP[3].dmgCoe, myP[3].speed, myP[3].hitLim, myP[3].addForce, this);
            }
            ActiveCooldown = Active_cooltime;
            ising = false;
            act = false;
            ResetAct();
        }
    }
}
