using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System.Linq;
using TMPro;

public class Laplus : BOSS
{
    private bool LastBoss;
    public float Pattern6Cooldown = 10f;
    public float Pattern6Timer = 0f;

    public bool attackMode;
    private LayerMask layerMask;

    private List<int> patternOrder = new List<int>();
    private int currentPatternIndex = 0;
    private float patternCooldown = 2f;
    private float lastPatternTime = 0f;
    private bool isWaiting = false;
    private bool isPatternComplete = false;
    private float waitTime = 2f;
    private float waitTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isCooldown = false;
    private bool isReCooldown = false;
    private bool isLastPattern = false;
    public SkeletonAnimation barrier;
    public TMP_Text DcountText;
    public TMP_Text itaiText;
    public AudioSource MainD;
    public AudioSource Potal;
    public GameObject Ptimer1;
    public GameObject Vfx2;
    public List<Vector3> spawnPositions;
    public GameObject laplaceTilePrefab;
    private List<GameObject> spawnedLaplaceTiles = new List<GameObject>();
    private int pattern3RepeatCount = 0;
    private const int maxPattern3Repeats = 3;
    private bool Pattern3ing = false;
    public GameObject Vfx4;
    public Transform Vfx4Point;
    public GameObject Vfx5;
    private HashSet<int> usedTileIndices = new HashSet<int>();
    private List<Tile> allTiles;
    public MeshRenderer meshRenderer;
    public GameObject Vfx6;
    public GameObject Vfx62;
    private Vector3 originalPosition;

    private BoxCollider2D boxCollider;
    private Rigidbody2D myRigidbody2D;

