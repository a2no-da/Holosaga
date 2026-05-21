using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Suisei : Tower
{
    public GameObject oneEffect;
    public GameObject axPrefab;
    public GameObject Vf3;
    public GameObject Vf3Up;
    public GameObject Vf3die;
    public Transform axSpawnPoint;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public Vector2 boxSize2 = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    HashSet<GameObject> DamageEnemiesss = new HashSet<GameObject>();
    public Sprite[] sprite;
    public AudioSource A1;
    public AudioSource A2;

    public override void Start()
    {
        base.Start();

        Active_cooltime = myC[3].cool;
        ActiveCooldown = Active_cooltime;
        inActive_cooltime = Active_cooltime;

        if (LevelS > 2)
        {
            myActive = true;
        }

        SetAnimation(0, AnimClip[0], true);
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2T.text = (myP[2].trigCount - 1).ToString();
        }

        if (LevelS > 2)
        {
            P3T.gameObject.SetActive(true);
            P3T.text = attackCount3.ToString();
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                if (oneEffect != null)
                {
                    Instantiate(oneEffect, transform.position, transform.rotation);
                }
                AttackEnemiesInRange(Power * myP[1].dmgCoe);
                break;
            case "damage_pattern2":
                A1.Play();
                GameObject axp = Instantiate(axPrefab, axSpawnPoint.position, transform.rotation);
                axp.GetComponent<ax>().Initialize(Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim, this);
                break;
            case "summon_pattern3":
                Vector3 spawnPosition = new Vector3(transform.position.x - 0.11f, transform.position.y - 0.58f, transform.position.z);
                GameObject vf3 = Instantiate(Vf3, spawnPosition, Quaternion.identity);
                Tetris tetris = vf3.GetComponent<Tetris>();

                if (tetris != null)
                {
                    tetris.speed = animationSpeed;
                    tetris.Initialize(this);
                }
                break;
            case "summon_pattern3_upgrade":
                Vf3Up.gameObject.SetActive(true);
                Vf3Up.GetComponent<Tes>().PlayAnimation();
                break;
            case "suisei_p2":
                A2.Play();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[3].name || trackEntry.Animation.Name == AnimClip[4].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            act = false;
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                if (attackCount3 >= 50)
                {
                    SetAnimation(0, AnimClip[4], false);
                }
                else
                {
                    SetAnimation(0, AnimClip[3], false);
                }
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
        Vf3Up.GetComponent<Tes>().speed = animationSpeed;

        if (attackCount3 >= 50)
        {
            towerP2 = sprite[1];
        }
        else
        {
            towerP2 = sprite[0];
        }
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

        if (LevelS > 2 && !Vf3Up.activeSelf)
        {
            act = true;
            ising = true;
            ActiveCooldown = Active_cooltime;
            if (attackCount3 >= 50)
            {
                SetAnimation(0, AnimClip[4], false);
                P3T.text = attackCount3.ToString();
            }
            else
            {
                SetAnimation(0, AnimClip[3], false);
            }

            ResetAct();
        }
    }

    void AttackEnemiesInRange(float damage = 0)
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
            if (enemyObject != null && enemyScript != null)
            {
                enemyScript.UpdateAttackingTower(this);
                enemyScript.TakeDamage(damage);
            }
        }
    }

    public void Tetris()
    {
        DamageEnemiess.Clear();
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    enemyScript.UpdateAttackingTower(this);
                    if((enemyScript.Health - (Power * myP[3].dmgCoe)) <= 0)
                    {
                        attackCount3 += 1;
                        if (attackCount3 >= 50)
                        {
                            attackCount3 = 50;
                        }
                        Instantiate(Vf3die, enemyScript.transform.position, enemyScript.transform.rotation); 
                        P3T.text = attackCount3.ToString();
                    }
                    enemyScript.TakeDamage(Power * myP[3].dmgCoe);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    public void Comet()
    {
        attackCount3 = 0;
        P3T.text = attackCount3.ToString();
        DamageEnemiesss.Clear();
        Vector2 raycastStart = new Vector2(0, 0);

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize2, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize2.x / 2, boxSize2.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize2.x / 2, boxSize2.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize2.x / 2, -boxSize2.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize2.x / 2, -boxSize2.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiesss.Contains(singleHit.collider.gameObject))
                {
                    enemyScript.UpdateAttackingTower(this);
                    enemyScript.TakeDamage(Power * myP[4].dmgCoe);
                    enemyScript.Stun(myP[4].stunTime);
                    DamageEnemiesss.Add(singleHit.collider.gameObject);
                }
            }
        }
    }
}
