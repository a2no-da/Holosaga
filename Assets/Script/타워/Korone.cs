using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Korone : Tower
{
    public GameObject doogPunchPrefab;
    private int dogPunchCounter = 0;
    private bool p2;

    public override void Start()
    {
        base.Start();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2_cooltime = myC[2].cool;
            P2Cooldown = P2_cooltime;
            P2T.text = P2Cooldown.ToString();
        }

        if (LevelS > 2)
        {
            P3T.gameObject.SetActive(true);
            float a = myP[4].trigProb;
            P3T.text = (a * 100).ToString() + "%";
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage1_pattern1":
                AttackEnemiesInRange(Power * myP[1].dmgCoe);
                break;
            case "damage2_pattern1":
                AttackEnemiesInRange(Power * myP[2].dmgCoe);
                break;
            case "damage_pattern2":
                AttackEnemiesInRange(Power * myP[3].dmgCoe);
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[2].name)
        {
            ising = false;
            p2 = false;
            SetAnimation(0, AnimClip[0], true);
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }

    public override void Update()
    {
        base.Update();

        if (LevelS > 1)
        {
            if (P2Cooldown > 0 && !isStunned)
            {
                P2Cooldown -= Time.deltaTime;
                P2T.text = ((int)P2Cooldown).ToString();
            }

            if (!p2 && P2Cooldown <= 0 && !isStunned && enemiesInRange.Count > 0)
            {
                p2 = true;
                ising = true;
                skeletonAnimation.state.ClearTracks();
                P2Cooldown = P2_cooltime;
                P2T.text = ((int)P2Cooldown).ToString();
                SetAnimation(0, AnimClip[2], false);
            }
            else if (isStunned)
            {
                p2 = false;
            }
        }
    }

    /*public override void Attack()
    {
        base.Attack();

        dogPunchCounter = 0;
        attackCount1++;
        int remainingCount = 1 - attackCount1;
        P2T.text = remainingCount.ToString(); 
        SetAnimation(0, AnimClip[1], false);

        if (LevelS > 1)
        {
            if (attackCount1 >= 2) 
            {
                attackCount1 = 0;
                remainingCount = 1 - attackCount1;
                P2T.text = remainingCount.ToString(); 
                SetAnimation(0, AnimClip[2], false);
            }
        }
    }*/

    public override void Attack()
    {
        base.Attack();

        dogPunchCounter = 0;
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
                if (LevelS > 2)
                {
                    if (dogPunchCounter < 3 && Random.value <= myP[4].trigProb)
                    {
                        DoogPunch(enemyScript);
                        dogPunchCounter++;
                    }
                }
            }
        }
    }

    private void DoogPunch(Enemy enemy)
    {
        float minY = -0.8f;
        float maxY = -0.4f;
        float randomY = Random.Range(minY, maxY);

        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + randomY, transform.position.z);

        GameObject doogPunch = Instantiate(doogPunchPrefab, newPosition, Quaternion.identity);
        Collider2D collider = doogPunch.GetComponent<Collider2D>();
        collider.offset = new Vector2(collider.offset.x, collider.offset.y - randomY);
        doogPunch.GetComponent<DoogPunch>().Initialize(Power * myP[3].dmgCoe, animationSpeed, this);
    }
}