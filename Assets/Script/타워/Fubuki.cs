using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System;

public class Fubuki : Tower
{
    public GameObject sanhoteto;
    public GameObject Mbar;
    public Image P3BarImage;
    public float P3 = 100;
    private bool isP3;
    private bool ishuge;
    public AnimationReferenceAsset[] siro;
    public AnimationReferenceAsset[] kuro;
    private float hugeTimeElapsed = 0f;
    private const float fillTarget = 0.3f;
    private bool isFilling = false;
    private float OH;
    private bool iskp2;
    public Sprite[] sprite;
    public bool issiro;
    private bool aaaat = false;
    public GameObject Yeah;
    public AudioSource A1;
    public AudioSource A2;
    public AudioSource A3;
    public float grotime;
    public Sprite[] sp;

    public override void Start()
    {
        base.Start();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        Active_cooltime = myC[4].cool;
        issiro = true;
        P3_cooltime = myC[3].cool;
        P3Cooldown = P3_cooltime;
        ActiveCooldown = Active_cooltime;
        inActive_cooltime = Active_cooltime;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            int remainingCount = myP[2].trigCount - 1;
            P2T.text = remainingCount.ToString();
        }

        if (LevelS > 2)
        {
            Mbar.SetActive(true);
            myActive = true;
        }
        else
        {
            Mbar.SetActive(false);
        }