    public override void Start()
    {
        base.Start();
        LastBoss = false;
        attackMode = true;
        isPatternComplete = false;
        SetAnimation(0, AnimClip[3], false);
        layerMask = LayerMask.GetMask("Tower");
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        meshRenderer.sortingOrder = 0;
        originalPosition = this.transform.position;
        GameManager.Instance.isDCo = true;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "main_damage":
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnMonsterReached(GameManager.Instance.DCo * 30);
                    GameManager.Instance.DCo = 0;
                }
                break;
            case "damage_pattern2":
                Sp();
                break;
            case "trigger_pattern3":
                ST();
                break;
            case "correct_pattern3":
                DT();
                break;
            case "trigger_pattern4":
                SB();
                break;
            case "damage_pattern5":
                SC();
                break;
            case "trigger_pattern6_1":
                SH();
                break;
            case "trigger_pattern6_2":
                SL();
                break;
            case "teleport_pattern6_1":
                T1();
                break;
            case "teleport_pattern6_2":
                T2(); 
                break;
            case "laplus_mainDamage":
                MainD.Play();
                break;
            case "laplus_portal":
                Potal.Play();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[2].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);

            ShufflePatterns();
            isReCooldown = false;
            currentPatternIndex = 0;
        }

        if (trackEntry.Animation.Name == AnimClip[3].name)
        {
            barrier.gameObject.SetActive(true);
            barrier.AnimationState.SetAnimation(0, "summon", false);
            ShufflePatterns();
            SetAnimation(0, AnimClip[0], true);
            isPatternComplete = true;
            LastBoss = true;
            lastPatternTime = Time.time;
        }

        if (trackEntry.Animation.Name == AnimClip[4].name || trackEntry.Animation.Name == AnimClip[6].name ||
          trackEntry.Animation.Name == AnimClip[7].name || trackEntry.Animation.Name == AnimClip[8].name)
        {
            isPatternComplete = true;
            SetAnimation(0, AnimClip[0], true);

            if (isLastPattern)
            {
                StartCooldown();
                isLastPattern = false;
            }
        }

        if (trackEntry.Animation.Name == AnimClip[9].name && GameManager.instance.isinfinite)
        {
            Destroy(gameObject);
        }

        if (trackEntry.Animation.Name == AnimClip[5].name && !Pattern3ing)
        {
            if (pattern3RepeatCount < maxPattern3Repeats)
            {
                pattern3RepeatCount++;
                SetAnimation(0, AnimClip[0], true);
                SetAnimation(0, AnimClip[5], false);
            }
            else
            {
                pattern3RepeatCount = 0;
                isPatternComplete = true;
                SetAnimation(0, AnimClip[0], true);

                if (isLastPattern)
                {
                    StartCooldown();
                    isLastPattern = false;
                }
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if(isDead) return;

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

        if (!ising && Pattern1Timer > 0 && attackMode && NoTower())
        {
            Pattern1Timer -= Time.deltaTime;
        }
        else
        {
            Pattern1Timer = Pattern1Cooldown;
        }

        if (Pattern1Timer <= 0 && attackMode && NoTower())
        {
            Pattern1Timer = Pattern1Cooldown;
            Pattern1();
        }

        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isCooldown = false;

                if (GameManager.Instance.DCo <= 0)
                {
                    SetAnimation(0, AnimClip[0], true);
                    ShufflePatterns();
                    isReCooldown = false;
                }
                else
                {
                    SetAnimation(0, AnimClip[2], false);
                }
                barrier.gameObject.SetActive(true);
                barrier.AnimationState.SetAnimation(0, "summon", false);
                attackMode = true;
            }
        }

        if (!isWaiting && !isReCooldown && LastBoss && isPatternComplete)
        {
            DecreasePatternTimers();
        }

        if (LastBoss)
        {
            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    isWaiting = false;
                    lastPatternTime = Time.time;
                }
            }
            else if (isPatternComplete && Time.time - lastPatternTime >= patternCooldown && IsCurrentPatternReady() && !isReCooldown)
            {
                ExecuteNextPattern();
                isPatternComplete = false;
                isWaiting = true;
                waitTimer = waitTime;
            }
        }

        if (CheckTileTowerContact() && Pattern3ing)
        {
            if (pattern3RepeatCount < maxPattern3Repeats)
            {
                pattern3RepeatCount++;
                SetAnimation(0, AnimClip[0], true);
                SetAnimation(0, AnimClip[5], false);
                Pattern3ing = false;
            }
            else
            {
                pattern3RepeatCount = 0;
                isPatternComplete = true;
                SetAnimation(0, AnimClip[0], true);

                Pattern3ing = false;
            }
        }

        if (GameManager.Instance != null)
        {
            DcountText.text = GameManager.Instance.DCo.ToString();
        }

        if(attackMode)
        {
            itaiText.text = "30 %";
        }
        else
        {
            itaiText.text = "150 %";
        }
    }

    private void StartCooldown()
    {
        isCooldown = true;
        isReCooldown = true;
        attackMode = false;
        SetAnimation(0, AnimClip[1], false);
        cooldownTimer = 10f;
        barrier.AnimationState.SetAnimation(0, "remove", false);
    }

    private void DecreasePatternTimers()
    {
        if (isDead) { return; }

        switch (patternOrder[currentPatternIndex])
        {
            case 0:
                if (Pattern2Timer > 0)
                {
                    Pattern2Timer -= Time.deltaTime;
                }
                break;
            case 1:
                if (Pattern3Timer > 0)
                {
                    Pattern3Timer -= Time.deltaTime;
                }
                break;
            case 2:
                if (Pattern4Timer > 0)
                {
                    Pattern4Timer -= Time.deltaTime;
                }
                break;
            case 3:
                if (Pattern5Timer > 0)
                {
                    Pattern5Timer -= Time.deltaTime;
                }
                break;
            case 4:
                if (Pattern6Timer > 0)
                {
                    Pattern6Timer -= Time.deltaTime;
                }
                break;
        }
    }

    public override void TakeDamage(float damage)
    {
        if (attackMode)
        {
            damage *= 0.3f;
        }
        else
        {
            damage *= 1.5f;
        }
        base.TakeDamage(damage);
    }


    private bool IsCurrentPatternReady()
    {
        if (isDead) { return false; }

        switch (patternOrder[currentPatternIndex])
        {
            case 0:
                return Pattern2Timer <= 0;
            case 1:
                return Pattern3Timer <= 0;
            case 2:
                return Pattern4Timer <= 0;
            case 3:
                return Pattern5Timer <= 0;
            case 4:
                return Pattern6Timer <= 0;
            default:
                return false;
        }
    }

    public void Pattern1()
    {
        if (!attackMode) { return; }
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        List<GameObject> aliveTowers = new List<GameObject>();

        foreach (GameObject tower in towers)
        {
            Tower towerComponent = tower.transform.parent.GetComponent<Tower>();
            if (towerComponent != null && !towerComponent.isDead && !IsPtimer1Attached(towerComponent))
            {
                aliveTowers.Add(tower);
            }
        }

        if (aliveTowers.Count > 0)
        {
            GameObject targetTower = aliveTowers[Random.Range(0, aliveTowers.Count)];

            Tower towerComponent = targetTower.transform.parent.GetComponent<Tower>();
            if (towerComponent != null)
            {
                GameObject newObject = Instantiate(Ptimer1, towerComponent.transform.position, Quaternion.identity);
                LaTimer laTimer = newObject.GetComponent<LaTimer>();
                newObject.transform.SetParent(towerComponent.transform);
                laTimer.Initialize(Power * mP[2].dmgCoe);
            }
        }
    }

    private bool NoTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        List<GameObject> aliveTowers = new List<GameObject>();

        foreach (GameObject tower in towers)
        {
            if (tower.transform.parent != null)
            {
                Tower towerComponent = tower.transform.parent.GetComponent<Tower>();
                if (towerComponent != null && !towerComponent.isDead && !IsPtimer1Attached(towerComponent))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsPtimer1Attached(Tower tower)
    {
        return tower.GetComponentInChildren<LaTimer>() != null;
    }

    public void Pattern2()
    {
        SetAnimation(0, AnimClip[4], false);
        Pattern2Timer = Pattern2Cooldown;
    }

    public void Sp()
    {
        int randomIndex = Random.Range(0, spawnPositions.Count);
        Vector3 randomSpawnPosition = spawnPositions[randomIndex];

        GameObject vf2 = Instantiate(Vfx2, randomSpawnPosition, Quaternion.identity);
        LaV2 laV2 = vf2.GetComponent<LaV2>();

        if (laV2 != null)
        {
            laV2.Initialize(Power * mP[2].dmgCoe);
        }
    }

    public void Pattern3()
    {
        if (!Pattern3ing)
        {
            Pattern3ing = true;
            SetAnimation(0, AnimClip[5], false);
            Pattern3Timer = Pattern3Cooldown;
            pattern3RepeatCount++;
        }
    }

    public void ST()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            return;
        }

        List<Tower> validTowers = gameManager.AllTowers.Where(tower => !tower.isEx).ToList();
        int numTowers = validTowers.Count;
        List<Vector3> spawnPositions = GetRandomPositions(numTowers);

        for (int i = 0; i < spawnPositions.Count; i++)
        {
            Vector3 spawnPosition = spawnPositions[i];
            GameObject laplaceTile = Instantiate(laplaceTilePrefab, spawnPosition, Quaternion.identity);
            spawnedLaplaceTiles.Add(laplaceTile);

            LaV3 laV3Component = laplaceTile.GetComponent<LaV3>();
            if (laV3Component != null)
            {
                laV3Component.mynum = i;
            }

            SkeletonAnimation skeletonAnimation = laplaceTile.GetComponent<SkeletonAnimation>();
            if (skeletonAnimation != null)
            {
                string skinName = (i + 1).ToString();
                skeletonAnimation.Skeleton.SetSkin(skinName);
                skeletonAnimation.Skeleton.SetSlotsToSetupPose();
            }
        }
        Pattern3ing = true;
    }

    private bool CheckTileTowerContact()
    {
        if(isDead) {  return false; }
        if (Pattern3ing)
        {
            GameManager gameManager = GameManager.Instance;
            bool allTowersInContact = true;
            List<GameObject> tilesToDestroy = new List<GameObject>();

            foreach (Tower tower in gameManager.AllTowers)
            {
                if (tower.isEx) continue;

                int towerIndex = gameManager.AllTowers.IndexOf(tower);
                bool towerInContact = false;

                foreach (GameObject laplaceTile in spawnedLaplaceTiles)
                {
                    LaV3 laV3Component = laplaceTile.GetComponent<LaV3>();
                    if (laV3Component != null && laV3Component.mynum == towerIndex)
                    {
                        if (laV3Component.IsTowerInContact(tower.gameObject))
                        {
                            tilesToDestroy.Add(laplaceTile);
                            towerInContact = true;
                            break;
                        }
                    }
                }
                if (!towerInContact)
                {
                    allTowersInContact = false;
                    break;
                }
            }

            if (allTowersInContact)
            {
                foreach (GameObject laplaceTile in tilesToDestroy)
                {
                    if (laplaceTile != null)
                    {
                        SkeletonAnimation skeletonAnimation = laplaceTile.GetComponent<SkeletonAnimation>();
                        LaV3 laV3Component = laplaceTile.GetComponent<LaV3>();
                        laV3Component.ed = true;
                        laV3Component.isRemoving = true;
                        spawnedLaplaceTiles.Remove(laplaceTile);
                        skeletonAnimation.AnimationState.SetAnimation(0, "remove", false).Complete += (trackEntry) =>
                        {
                            Destroy(laplaceTile);
                        };
                    }
                }
            }
            else
            {
                return false;
            }

            return allTowersInContact;
        }
        return false;
    }

    private List<Vector3> GetRandomPositions(int numPositions)
    {
        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        List<Vector3> randomPositions = new List<Vector3>();

        for (int i = 0; i < numPositions; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allTiles.Count);
            Tile randomTile = allTiles[randomIndex];
            Vector3 spawnPosition = randomTile.transform.position;

            randomPositions.Add(spawnPosition);
            allTiles.RemoveAt(randomIndex);
        }

        return randomPositions;
    }

    public void DT()
    {
        if (!isDead)
        {
            GameManager gameManager = GameManager.Instance;

            foreach (Tower tower in gameManager.AllTowers)
            {
                if (tower.isEx) continue;

                bool isInContact = false;
                int towerIndex = gameManager.AllTowers.IndexOf(tower);

                foreach (GameObject laplaceTile in spawnedLaplaceTiles)
                {
                    LaV3 laV3Component = laplaceTile.GetComponent<LaV3>();
                    if (laV3Component != null && laV3Component.mynum == towerIndex)
                    {
                        if (laV3Component.IsTowerInContact(tower.gameObject))
                        {
                            isInContact = true;
                            break;
                        }
                    }
                }

                if (!isInContact)
                {
                    if (tower.isDead == false)
                    {
                        tower.TakeDamage(9999);
                    }
                }
            }

            List<GameObject> tilesToRemove = new List<GameObject>(spawnedLaplaceTiles);
            foreach (GameObject laplaceTile in tilesToRemove)
            {
                LaV3 laV3Component = laplaceTile.GetComponent<LaV3>();
                laV3Component.ed = true;
                SkeletonAnimation skeletonAnimation = laplaceTile.GetComponent<SkeletonAnimation>();
                skeletonAnimation.AnimationState.SetAnimation(0, "remove", false).Complete += (trackEntry) =>
                {
                    if (laplaceTile != null)
                    {
                        spawnedLaplaceTiles.Remove(laplaceTile);
                        Destroy(laplaceTile);
                    }
                };
            }
            Pattern3ing = false;
        }
        else
        {
            List<GameObject> tilesToRemove = new List<GameObject>(spawnedLaplaceTiles);
            foreach (GameObject laplaceTile in tilesToRemove)
            {
                LaV3 laV3Component = laplaceTile.GetComponent<LaV3>();
                laV3Component.ed = true;
                SkeletonAnimation skeletonAnimation = laplaceTile.GetComponent<SkeletonAnimation>();
                skeletonAnimation.AnimationState.SetAnimation(0, "remove", false).Complete += (trackEntry) =>
                {
                    if (laplaceTile != null)
                    {
                        spawnedLaplaceTiles.Remove(laplaceTile);
                        Destroy(laplaceTile);
                    }
                };
            }
            Pattern3ing = false;

            return;
        }
    }

    public void Pattern4()
    {
        SetAnimation(0, AnimClip[6], false);
        Pattern4Timer = Pattern4Cooldown;
    }

    public void SB()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            return;
        }

        List<Tower> validTowers = gameManager.AllTowers.Where(tower => !tower.isEx).ToList();
        int numValidTowers = validTowers.Count;

        for (int i = 0; i < numValidTowers; i++)
        {
            GameObject laplaceBlack = Instantiate(Vfx4, Vfx4Point.position, Quaternion.identity);
            LaV4 laV4Component = laplaceBlack.GetComponent<LaV4>(); 

            if (laV4Component != null)
            {
                laV4Component.mynum = i;
                laV4Component.Initialize(Power * mP[4].dmgCoe, validTowers[i]);
            }
        }
    }

    public void Pattern5()
    {
        SetAnimation(0, AnimClip[7], false);
        Pattern5Timer = Pattern5Cooldown;
    }

    public void SC()
    {
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < allTiles.Count; i++)
        {
            if (!usedTileIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count < 4)
        {
            usedTileIndices.Clear();
            availableIndices.Clear();
            for (int i = 0; i < allTiles.Count; i++)
            {
                availableIndices.Add(i);
            }
        }

        List<int> selectedIndices = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
            selectedIndices.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        foreach (int index in selectedIndices)
        {
            Tile selectedTile = allTiles[index];
            Vector3 spawnPosition = selectedTile.transform.position;

            GameObject vf5 = Instantiate(Vfx5, spawnPosition, Quaternion.identity);
            LaV5 laV5 = vf5.GetComponent<LaV5>();

            if (laV5 != null)
            {
                laV5.Initialize(Power * mP[5].dmgCoe, mP[5].stunTime);
            }

            usedTileIndices.Add(index);
        }
    }

    public void Pattern6()
    {
        SetAnimation(0, AnimClip[8], false);
        Pattern6Timer = Pattern6Cooldown;
    }

    public void SH()
    {
        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        List<Tile> filteredTiles = allTiles.Where(tile => tile.y == 1 || tile.y == 2).ToList();

        if (filteredTiles.Count > 0)
        {
            Tile randomTile = filteredTiles[Random.Range(0, filteredTiles.Count)];
            GameObject spawnedVf6 = Instantiate(Vfx6, Vector3.zero, Quaternion.identity);
            spawnedVf6.transform.SetParent(randomTile.transform);
            spawnedVf6.transform.localPosition = Vector3.zero;

            LaV61 laV61 = spawnedVf6.GetComponent<LaV61>();

            if (laV61 != null)
            {
                laV61.Initialize(Power * mP[6].dmgCoe);
            }
        }
    }

    public void SL()
    {
        GameObject spawnedVf62 = Instantiate(Vfx62, Vector3.zero, Quaternion.identity);
        spawnedVf62.transform.position = new Vector3(0, -1.38f, 0);

        LaV62 laV62 = spawnedVf62.GetComponent<LaV62>();

        if (laV62 != null)
        {
            laV62.Initialize(Power * mP[7].dmgCoe);
        }
    }

    public void T1()
    {
        this.transform.position = new Vector3(0, 0.6600003f, 0);
    }

    public void T2()
    {
        this.transform.position = originalPosition;
    }

    private void ExecuteNextPattern()
    {
        switch (patternOrder[currentPatternIndex])
        {
            case 0:
                Pattern2();
                break;
            case 1:
                Pattern3();
                break;
            case 2:
                Pattern4();
                break;
            case 3:
                Pattern5();
                break;
            case 4:
                Pattern6();
                break;
        }

        currentPatternIndex++;
        if (currentPatternIndex >= patternOrder.Count)
        {
            isLastPattern = true;
            currentPatternIndex = 0;
        }
    }

    private void ShufflePatterns()
    {
        List<int> allPatterns = new List<int> { 0, 1, 2, 3, 4 };
        patternOrder.Clear();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, allPatterns.Count);
            patternOrder.Add(allPatterns[randomIndex]);
            allPatterns.RemoveAt(randomIndex);
        }
    }

    protected override void Die()
    {
        base.Die();
        isDead = true;

        DT();

        boxCollider = GetComponent<BoxCollider2D>();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        if (boxCollider != null) Destroy(boxCollider);
        if (myRigidbody2D != null) Destroy(myRigidbody2D);

        SetAnimation(0, AnimClip[9], false);
        barrier.AnimationState.SetAnimation(0, "remove", false);
    }
}

