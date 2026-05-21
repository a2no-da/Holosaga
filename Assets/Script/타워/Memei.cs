using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Mumei : Tower
{
    public GameObject featherPrefab;
    public Transform featherSpawnPoint;
    public bool isfe = false;
    private float buffDuration = 10f;
    private float buffTimer = 0f;
    public GameObject Yeah1;
    public GameObject Yeah2;
    public AudioSource a1;
    public AudioSource a2;
    public AudioSource a3;

    public override void Start()
    {
        base.Start();

        Active_cooltime = myC[2].cool;
        ActiveCooldown = Active_cooltime;
        inActive_cooltime = Active_cooltime;

        if (LevelS > 1)
        {
            myActive = true;
        }

        SetAnimation(0, AnimClip[0], true);
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "mumei_damage_pattern1_start":
                a1.Play();
                break;
            case "damage_pattern1":
                a2.Play();
                FireFeather();
                break;
            case "damage_pattern2":
                if(isfe)
                {
                    FireFeather();
                }
                break;
            case "buff_pattern2":
                if (act && !isfe)
                {
                    isfe = true;
                    a3.Play();
                    ApplyBuff("¹«¸ÞÀÌ2");

                    var yeah1Script = Yeah1.GetComponent<OffDVFX>();
                    if (yeah1Script != null)
                    {
                        yeah1Script.SetAni("front");
                    }

                    var yeah2Script = Yeah2.GetComponent<OffDVFX>();
                    if (yeah2Script != null)
                    {
                        yeah2Script.SetAni("back");
                    }

                    buffTimer = buffDuration;
                }
                break;
        }
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 1)
        {
            act = true;
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[2], true);

            ResetAct();
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[2].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            act = false;
        }
        else if(trackEntry.Animation.Name != AnimClip[0].name)
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

    public override void Update()
    {
        base.Update();

        if (isfe)
        {
            buffTimer -= Time.deltaTime; 

            if (buffTimer <= 0f)
            {
                isfe = false; 
            }
        }
    }

    public override void Attack()
    {
        base.Attack();

        int randomIndex = UnityEngine.Random.Range(0, 3);
        int selectedAnimIndex;

        if (randomIndex == 0)
        {
            selectedAnimIndex = 1; 
        }
        else if (randomIndex == 1)
        {
            selectedAnimIndex = 3; 
        }
        else 
        {
            selectedAnimIndex = 4; 
        }

        SetAnimation(0, AnimClip[selectedAnimIndex], false);
    }

    void FireFeather()
    {
        bool isp3 = false;
        GameObject featherGo = Instantiate(featherPrefab, featherSpawnPoint.position, Quaternion.identity);
        Feather feather = featherGo.GetComponent<Feather>();

        if (LevelS > 2)
        {
            isp3 = true;
        }

        if (feather != null)
        {
            feather.Initialize(Power * myP[1].dmgCoe, myP[1].speed, myP[1].hitLim, this, Power * myP[2].dmgCoe, Power * myP[3].dmgCoe, isp3);
        }
    }
}
