using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Watson : Tower
{
    public GameObject bulletPrefab;
    public GameObject bulletPrefab2;
    public Transform bulletSpawnPoint;
    public AudioSource bsound1;
    public AudioSource bsound2;
    public AudioSource wsound;
    public GameObject P3Prefab;

    public override void Start()
    {
        base.Start();

        if (LevelS > 2)
        {
            myActive = true;
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2T.text = (myP[2].trigCount - 1).ToString();
        }
        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "pattern1_damage1":
                FireBullet();
                bsound1.Play();
                break;
            case "pattern2_damage1":
                FireBullet2();
                bsound2.Play();
                break;
            case "watson_pattern3_sound":
                wsound.Play();
                break;
            case "pattern3_trigger":
                SP();
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

    public override void Attack()
    {
        base.Attack();
        int remainingCount;

        if (LevelS > 1)
        {
            attackCount1 += 1;
            remainingCount = (myP[2].trigCount - 1) - attackCount1;
            P2T.text = remainingCount.ToString();
        }

        if (attackCount1 >= myP[2].trigCount)
        {
            attackCount1 = 0;
            remainingCount = (myP[2].trigCount - 1) - attackCount1;
            P2T.text = remainingCount.ToString();
            SetAnimation(0, AnimClip[2], false);
        }
        else
        {
            SetAnimation(0, AnimClip[1], false);
        }
        act = false;
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

    void FireBullet()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(Power * myP[1].dmgCoe, myP[1].speed, myP[1].hitLim, this);
        }
    }

    void FireBullet2()
    {
        GameObject bulletGO = Instantiate(bulletPrefab2, bulletSpawnPoint.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim, this);
        }
    }

    void SP()
    {
        Instantiate(P3Prefab, transform.position, Quaternion.identity, this.transform);
        this.ApplyBuff("¿Ó½¼3");
        GameObject[] allTowers = GameObject.FindGameObjectsWithTag("RS");
        List<GameObject> deadTowers = new List<GameObject>();

        foreach (GameObject tower in allTowers)
        {
            Tower towerComponent = tower.transform.parent.GetComponent<Tower>();
            if (towerComponent != null && towerComponent.isDead)
            {
                deadTowers.Add(tower);
            }
        }

        if (deadTowers.Count > 0)
        {
            GameObject randomDeadTower = deadTowers[Random.Range(0, deadTowers.Count)];
            Tower towerComponent = randomDeadTower.transform.parent.GetComponent<Tower>();

            int originalRespawnTime = towerComponent.respawnTime;
            towerComponent.respawnTime = (int)(towerComponent.respawnTime * 0.5f);
            GameObject spawnedObject = Instantiate(P3Prefab, randomDeadTower.transform.position, Quaternion.identity, randomDeadTower.transform);

            MeshRenderer meshRenderer = spawnedObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingLayerName = "Back";
                //meshRenderer.sortingOrder = 0; 
            }

            Debug.Log("Á×Àº Å¸¿ö : " + randomDeadTower.name + ", ¸®½ºÆù ½Ã°£À» " + originalRespawnTime + "¿¡¼­ " + towerComponent.respawnTime + "ÁÙ");
        }
    }
}
