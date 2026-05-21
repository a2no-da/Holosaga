using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Towa : Tower
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject GrenadePrefab;
    public Transform GrenadeSpawnPoint;
    public SkeletonAnimation vfx2;
    public GameObject Healprefab;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();
    public AudioSource a1;
    public AudioSource a2;
    public AudioSource a3;
    public AudioSource a4;
    public AudioSource a5;

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        layerMask = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Tower"));
        Active_cooltime = myC[3].cool;
        ActiveCooldown = Active_cooltime;
        inActive_cooltime = Active_cooltime;

        Act();

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2T.text = (myP[2].trigCount - 1).ToString();
        }

        if (LevelS > 2)
        {
            myActive = true;
        }
    }

    public override void Update()
    {
        base.Update();
        vfx2.GetComponent<TowaP2>().speed = animationSpeed;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                FireBullet();
                a2.Play();
                break;
            case "VFX_pattern2":
                vfx2.gameObject.SetActive(true);
                vfx2.AnimationState.SetAnimation(0, "animation", false);
                break;
            case "damage_pattern2":
                DamageEnemiess.Clear();
                Chargeshot();
                a3.Play();
                break;
            case "damage_pattern3":
                FireInTheHole();
                a5.Play();
                break;
            case "towa_gunCharge":
                a1.Play();
                break;
            case "towa_damage_pattern3_start":
                a4.Play(); 
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
        else if(trackEntry.Animation.Name != AnimClip[0].name)
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

        int remainingCount;

        if (LevelS > 1)
        {
            attackCount1 += 1;
        }

        if (attackCount1 >= myP[2].trigCount)
        {
            SetAnimation(0, AnimClip[2], false);
            attackCount1 = 0;
        }
        else
        {
            SetAnimation(0, AnimClip[1], false);
        }

        remainingCount = (myP[2].trigCount - 1) - attackCount1;
        P2T.text = remainingCount.ToString();
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[3], false);
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

    void FireInTheHole()
    {
        GameObject grenadeGO = Instantiate(GrenadePrefab, GrenadeSpawnPoint.position, Quaternion.identity);
        Grenade grenade = grenadeGO.GetComponent<Grenade>();

        if (grenade != null)
        {
            grenade.Initialize(Power * myP[4].dmgCoe, myP[4].speed, myP[4].hitLim, this);
        }
    }

    void Chargeshot()
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
            if (singleHit.collider != null)
            {
                if (singleHit.collider.gameObject.CompareTag("Tower"))
                {
                    Transform towerParent = singleHit.collider.transform.parent;
                    if (towerParent != null)
                    {
                        Tower towerScript = towerParent.GetComponent<Tower>();
                        if (towerScript != null)
                        {
                            PlayAnimationAndSound(towerScript);
                            towerScript.Heal(Power * myP[3].dmgCoe, this);
                        }
                    }
                }
                else if (singleHit.collider.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                    if (enemyScript != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                    {
                        enemyScript.UpdateAttackingTower(this);
                        enemyScript.TakeDamage(Power * myP[2].dmgCoe);
                    }
                }
                DamageEnemiess.Add(singleHit.collider.gameObject);
            }
        }
    }

    private void PlayAnimationAndSound(Tower tower)
    {
        GameObject instance = Instantiate(Healprefab, Vector3.zero, Quaternion.identity, tower.transform);
    }
}
