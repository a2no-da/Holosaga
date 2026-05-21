using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Chloe : BOSS
{
    private LayerMask layerMask;
    public GameObject daggerPrefab;
    public Transform daggerSpawnPoint;
    public GameObject SpawnSakamataPrefab;
    public SpawnPoint[] spawnPoints;
    public float assassinationDistance = 2.475f;
    private Tower leftTower;
    private Vector3 originalPosition;
    private bool isAssassinationFailed;
    public GameObject KagePrefab;
    private int kageCount = 0;

    public AudioSource A1;
    public AudioSource A2;
    public AudioSource A3s;
    public AudioSource A3d;
    public AudioSource A4s;
    public AudioSource A4d;
    public AudioSource A4t;

    private Vector3[] originalSpawnPositions = new Vector3[]
      {
        new Vector3(8.19f, 0.660003f, 0f),
        new Vector3(8.19f, -1.36f, 0f),
        new Vector3(8.19f, -3.38f, 0f),
        new Vector3(8.19f, -5.4f, 0f)
      };

    private List<Vector3> spawnPositions;

    public override void Start()
    {
        base.Start();
        layerMask = LayerMask.GetMask("Tower");
        spawnPositions = new List<Vector3>(originalSpawnPositions);
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                Attack1();
                A1.Play();
                break;
            case "damage_pattern2":
                Attack2();
                A2.Play();
                break;
            case "cloe_start_pattern3":
                A3s.Play();
                break;
            case "cloe_damage_pattern3":
                Attack33();
                break;
            case "cloe_start_pattern4":
                A4s.Play();
                break;
            case "teleport1_pattern4":
                TELPo();
                A4t.Play();
                break;
            case "damage_pattern4":
                A4d.Play();
                Pattern4Attacked();
                break;
            case "teleport_pattern4":
                A4t.Play();
                break;
            case "teleport2_pattern4":
                faill();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name && trackEntry.Animation.Name != AnimClip[2].name && trackEntry.Animation.Name != AnimClip[6].name 
            && trackEntry.Animation.Name != AnimClip[5].name && trackEntry.Animation.Name != AnimClip[7].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);

            if(trackEntry.Animation.Name == AnimClip[4].name)
            {
                ResetAttack();
            }
        }
        
        if (trackEntry.Animation.Name == AnimClip[5].name)
        {
            fail();
        }
        
        if (trackEntry.Animation.Name == AnimClip[7].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            moveSpeed = initialMoveSpeed;
        }
    }

    public override void Update()
    {
        base.Update();

        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, -transform.right, raycastDistance, layerMask);
        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        Vector2 raycastEnd = raycastStart - new Vector2(transform.right.x, transform.right.y) * raycastDistance;
        Debug.DrawLine(raycastStart, raycastEnd, Color.blue);

        if ((hit.collider == null || (!hit.collider.CompareTag("Tower") && !hit.collider.CompareTag("Kabe"))) && !kabedong)
        {
            if (!ising && !isStunned && !isTeleporting)
            {
                if (!isSlowEffectActive)
                {
                    moveSpeed = initialMoveSpeed;
                }
                SetAnimation(0, AnimClip[2], true);
            }
        }
        else if (hit.collider != null && (hit.collider.CompareTag("Tower") && !ising))
        {
            if (!ising && !isTeleporting)
            {
                moveSpeed = 0;
                SetAnimation(0, AnimClip[0], true);
            }

            if (Pattern1Timer <= 0 && !ising && !isStunned && !isTeleporting && (targetEnemy != null || !shouldIgnoreRange2))
            {
                BasicAttack();
                Pattern1Timer = Pattern1Cooldown;
            }
            else if (isStunned)
            {
                ising = false;
                Pattern1Timer = Pattern1Cooldown;
                SetAnimation(0, AnimClip[0], true);
            }
        }
        else if (hit.collider != null && (hit.collider.CompareTag("Kabe") && !ising) || kabedong)
        {
            if (!isTeleporting)
            {
                moveSpeed = 0;
                SetAnimation(0, AnimClip[0], true);
            }
        }

        if (!ising && Pattern2Timer <= 0 && !isStunned && !isTeleporting)
        {
            Pattern2Timer = Pattern2Cooldown;
            Attack();
        }
        else if (isStunned)
        {
            ising = false;
            Pattern2Timer = Pattern2Cooldown;
        }

        if (Pattern3Timer <= 0 && !ising && !isStunned && !isTeleporting)
        {
            Pattern3Attack();
            Pattern3Timer = Pattern3Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern3Timer = Pattern3Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        if ((towers.Length > 0  || !shouldIgnoreRange5))
        {
            if (Pattern4Timer <= 0 && !isTeleporting && !isStunned)
            {
                Pattern4Attack();
                Pattern4Timer = Pattern4Cooldown;
            }
        }
        else if (isStunned)
        {
            ising = false;
            Pattern4Timer = Pattern4Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        if (Pattern5Timer <= 0 && !isTeleporting && !isStunned && (targetEnemy != null || !shouldIgnoreRange1))
        {
            SpawnSakamata();
            Pattern5Timer = Pattern5Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern5Timer = Pattern5Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        if (isStunned)
        {
            ResetAttack();
        }

        MoveTowardsEndPoint();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower") && collider.transform.parent != null)
        {
            GameObject potentialTarget = collider.transform.parent.gameObject;
            if (targetEnemy == null || Vector2.Distance(transform.position, potentialTarget.transform.position) < Vector2.Distance(transform.position, targetEnemy.transform.position))
            {
                targetEnemy = potentialTarget;
            }
        }

        if (collider.gameObject.CompareTag("Kabe"))
        {
            kabedong = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Kabe"))
        {
            kabedong = false;
        }

        if (targetEnemy == null)
        {
            return;
        }

        if (collider.gameObject.CompareTag("Tower") && collider.transform.parent != null && targetEnemy != null && collider.transform.parent.gameObject == targetEnemy)
        {
            targetEnemy = null;
            moveSpeed = initialMoveSpeed;
        }
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising && !isTeleporting && !isStunned)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private void BasicAttack()
    {
        ising = true;
        SetAnimation(0, AnimClip[1], false);
    }

    private void Attack1()
    {
        if (targetEnemy != null)
        {
            Tower tower = targetEnemy.GetComponent<Tower>();

            targetEnemy.GetComponent<Tower>().TakeDamage(Power * mP[2].dmgCoe);

            if (tower.Health <= 0)
            {
                targetEnemy = null;
            }
        }
    }

    public override void Attack()
    {
        ising = true;
        SetAnimation(0, AnimClip[3], false);
    }

    public override void Attack2()
    {
        ising = true;
        DaggerGo();
    }


    void DaggerGo()
    {
        GameObject DaggerGo = Instantiate(daggerPrefab, daggerSpawnPoint.position, Quaternion.identity);
        Dagger dagger = DaggerGo.GetComponent<Dagger>();

        if (dagger != null)
        {
            dagger.Initialize(Power * mP[3].dmgCoe, mP[3].speed, mP[3].hitLim);
        }
    }

    public void Pattern3Attack()
    {
        if (!ising)
        {
            ising = true;
            SetAnimation(0, AnimClip[4], false);
        }
    }

    private void Attack33()
    {
        int randomIndex = Random.Range(0, spawnPositions.Count);
        Vector3 randomPosition = spawnPositions[randomIndex];

        if (kageCount == 0 || kageCount == 2) 
        {
            randomPosition.x = -Mathf.Abs(randomPosition.x);
        }

        GameObject kagePrefab = Instantiate(KagePrefab, randomPosition, Quaternion.identity);
        Kage kage = kagePrefab.GetComponent<Kage>();

        if (kage != null)
        {
            kage.Initialize(Power * mP[4].dmgCoe);
        }

        spawnPositions.RemoveAt(randomIndex);

        kageCount++;
    }

    public void ResetAttack()
    {
        kageCount = 0;
        spawnPositions = new List<Vector3>(originalSpawnPositions);
    }

    public void Pattern4Attack()
    {
        if (!ising)
        {
            moveSpeed = 0;
            ising = true;
            SetAnimation(0, AnimClip[5], false);
        }
    }

    private void TELPo()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        List<GameObject> aliveTowers = new List<GameObject>();
  
        foreach (GameObject tower in towers)
        {
            Tower towerComponent = tower.GetComponentInParent<Tower>();
            if (towerComponent != null && towerComponent.Health > 0) 
            {
                if (!towerComponent.isDead)
                {
                    aliveTowers.Add(tower);
                }
            }
        }
        if (aliveTowers.Count > 0)
        {
            GameObject target = aliveTowers[Random.Range(0, aliveTowers.Count)];
            originalPosition = transform.position;
            transform.position = target.transform.position + new Vector3(1.65f, 0, 0);
        }
        else
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            moveSpeed = initialMoveSpeed;
        }
    }

    private void Pattern4Attacked()
    {
        Vector3 raycastStart = transform.position + new Vector3(-1.15f, 0.15f, 0);
        Vector3 boxSize = new Vector3(2.3f, 0.3f, 0f);
        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, -transform.right, assassinationDistance, layerMask);

        if (hit.collider != null)
        {
            Tower tower = hit.collider.GetComponentInParent<Tower>();
            if (tower != null)
            {
                tower.TakeDamage(10);
                tower.TakeDamage(9999);
            }
        }
    } 

    private void fail()
    {
        ising = true;
        SetAnimation(0, AnimClip[7], false);
    }

    private void faill()
    {
        transform.position = originalPosition;
    }

    private void SpawnSakamata()
    {
        Vector3 ChloePosition = transform.position;

        List<SpawnPoint> differentPositions = new List<SpawnPoint>();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (Mathf.Abs(ChloePosition.y - spawnPoint.position.y) > Mathf.Epsilon)
            {
                differentPositions.Add(spawnPoint);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, differentPositions.Count);
            SpawnPoint spawnPoint = differentPositions[randomIndex];
            differentPositions.RemoveAt(randomIndex);

            Vector3 finalSpawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, ChloePosition.z);
            GameObject spawnedSakamata = Instantiate(SpawnSakamataPrefab, finalSpawnPosition, Quaternion.identity);

            MeshRenderer meshRenderer = spawnedSakamata.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = spawnPoint.sortingOrder;
            }
        }
    }
}
