using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Noel : Tower
{    
    private Coroutine pullCoroutine;
    public Vector2 pullRange = new Vector2(4.95f, 6.06f);
    public float yOffset = 1f;
    public GameObject P3;

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

        SetAnimation(0, AnimClip[0], true);
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
            case "pattern1_damage":
                AttackEnemiesInRange(Power * myP[1].dmgCoe);
                break;
            case "pattern2_damage1":
                AttackEnemiesInRange(Power * myP[2].dmgCoe);
                break;
            case "pattern2_damage2":
                AttackEnemiesInRange(Power * myP[2].dmgCoe, true);
                break;
            case "pattern3_damage":
                DamageEnemiesInRange();
                if (pullCoroutine != null)
                {
                    StopCoroutine(pullCoroutine);
                }
                pullCoroutine = StartCoroutine(PullEnemiesInRange());
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
            if(act)
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

        if (isStunned)
        {
            StopC();
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

        if (LevelS > 2)
        {
            act = true;
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[3], true);

            ResetAct();
        }
    }

    private IEnumerator PullEnemiesInRange()
    {
        GameObject p3 = Instantiate(P3, aniTransform.position, Quaternion.identity);

        Vector3 center = transform.position + new Vector3(0, yOffset, 0);

        Collider2D[] allEnemies = Physics2D.OverlapBoxAll(center, pullRange, 0f);

        enemiesInRange = new List<GameObject>();
        foreach (Collider2D enemy in allEnemies)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                enemiesInRange.Add(enemy.gameObject);
            }
        }

        float endPositionY = 0f; 
        switch (CurrentTile.GridPosition.Y)
        {
            case 0:
                endPositionY = 0.660003f;
                break;
            case 1:
                endPositionY = -1.36f;
                break;
            case 2:
                endPositionY = -3.38f;
                break;
            case 3:
                endPositionY = -5.4f;
                break;
        }
        Vector3 endPosition = new Vector3(transform.position.x + 1.65f, endPositionY, 0); 
        float duration = 0.35f; 

        foreach (GameObject enemyObject in enemiesInRange)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DisableMovement(); 

                StartCoroutine(MoveEnemyToEndPosition(enemy, endPosition, duration));
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (GameObject enemyObject in enemiesInRange)
        {
            if (enemyObject != null)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.EnableMovement();
                }
            }
        }

        yield return new WaitForSeconds(0.542f / animationSpeed);
        yield break;
    }

    private void DamageEnemiesInRange()
    {
        Vector3 center = transform.position + new Vector3(0, yOffset, 0);

        Collider2D[] allEnemies = Physics2D.OverlapBoxAll(center, pullRange, 0f);

        enemiesInRange = new List<GameObject>();
        foreach (Collider2D enemy in allEnemies)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                enemiesInRange.Add(enemy.gameObject);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.UpdateAttackingTower(this);
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(Power * myP[3].dmgCoe);
                    }
                }
            }
        }      
    }

    private IEnumerator MoveEnemyToEndPosition(Enemy enemy, Vector3 endPosition, float duration)
    {
        Vector3 startPosition = enemy.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            if (enemy == null || enemy.Laplus == true)
            {
                yield break; 
            }

            enemy.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!enemy.Laplus)
        {
            if (enemy != null)
            {
                enemy.transform.position = endPosition;
            }
        }
    }

    public void StopC()
    {
        if (pullCoroutine != null)
        {
            StopCoroutine(pullCoroutine);
            pullCoroutine = null;
        }

        ising = false;
    }

    void DebugDrawSquare(Vector3 center, Vector2 size)
    {
        Vector3 topLeft = center + new Vector3(-size.x / 2, size.y / 2, 0);
        Vector3 topRight = center + new Vector3(size.x / 2, size.y / 2, 0);
        Vector3 bottomRight = center + new Vector3(size.x / 2, -size.y / 2, 0);
        Vector3 bottomLeft = center + new Vector3(-size.x / 2, -size.y / 2, 0);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
    }

    void AttackEnemiesInRange(float damage = 0, bool rigidity = false)
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
                if (rigidity && enemyScript != null)
                {
                    enemyScript.Stun(myP[2].stunTime);
                }
            }
        }
    }
}

