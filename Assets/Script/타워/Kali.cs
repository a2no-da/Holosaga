using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Kali : Tower
{
    public GameObject oneEffect;
    public GameObject coilPrefab;
    public GameObject UDCoilPrefab;
    public GameObject P3effectPrefab;
    public Transform coilSpawnPoint;
    public Force forceComponent;
    public GameObject Dethss;
    public int tes; 

    public override void Start()
    {
        base.Start();

        this.ApplyBuff("漠府");

        if (LevelS > 2)
        {
            myActive = true;
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        if (forceComponent == null)
        {
            forceComponent = gameObject.AddComponent<Force>();
        }

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2T.text = (myP[3].trigCount - 1).ToString();
        }

        Act();
        GameManager.instance.kariii = this;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "pattern1_damage":
                AttackEnemiesInRange(Power * myP[1].dmgCoe);
                if (oneEffect != null)
                {
                    Instantiate(oneEffect, transform.position, transform.rotation);
                }
                break;
            case "pattern2_summon":
                SummonSkeleton();
                break;
            case "pattern3_damage":
                if (P3effectPrefab != null)
                {
                    Instantiate(P3effectPrefab, transform.position, transform.rotation);
                }
                FireInAllDirections();
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

    public void getsoul()
    {
        this.ApplyBuff("漠府");
    }

    public void Bossgetsoul()
    {
        this.ApplyBuff("漠府焊胶");
    }

    public override void Update()
    {
        base.Update();
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

    public override void Attack()
    {
        base.Attack();

        int remainingCount;

        if (LevelS > 1)
        {
            attackCount1 += 1;
            remainingCount = (myP[3].trigCount - 1) - attackCount1;
            P2T.text = remainingCount.ToString();
        }

        if (attackCount1 >= myP[3].trigCount && rigtTile())
        {
            attackCount1 = 0;
            remainingCount = (myP[3].trigCount - 1) - attackCount1;
            P2T.text = remainingCount.ToString();
            SetAnimation(0, AnimClip[2], false);
        }
        else
        {
            SetAnimation(0, AnimClip[1], false);
        }

        if (attackCount1 >= myP[3].trigCount && !rigtTile())
        {
            attackCount1 = myP[3].trigCount;
            remainingCount = myP[3].trigCount - attackCount1;
            P2T.text = remainingCount.ToString();
        }

        act = false;
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
            }
        }
    }

    void FireInAllDirections()
    {
        Vector3[] directions = new Vector3[]
 {
        Vector3.right,      
        Vector3.left,       
        Vector3.up,          
        Vector3.down,       
        new Vector3(1, 1, 0),  
        new Vector3(-1, 1, 0),
        new Vector3(1, -1, 0), 
        new Vector3(-1, -1, 0) 
 };

        foreach (Vector3 direction in directions)
        {
            if (direction == Vector3.up || direction == Vector3.down)
            {
                FireCoil(direction, UDCoilPrefab);
            }
            else
            {
                FireCoil(direction, coilPrefab); 
            }
        }
    }

    void FireCoil(Vector3 direction, GameObject coilPrefab)
    {
        Quaternion rotation;

        if (direction == Vector3.right)
        {
            rotation = Quaternion.Euler(0, -180, 0);
        }
        else if (direction == Vector3.left)
        {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector3.up)
        {
            rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (direction == Vector3.down)
        {
            rotation = Quaternion.Euler(0, 180, 90);
        }
        else if (direction == new Vector3(1, 1, 0))
        {
            rotation = Quaternion.Euler(0, 180, -45);
        }
        else if (direction == new Vector3(-1, 1, 0))
        {
            rotation = Quaternion.Euler(0, 0, -45);
        }
        else if (direction == new Vector3(1, -1, 0))
        {
            rotation = Quaternion.Euler(0, -180, 45);
        }
        else if (direction == new Vector3(-1, -1, 0))
        {
            rotation = Quaternion.Euler(0, 0, 45);
        }
        else
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        GameObject coilInstance = Instantiate(coilPrefab, coilSpawnPoint.position, Quaternion.identity);
        Coil coil = coilInstance.GetComponent<Coil>();

        Transform deathCoilChild = coilInstance.transform.Find("单胶 内老");
        if (deathCoilChild != null)
        {
            deathCoilChild.rotation = rotation;
        }

        if (coil != null)
        {
            coil.Initialize(Power * myP[4].dmgCoe, myP[4].speed, myP[4].hitLim, direction, this);
        }
    }

    public void SummonSkeleton()
    {
        Tile currentTile = GetCurrentTile();

        if (currentTile != null)
        {
            Vector3 rightPosition = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, currentTile.transform.position.z);

            Tile rightTile = null;
            foreach (var tile in LevelManager.Instance.Tiles)
            {
                if (Vector3.Distance(tile.Value.transform.position, rightPosition) < 0.1f)
                {
                    rightTile = tile.Value;
                    break;
                }
            }

            if (rightTile != null)
            {
                PushEnemiesOnTile(rightTile);

                GameObject De = Instantiate(Dethss, transform.position, Quaternion.identity);
                DeathSense death = De.GetComponent<DeathSense>();

                if (death != null)
                {
                    death.Initialize(this);
                }
            }
            else
            {
                attackCount1 = myP[3].trigCount;
                P2T.text = 0.ToString();
            }
        }
        else
        {
            attackCount1 = myP[3].trigCount;
            P2T.text = 0.ToString();
        }
    }

    private bool rigtTile()
    {
        Tile currentTile = GetCurrentTile();

        if (currentTile != null)
        {
            Vector3 rightPosition = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, currentTile.transform.position.z);

            foreach (var tile in LevelManager.Instance.Tiles)
            {
                if (Vector3.Distance(tile.Value.transform.position, rightPosition) < 0.1f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void PushEnemiesOnTile(Tile tile)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tile.transform.position, tile.GetComponent<BoxCollider2D>().size, LayerMask.GetMask("Enemy"));
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    forceComponent.PublicApplyForce(rb, new Vector2(myP[3].addForce, 0));
                }
            }
        }
    }

    private Tile GetCurrentTile()
    {
        Vector3 currentPosition = transform.position;

        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (var tile in LevelManager.Instance.Tiles)
        {
            float distance = Vector3.Distance(currentPosition, tile.Value.transform.position);
            if (distance < closestDistance)
            {
                closestTile = tile.Value;
                closestDistance = distance;
            }
        }

        return closestTile;
    }
}
