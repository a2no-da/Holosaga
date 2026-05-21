using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Flare : Tower
{
    public GameObject FirePrefab;
    public Transform FireSpawnPoint;
    public GameObject Vf3;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemies = new HashSet<GameObject>();
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    private List<Enemy> strongestEnemies;
    public GameObject Hell;
    public AudioSource A1;

    public override void Start()
    {
        base.Start();
        SetAnimation(0, AnimClip[0], true);
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 2)
        {
            myActive = true;
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        layerMask = 1 << LayerMask.NameToLayer("Enemy");

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
            case "hureya_VFX_pattern1":
                A1.Play();
                break;
            case "damage_pattern1":
                GoFire();
                break;
            case "damage_pattern2":
                Find();
                if (strongestEnemies.Count > 0)
                {
                    Enemy target = strongestEnemies[Random.Range(0, strongestEnemies.Count)];
                    if (target != null && !target.Equals(null))
                    {
                        HellFire(target.transform.position);
                    }
                }
                break;
            case "damage_pattern3":
                DamageEnemiess.Clear();
                FlameThrower();
                break;
            case "vfx_pattern3":
                Vf3.gameObject.SetActive(true);
                Vf3.GetComponent<FlareV3>().PlayAnimation();
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
        Vf3.GetComponent<FlareV3>().speed = animationSpeed;
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

    void GoFire()
    {
        GameObject GoFire = Instantiate(FirePrefab, FireSpawnPoint.position, Quaternion.identity);
        FireBall fireball = GoFire.GetComponent<FireBall>();

        if (fireball != null)
        {
            fireball.Initialize(Power * myP[1].dmgCoe, myP[1].speed, myP[1].hitLim, myP[3].trigProb, this);
        }
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[3], true);
            ResetAct();
            act = true;
        }
    }


    void Find()
    {
        if (!isDead)
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            strongestEnemies = new List<Enemy>();
            float maxHealth = 0;

            foreach (Enemy e in enemies)
            {
                if (e.Health > maxHealth)
                {
                    maxHealth = e.Health;
                    strongestEnemies.Clear();
                    strongestEnemies.Add(e);
                }
                else if (e.Health == maxHealth)
                {
                    strongestEnemies.Add(e);
                }
            }
        }
    }

    void HellFire(Vector3 position)
    {
        GameObject hellFire = Instantiate(Hell, position, Quaternion.identity);
        HellFire hellfiree = hellFire.GetComponent<HellFire>();
        if (hellfiree != null)
        {
            hellfiree.Initialize(Power * myP[2].dmgCoe);
        }
    }

    void FlameThrower()
    {
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
                    enemyScript.TakeDamage(Power * myP[3].dmgCoe);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
        persent(myP[3].trigProb);
    }

    public void persent(float prob)
    {
        if (LevelS > 2)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < prob)
            {
                Find();
                if (strongestEnemies.Count > 0)
                {
                    Enemy target = strongestEnemies[Random.Range(0, strongestEnemies.Count)];
                    if (target != null && !target.Equals(null))
                    {
                        HellFire(target.transform.position);
                    }
                }
            }
        }
    }
}