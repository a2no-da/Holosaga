using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Kobo : Tower
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject cloudPrefab;
    public GameObject cloud;

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 2)
        {
            myActive = true;
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                FireBullet();
                break;
            case "summon_pattern3":
                if (cloud != null)
                {
                    Destroy(cloud);
                    cloud = null;
                }
                cloud = Instantiate(cloudPrefab, transform.position, Quaternion.identity);
                RainCloud rainCloud = cloud.GetComponent<RainCloud>();

                if (rainCloud != null)
                {
                    rainCloud.Initialize(Power * myP[3].dmgCoe, Power * myP[4].dmgCoe, myP[4].stunTime, this);
                }
                break;
        }
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[2], false);
            act = true;
            ResetAct();
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[2].name)
        {
            SetAnimation(0, AnimClip[3], true);
            act = false;
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (trackEntry.Animation.Name != AnimClip[3].name)
            {
                if (act)
                {
                    SetAnimation(0, AnimClip[2], false);
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
    }

    public void stop()
    {
        ising = false;
        SetAnimation(0, AnimClip[4], false);
    }

    public override void Update()
    {
        base.Update();

        if(isStunned)
        {
            if (cloud != null)
            {
                Destroy(cloud);
                cloud = null;
            }
        }
    }

    protected override void Die()
    {
        base.Die();

        if (cloud != null)
        {
            Destroy(cloud);
            cloud = null;
        }
    }

    public override void Attack()
    {
        base.Attack();

        SetAnimation(0, AnimClip[1], false);
    }

    void FireBullet()
    {
        GameObject drop = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        WaterDrop waterDrop = drop.GetComponent<WaterDrop>();

        if (waterDrop != null)
        {
            waterDrop.Initialize(Power * myP[1].dmgCoe, myP[1].speed, myP[2].speed, myP[1].hitLim, this);
        }
    }
}