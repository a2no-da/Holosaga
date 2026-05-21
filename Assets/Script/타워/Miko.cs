using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Miko : Tower
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float sCooldown;
    public GameObject luckPrefab;
    public GameObject disasterPrefab;
    public GameObject zenlossPre;
    public GameObject toriiPrefab;
    public GameObject roadPrefab;
    public GameObject firePrefab;
    public AudioSource BB;
    public AudioSource GG;
    
    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        GameManager.instance.miko = this;
        GameManager.instance.zenlossPrefab = zenlossPre;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2_cooltime = myC[2].cool;
            P2T.text = myC[2].cool.ToString();
            sCooldown = P2_cooltime;
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
            case "summon_pattern2_bad":
                Disaster();
                break; 
            case "summon_pattern2_good":
                Luck();
                break;
            case "miko_choosingBad":
                BB.Play();
                break;
            case "miko_choosing":
                GG.Play();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            SetAnimation(0, AnimClip[0], true);
            act = false;
            ising = false;
        }
    }

    public override void Update()
    {
        base.Update();

        if (sCooldown > 0 && !isStunned && LevelS > 1)
        {
            sCooldown -= Time.deltaTime;
            P2T.text = ((int)sCooldown).ToString();
        }

        if (sCooldown <= 0 && !isStunned && enemiesInRange.Count > 0 && LevelS > 1 && !ising)
        {
            ising = true;
            if (Random.Range(0, 2) == 0) 
            {
                SetAnimation(0, AnimClip[2], false);
            }
            else
            {
                SetAnimation(0, AnimClip[3], false);
            }
            sCooldown = P2_cooltime;
        }
        else if (isStunned)
        {
            sCooldown = P2_cooltime;
        }
    }

    public override void Attack()
    {
        base.Attack();

        SetAnimation(0, AnimClip[1], false);
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
        }
    }

    void FireBullet()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(Power * myP[1].dmgCoe, myP[1].speed, myP[1].hitLim, this);
        }
    }

    void Luck()
    {
        if (GameManager.Instance.AllTowers.Count > 0)
        {
            int randomIndex = Random.Range(0, GameManager.Instance.AllTowers.Count);
            Tower selectedTower = GameManager.Instance.AllTowers[randomIndex];

            GameObject luckGO = Instantiate(luckPrefab, selectedTower.transform.position, Quaternion.identity);
            MikoLuck mikoLuck = luckGO.GetComponent<MikoLuck>();

            if (mikoLuck != null)
            {
                mikoLuck.Initialize(Power * myP[2].dmgCoe, this);
            }
        }
    }

    void Disaster()
    {
        Vector2 raycastStart = Vector2.zero; 
        Vector2 boxSize = new Vector2(19.2f, 11f); 
        LayerMask enemyLayerMask = LayerMask.GetMask("Enemy"); 

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, Vector2.zero, 0f, enemyLayerMask);

        if (hitResults.Length > 0)
        {
            int randomIndex = Random.Range(0, hitResults.Length);
            GameObject selectedEnemy = hitResults[randomIndex].collider.gameObject;

            GameObject disast = Instantiate(disasterPrefab, selectedEnemy.transform.position, Quaternion.identity);
            MikoD disaster = disast.GetComponent<MikoD>();
            if (disaster != null)
            {
                disaster.Initialize(Power * myP[3].dmgCoe, Power * myP[4].dmgCoe, myP[3].stunTime, this);
            }
        }
    }
}

