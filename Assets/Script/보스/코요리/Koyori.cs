using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;

public class Koyori : BOSS
{
    private LayerMask layerMask;
    public GameObject kokoroPrefab;
    public GameObject Vfx2;
    public GameObject Vfx4;
    public Transform aniTransform;
    public SpawnPoint[] spawnPoints;
    public GameObject platePrefab;
    public GameObject potionPrefab;
    private LevelManager levelManager;
    public AudioClip clip;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
    }

    public override void Start()
    {
        base.Start();
        layerMask = LayerMask.GetMask("Tower");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                Attack1();
                break;
            case "damage_pattern2":
                Attack2();
                break;
            case "damage2_pattern3":
                Pattern3Throw(false);
                break;
            case "damage1_pattern3":
                Pattern3Throw(true);
                break;
            case "rustling":
                Busitherock();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name && trackEntry.Animation.Name != AnimClip[2].name && trackEntry.Animation.Name != AnimClip[6].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
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
            moveSpeed = 0;
            SetAnimation(0, AnimClip[0], true);

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

        if (!ising && Pattern2Timer <= 0 && !isStunned && (targetEnemy != null || !shouldIgnoreRange3) && !isTeleporting)
        {
            Pattern2Timer = Pattern2Cooldown;
            Attack();
        }
        else if (isStunned)
        {
            ising = false;
            Pattern2Timer = Pattern2Cooldown;
        }

        if (!ising && Pattern3Timer <= 0 && !isTeleporting && !isStunned && (targetEnemy != null || !shouldIgnoreRange4))
        {
            Pattern3Timer = Pattern3Cooldown;
            Pattern3();
        }
        else if (isStunned)
        {
            ising = false;
            Pattern3Timer = Pattern3Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        if (!ising && Pattern4Timer <= 0 && !isTeleporting && !isStunned && (targetEnemy != null || !shouldIgnoreRange5))
        {
            GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
            List<GameObject> aliveTowers = new List<GameObject>();

            foreach (GameObject tower in towers)
            {
                Tower towerComponent = tower.transform.parent.GetComponent<Tower>();
                if (towerComponent != null && !towerComponent.isDead)
                {
                    aliveTowers.Add(tower);
                }
            }

            if (aliveTowers.Count > 0)
            {
                Pattern4Timer = Pattern4Cooldown;

                GameObject targetTower = aliveTowers[Random.Range(0, aliveTowers.Count)];

                Tower towerComponent = targetTower.transform.parent.GetComponent<Tower>();
                if (towerComponent != null)
                {
                    StartCoroutine(DelayedStun(towerComponent, mP[6].stunTime, 0.00001f));
                    Instantiate(Vfx4, towerComponent.transform.position, Quaternion.identity);
                }
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
            SpawnKokoro();
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
            SetAnimation(0, AnimClip[0], true);
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

    public void BasicAttack()
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
        StartCoroutine(SpawnVFX2WithDelay());
    }

    private IEnumerator SpawnVFX2WithDelay()
    {
        Tile currentTile = GetCurrentTile();
        Tile leftTile = FindLeftTile(currentTile);
        Vector3 spawnPosition = leftTile != null ? leftTile.transform.position : transform.position;

        for (int i = 1; i <= 5; i++)
        {
            if (i != 1)
            {
                spawnPosition.x -= 1.65f;
            }
            GameObject vf2 = Instantiate(Vfx2, spawnPosition, Quaternion.identity);
            Vfx2 vfx = vf2.GetComponent<Vfx2>();

            if (vf2 != null)
            {
                vfx.Initialize(Power * mP[3].dmgCoe);
            }

            yield return new WaitForSeconds(0.5f);
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

    public Tile FindLeftTile(Tile currentTile)
    {
        Vector3 leftTilePosition = new Vector3(currentTile.transform.position.x /*- 1.65f*/, currentTile.transform.position.y, currentTile.transform.position.z);

        foreach (var item in LevelManager.Instance.Tiles)
        {
            if (Vector3.Distance(item.Value.transform.position, leftTilePosition) < 0.1f)
            {
                return item.Value;
            }
        }

        return null;
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising && !isTeleporting && !isStunned)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DelayedStun(Tower tower, float stunDuration, float delay)
    {
        yield return new WaitForSeconds(delay);
        tower.Stun(stunDuration);
    }

    private void SpawnKokoro()
    {
        Vector3 KoyoriPosition = transform.position;

        List<SpawnPoint> differentPositions = new List<SpawnPoint>();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (Mathf.Abs(KoyoriPosition.y - spawnPoint.position.y) > Mathf.Epsilon)
            {
                differentPositions.Add(spawnPoint);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, differentPositions.Count);
            SpawnPoint spawnPoint = differentPositions[randomIndex];
            differentPositions.RemoveAt(randomIndex);

            Vector3 finalSpawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, KoyoriPosition.z);
            GameObject spawnedKokoro = Instantiate(kokoroPrefab, finalSpawnPosition, Quaternion.identity);

            MeshRenderer meshRenderer = spawnedKokoro.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = spawnPoint.sortingOrder;
            }
        }
    }

    public void Pattern3()
    {
        ising = true;
        if (Random.value < mP[5].trigProb)
        {
            SetAnimation(0, AnimClip[4], false);
        }
        else
        {
            SetAnimation(0, AnimClip[5], false);
        }
    }

    public void Pattern3Throw(bool isSuccess)
    {
        levelManager.OccupiedPoints.Clear();
        List<Point> keys = new List<Point>(levelManager.Tiles.Keys.Except(levelManager.OccupiedPoints));
        for (int i = 0; i < 2; i++)
        {
            Point randomKey = keys[Random.Range(0, keys.Count)];
            levelManager.OccupiedPoints.Add(randomKey);
            keys.Remove(randomKey);

            CreatePotion(randomKey, "damaging", Plate.PlateType.Poison);
        }

        if (!isSuccess)
        {
            Point randomKey = keys[Random.Range(0, keys.Count)];
            levelManager.OccupiedPoints.Add(randomKey);

            CreatePotion(randomKey, "healing", Plate.PlateType.Heal);
        }
    }

    private void CreatePotion(Point key, string potionAnimation, Plate.PlateType plateType)
    {
        Tile tile = levelManager.Tiles[key];

        GameObject potionObject = Instantiate(potionPrefab, transform.position, Quaternion.identity);
        Potion potion = potionObject.GetComponent<Potion>();

        potion.SetAnimation(potionAnimation);

        Vector3 destination = tile.transform.position;
        float duration = 1.6667f;
        potion.MoveTo(destination, duration);

        string plateAnimation = potionAnimation;

        potion.OnAnimationEnd += () => {
            GameObject plateObject = Instantiate(platePrefab, destination, Quaternion.identity);
            Plate plate = plateObject.GetComponent<Plate>();
            if (plateObject != null)
            {
                plate.Initialize(Power * mP[4].dmgCoe, Power * mP[5].dmgCoe);
            }
            plate.type = plateType;
            plate.SetAnimation(plateAnimation);
            plate.Activate();
            plate.OnDeactivate += () => {
                levelManager.OccupiedPoints.Remove(key);
            };
        };
    }

    void Busitherock()
    {
        GameObject soundObject = new GameObject("TempSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;

        audioSource.Play();

        Destroy(soundObject, clip.length);
    }
}