        if (LevelS > 2)
        {
            P3T.text = myC[3].cool.ToString();
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                A1.Play();
                AttackEnemiesInRange(Power * myP[1].dmgCoe, false);
                break;
            case "damage_pattern2_1":
                A2.Play();
                if (!isP3 && !ishuge && LevelS > 2)
                {
                    P3 += 2;
                }
                SpawnP2();
                break;
            case "damage_pattern2_2":
                A1.Play();
                AttackEnemiesInRange(Power * myP[3].dmgCoe, true);
                break;
            case "fubuki_damage_pattern2_1_1":
                A3.Play();
                break;
        }
    }

    public override void Active()
    {
        base.Active();
        if (LevelS > 2 && P3 > 0f && !ishuge)
        {
            ising = true;
            ActiveCooldown = Active_cooltime;
            if (!isP3)
            {
                isP3 = true;
                Chakuro();
                ising = false;
            }
            else
            {
                isP3 = false;
                Chasiro();
                ising = false;
            }
            ResetAct();
        }
    }

    public void Chasiro()
    {
        AnimClip = siro;

        issiro = true;

        var yeahScript = Yeah.GetComponent<OffDVFX>();
        if (yeahScript != null)
        {
            yeahScript.SetAni("2");
        }

        if (LevelS > 2)
        {
            P3T.gameObject.SetActive(false);
        }

        if (!aaaat)
        {
            skeletonAnimation.state.ClearTracks();
            SetAnimation(0, AnimClip[0], true);
            ising = false;
        }

        RemoveBuff("Äí·Î°¡¹Ì");
        ChangeSkin(skinName2, skeletonAnimation);
        minAnimationIndex = 15;
        maxAnimationIndex = 15;

        towerP2 = sprite[0];
    }

    public void Chakuro()
    {
        AnimClip = kuro;
        issiro = false;

        var yeahScript = Yeah.GetComponent<OffDVFX>();
        if (yeahScript != null)
        {
            yeahScript.SetAni("1");
        }

        if (LevelS > 2)
        {
            P3T.gameObject.SetActive(true);
        }

        if (!aaaat)
        {
            skeletonAnimation.state.ClearTracks();
            SetAnimation(0, AnimClip[0], true);
        }

        ApplyBuff("Äí·Î°¡¹Ì");
        ChangeSkin("kuro", skeletonAnimation);

        minAnimationIndex = 16;
        maxAnimationIndex = 16;

        towerP2 = sprite[1];
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[2].name)
        {
            ising = false;
            iskp2 = false;
            aaaat = false;
            SetAnimation(0, AnimClip[0], true);
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            ising = false;
            aaaat = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }

    public override void Update()
    {
        base.Update();

        if (!isDead)
        {
            if (isP3)
            {
                float decreaseAmount = (5f / 1f) * Time.deltaTime;

                P3 -= decreaseAmount;

                P3BarImage.fillAmount = Mathf.Clamp(P3 / 100f, 0f, 1f);

                if (P3 <= 0f)
                {
                    isP3 = false;
                    ishuge = true;
                    isFilling = false;
                    Chasiro();
                    P3 = 0f;
                    P3BarImage.fillAmount = 0f;
                }
            }
            else if (!ishuge)
            {
                float increaseAmount = (5f / 1f) * Time.deltaTime;

                P3 += increaseAmount;
                if (P3 > 100f)
                {
                    P3 = 100f;
                }

                P3BarImage.fillAmount = Mathf.Clamp(P3 / 100f, 0f, 1f);
            }

            if (ishuge)
            {
                if (!isFilling)
                {
                    P3BarImage.GetComponent<Image>().sprite = sp[1];
                    isFilling = true;
                    hugeTimeElapsed = 0f;
                }

                hugeTimeElapsed += Time.deltaTime;
                if (hugeTimeElapsed <= grotime)
                {
                    P3BarImage.fillAmount = Mathf.Clamp(hugeTimeElapsed / grotime * fillTarget, 0f, fillTarget);
                }
                else
                {
                    ishuge = false;
                    P3BarImage.GetComponent<Image>().sprite = sp[0];
                    P3 = 30;
                    P3BarImage.fillAmount = 0.3f;
                }
            }

            if (isP3)
            {
                if (P3Cooldown > 0 && !isStunned)
                {
                    P3Cooldown -= Time.deltaTime;
                    P3T.text = ((int)P3Cooldown).ToString();
                }

                if (!iskp2 && P3Cooldown <= 0 && !isStunned && enemiesInRange.Count > 0)
                {
                    ising = true;
                    iskp2 = true;
                    aaaat = true;
                    skeletonAnimation.state.ClearTracks();
                    P3Cooldown = P3_cooltime;
                    SetAnimation(0, AnimClip[3], false);
                }
                else if (isStunned)
                {
                    ising = false;
                }

                if (isStunned)
                {
                    iskp2 = false;
                }
            }

            if (isStunned)
            {
                aaaat = false;
            }

            Canvas hpBarCanvas = HealthBarInstance.GetComponent<Canvas>();
            Canvas BarCanvas = Mbar.GetComponent<Canvas>();
            if (hpBarCanvas == null)
            {
                hpBarCanvas = HealthBarInstance.GetComponentInChildren<Canvas>();
            }

            if (BarCanvas == null)
            {
                BarCanvas = Mbar.GetComponentInChildren<Canvas>();
            }

            if (BarCanvas.sortingOrder != hpBarCanvas.sortingOrder)
            {
                BarCanvas.sortingOrder = hpBarCanvas.sortingOrder;
            }
        }
    }

    public override void Attack()
    {
        base.Attack();
        ising = true;
        aaaat = true;
        attackCount1++;
        int remainingCount = 2 - attackCount1;
        P2T.text = remainingCount.ToString();
        SetAnimation(0, AnimClip[1], false);

        if (LevelS > 1)
        {
            if (attackCount1 >= myP[2].trigCount)
            {
                attackCount1 = 0;
                remainingCount = 2;
                P2T.text = remainingCount.ToString();
                iskp2 = true;
                SetAnimation(0, AnimClip[2], false);
            }
        }
    }

    public void SpawnP2()
    {
        /*if (sanhoteto != null)
        {
            GameObject Sanhote = Instantiate(sanhoteto, transform.position, transform.rotation);
            Sanhote.GetComponent<Sanhoteto>().Initialize(this, Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim);
        }*/
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

        GameObject Sanhote = Instantiate(sanhoteto, spawnPosition, transform.rotation);

        if(issiro)
        {
            Sanhote.GetComponent<Sanhoteto>().Initialize(this, Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim, "siro");
        }
        else
        {
            Sanhote.GetComponent<Sanhoteto>().Initialize(this, Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim, "kuro");
        }
    }

    private void AttackEnemiesInRange(float power, bool ok)
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
                if(ok)
                {
                    float healAmount = power / 3f;
                    Health += healAmount;

                    if (Health > MaxHealth)
                    {
                        Health = MaxHealth;
                    }
                }
            }
        }
    }

    /*private IEnumerator PerformAttack(Enemy enemy, AnimationReferenceAsset animation)
    {
        SetAnimation(0, animation, false);

        yield return new WaitForSeconds(0.3f / animationSpeed);

        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = enemiesInRange[i];
            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(Power);
            }
        }
        yield return new WaitForSeconds(0.7f / animationSpeed);
    }
    
    private IEnumerator AttackSequence(Enemy enemy)
    {
        yield return StartCoroutine(PerformAttack(enemy, AnimClip[1]));
        SetAnimation(0, AnimClip[0], true);
        ising = false;
    }

    public Tile FindNearestTile(Vector3 position)
    {
        Tile nearestTile = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var item in LevelManager.Instance.Tiles)
        {
            float distance = Vector3.Distance(position, item.Value.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTile = item.Value;
            }
        }

        return nearestTile;
    }

    public Tile FindRightTile(Tile currentTile)
    {
        Vector3 rightTilePosition = new Vector3(currentTile.transform.position.x - 1.65f, currentTile.transform.position.y, currentTile.transform.position.z);

        foreach (var item in LevelManager.Instance.Tiles)
        {
            if (item.Value.transform.position == rightTilePosition)
            {
                return item.Value;
            }
        }

        return null;
    }

    private IEnumerator MoveBack()
    {
        ising = true;
        Tower towerToMove = this;
        Tile previousTile = towerToMove.CurrentTile;
        GameObject targetEnemy = detectionCollider.GetLeftmostEnemy();

        if (targetEnemy == null)
        {
            ising = false;
            yield break;
        }
        if (targetEnemy != null)
        {
            Vector2 enemyPos = targetEnemy.transform.position;
            Tile nearestTile = FindNearestTile(enemyPos);
            if (nearestTile != null)
            {
                Tile rightTile = FindRightTile(nearestTile);
                while (rightTile != null && !rightTile.CheckIsEmpty())
                {
                    nearestTile = rightTile;
                    rightTile = FindRightTile(nearestTile);
                }
                if (rightTile != null)
                {
                    SetAnimation(0, AnimClip[2], false);

                    yield return new WaitForSeconds(0.5f / animationSpeed);

                    MoveTower(towerToMove, rightTile, previousTile);

                    targetPosition = new Vector3(rightTile.transform.position.x, rightTile.transform.position.y + offset, rightTile.transform.position.z);

                    SetAnimation(0, AnimClip[3], false);

                    yield return new WaitForSeconds(0.5f / animationSpeed);

                    SetAnimation(0, AnimClip[0], true);
                }
                else
                {
                    ising = false;
                    SetAnimation(0, AnimClip[0], true);
                    MoveTower(towerToMove, previousTile, null);
                }
            }
        }
    }

    public void MoveTower(Tower towerToMove, Tile destinationTile, Tile previousTile)
    {
        if (towerToMove == null || destinationTile == null || !destinationTile.CheckIsEmpty())
        {
            return;
        }

        float offset = 0.2f;
        Vector3 towerPosition = new Vector3(destinationTile.transform.position.x, destinationTile.transform.position.y + offset, destinationTile.transform.position.z);
        towerToMove.transform.position = towerPosition;

        destinationTile.SetTower(towerToMove);

        towerToMove.CurrentTile = destinationTile;

        if (previousTile != null)
        {
            previousTile.SetTower(null);
        }

        MeshRenderer meshRenderer = towerToMove.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.sortingOrder = (destinationTile.GridPosition.Y - 3);

        towerToMove.InitializeMeshRenderer(destinationTile.GridPosition.Y - 3);

        SetAnimation(0, AnimClip[0], true);
        ising = false;
    }

    public bool IsMovingComplete(Vector3 targetPosition)
    {
        return transform.position == targetPosition;
    }*/
}
