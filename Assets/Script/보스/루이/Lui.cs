using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;

public class Lui : BOSS
{
    private LayerMask layerMask;
    public SpawnPoint[] spawnPoints;
    public float Pattern6Cooldown = 10f;
    public float Pattern6Timer = 0f;
    public GameObject TsunmirePrefab;
    public GameObject PadongPrefabP;
    public GameObject PadongPrefabP_2;
    public GameObject PadongPrefabX;
    public GameObject JangPanPrefab;
    public Transform PadongSpawnPoint;
    public GameObject Ganmo;
    public GameObject GanmoGoPrefab;
    public Transform GanmoGoSpawnPoint;
    public GameObject shieldPrefab;
    public bool shouldConsiderIsing7;
    public bool shouldIgnoreRange7;
    private GameObject shieldInstance;
    public GameObject Vfx4;
    public GameObject Vfx6;
    private List<Vf6> vf6Instances = new List<Vf6>();
    private LevelManager levelManager;
    public GameObject bloodPrefab;
    public AudioSource A3;
    public AudioSource A6;
    public AudioSource S6;
    public AudioSource bomb;

    private Vector3[] crossDirections = new Vector3[]
    {
    Vector3.up,    
    Vector3.down,  
    Vector3.left,  
    Vector3.right  
    };

    private Vector3[] diagonalDirections = new Vector3[]
    {
    new Vector3(1, 1, 0),   
    new Vector3(-1, 1, 0),  
    new Vector3(-1, -1, 0), 
    new Vector3(1, -1, 0)   
    };

    private Vector2[] positions = new Vector2[]
    {
        new Vector2(-6.6f, 0.66f),
        new Vector2(-6.6f, -3.38f),
        //new Vector2(6.6f, 0.66f),
        //new Vector2(6.6f, -3.38f)
    };

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

        Pattern6Timer = mC[6].initCool;
        Pattern6Cooldown = mC[6].cool;

        shouldConsiderIsing7 = mC[6].IsIndep == 0;
        shouldIgnoreRange7 = mC[6].IgnRange == 0;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage1_pattern1":
                Attack1();
                break;
            case "damage1_pattern3":
                A3.Play();
                Attack3_0();
                break;
            case "damage2_pattern3":
                A3.Play();
                Attack3_1();
                break;
            case "damage3_pattern3":
                A3.Play();
                Attack3_2();
                break;
            case "summon_pattern6":
                S6.Play();
                Attack6();
                break;
            case "damage1_pattern6":
                bomb.Play();
                A6.Play();
                KillerQueen();
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

        if (trackEntry.Animation.Name == AnimClip[4].name)
        {
            Destroy6();
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
            if (!isTeleporting)
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

        if (!ising && Pattern2Timer <= 0 && !isStunned && (targetEnemy != null || !shouldIgnoreRange3) && !isTeleporting)
        {
            Attack2();
            Pattern2Timer = Pattern2Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern2Timer = Pattern2Cooldown;
        }

        if (Pattern3Timer <= 0 && !ising && !isStunned && !isTeleporting && (targetEnemy != null || !shouldIgnoreRange4))
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

        if (Pattern4Timer <= 0 && !ising && !isStunned && !isTeleporting && (targetEnemy != null || !shouldIgnoreRange5))
        {
            Attack4();
            Pattern4Timer = Pattern4Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern4Timer = Pattern4Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        if (isStunned)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            KillerQueen();
            Destroy6();
        }

        if (Pattern5Timer <= 0 && !isTeleporting && !isStunned && (targetEnemy != null || !shouldIgnoreRange1))
        {
            SpawnTsunmire();
            Pattern5Timer = Pattern5Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern5Timer = Pattern5Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        if ((!ising || !shouldConsiderIsing7) && Pattern6Timer > 0 && !isStunned && !isTeleporting)
        {
            Pattern6Timer -= Time.deltaTime;
        }

        if (!ising && Pattern6Timer <= 0 && !isStunned && (targetEnemy != null || !shouldIgnoreRange7) && !isTeleporting)
        {
            Attack66();
            Pattern6Timer = Pattern6Cooldown;
        }
        else if (isStunned)
        {
            Pattern6Timer = Pattern6Cooldown;
        }

        MoveTowardsEndPoint();
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising && !isTeleporting && !isStunned)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
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
        if (!ising)
        {
            ising = true;
            SetAnimation(0, AnimClip[1], false);
        }
    }

    private void Attack1()
    {
        if (targetEnemy != null)
        {
            Tower tower = targetEnemy.GetComponent<Tower>();

            targetEnemy.GetComponent<Tower>().TakeDamage(Power * mP[2].dmgCoe);

            if (tower.Health <= 0)
            {
                ising = false;
                targetEnemy = null;
            }
        }
    }

    public override void Attack2()
    {
        Ganmo ganmo = Ganmo.GetComponent<Ganmo>();
        ganmo.GoMan();
    }

    public void GanmoGGo()
    {
        GameObject GanmouGo = Instantiate(GanmoGoPrefab, GanmoGoSpawnPoint.position, Quaternion.identity);
        GanmoF ganmo = GanmouGo.GetComponent<GanmoF>();

        if (ganmo != null)
        {
            ganmo.Initialize(Power * mP[3].dmgCoe, mP[3].speed, mP[3].hitLim);
        }
    }

    private void Pattern3Attack()
    {
        if (!ising)
        {
            ising = true;
            SetAnimation(0, AnimClip[3], false);
            Attack3_0();
        }
    }

    public void Attack3_0()
    {
        Tile currentTile = GetCurrentTile();
        Tile leftTile = FindLeftTile(currentTile);
        Vector3 spawnPosition = leftTile != null ? leftTile.transform.position : transform.position;
        GameObject jangPanObj = Instantiate(JangPanPrefab, spawnPosition, Quaternion.identity);
        JangPan jangPanScript = jangPanObj.GetComponent<JangPan>();

        if (jangPanScript != null)
        {
            jangPanScript.Initialize(Power * mP[4].dmgCoe, mP[4].speed, mP[4].hitLim, mP[4].stunTime);
        }
    }

    public void Attack3_1()
    {
        PadongGo(crossDirections[0], PadongPrefabP_2); 
        PadongGo(crossDirections[1], PadongPrefabP_2);
        PadongGo(crossDirections[2], PadongPrefabP);
        PadongGo(crossDirections[3], PadongPrefabP);
    }

    public void Attack3_2()
    {
        foreach (Vector3 dir in diagonalDirections)
        {
            PadongGo(dir, PadongPrefabX); 
        }
    }

    private void Attack4()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        List<GameObject> aliveTowers = new List<GameObject>();
        levelManager.OccupiedPoints.Clear();
        List<Point> keys = new List<Point>(levelManager.Tiles.Keys.Except(levelManager.OccupiedPoints));

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
            GameObject targetTower = aliveTowers[Random.Range(0, aliveTowers.Count)];

            Tower towerComponent = targetTower.transform.parent.GetComponent<Tower>();
            if (towerComponent != null)
            {
                towerComponent.TakeDamage(Power * mP[5].dmgCoe);
                GameObject vfxInstance = Instantiate(Vfx4, towerComponent.transform.position, Quaternion.identity, towerComponent.transform);
            }

            for (int i = 0; i < 1; i++)
            {
                Point randomKey = keys[Random.Range(0, keys.Count)];
                levelManager.OccupiedPoints.Add(randomKey);
                keys.Remove(randomKey);

                CreateBlood(randomKey, "damaging", towerComponent.transform.position);
            }
        }
    }

    protected override void Die()
    {
        Destroy6();
        base.Die();
    }

    private void CreateBlood(Point key, string bloodAnimation, Vector3 position)
    {
        Tile tile = levelManager.Tiles[key];

        GameObject bloodObject = Instantiate(bloodPrefab, position, Quaternion.identity);
        Blood blood = bloodObject.GetComponent<Blood>();
        if (blood != null)
        {
            blood.Initialize(Power * mP[5].dmgCoe);
        }

        Vector3 destination = tile.transform.position;
        float duration = 2f;
        blood.MoveTo(destination, duration);
    }

    public void Attack66()
    {
        ising = true;
        SetAnimation(0, AnimClip[4], false);
    }

    private void Attack6()
    {
        int shieldIndex = Random.Range(0, positions.Length);
        List<Point> points = new List<Point>(LevelManager.Instance.Tiles.Keys);

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == shieldIndex)
            {
                shieldInstance = Instantiate(shieldPrefab, positions[i], Quaternion.identity);
                break;
            }
        }

        Collider2D shieldCollider = shieldInstance.GetComponent<Collider2D>();

        foreach (KeyValuePair<Point, Tile> tilePair in LevelManager.Instance.Tiles)
        {
            Tile tile = tilePair.Value;
            Vector3 tilePosition = tile.transform.position;

            Point tilePoint = LevelManager.Instance.PositionToPoint(tilePosition);

            if (!shieldCollider.OverlapPoint(new Vector2(tilePosition.x, tilePosition.y)))
            {
                GameObject vfxInstance = Instantiate(Vfx6, tilePosition, Quaternion.identity);
                Vf6 vfx = vfxInstance.GetComponent<Vf6>();
                if (vfx != null)
                {
                    vfx.Initialize(Power * mP[6].dmgCoe);
                    vf6Instances.Add(vfx);
                }
            }
        }
    }

    public void KillerQueen()
    {
        /*foreach (Vf6 vfx in vf6Instances)
        {
            vfx.PlayAnimation("animation");
        }*/
    }

    private void Destroy6()
    {
        if (shieldInstance != null)
        {
            Destroy(shieldInstance);
            shieldInstance = null; 
        }
        vf6Instances.Clear();
    }

    void PadongGo(Vector3 direction, GameObject padongPrefab)
    {
        float angle = 0f;
        float angley = 0f;
        if (direction.x > 0 && direction.y == 0)
        {
            //angle = 180f;
            angley = 180f;
        }
        else if (direction.x < 0 && direction.y == 0)
        {
            angle = 0f;
        }
        else if (direction.x == 0 && direction.y > 0)
        {
            angle = -90f;
        }
        else if (direction.x == 0 && direction.y < 0)
        {
            angle = 90f;
        }
        else if (direction.x > 0 && direction.y > 0)
        {
            angle = -135f;
        }
        else if (direction.x < 0 && direction.y > 0)
        {
            angle = -45f;
        }
        else if (direction.x < 0 && direction.y < 0)
        {
            angle = 45f;
        }
        else if (direction.x > 0 && direction.y < 0)
        {
            angle = 135f;
        }

        Quaternion rotation = Quaternion.Euler(0f, angley, angle);

        GameObject PadongGo = Instantiate(padongPrefab, PadongSpawnPoint.position, Quaternion.identity);
        Padong padong = PadongGo.GetComponent<Padong>();

        Transform spriteChild = PadongGo.transform.Find("Square");
        if (spriteChild != null)
        {
            spriteChild.rotation = Quaternion.Euler(0f, angley, angle);
        }

        if (padong != null)
        {
            padong.Initialize(Power * mP[4].dmgCoe, mP[4].speed, mP[4].hitLim, mP[4].stunTime, direction);
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
        Vector3 leftTilePosition = new Vector3(currentTile.transform.position.x - 1.65f, currentTile.transform.position.y, currentTile.transform.position.z);

        foreach (var item in LevelManager.Instance.Tiles)
        {
            if (Vector3.Distance(item.Value.transform.position, leftTilePosition) < 0.1f)
            {
                return item.Value;
            }
        }

        return null;
    }

    private void SpawnTsunmire()
    {
        Vector3 IrohaPosition = transform.position;

        List<SpawnPoint> differentPositions = new List<SpawnPoint>();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (Mathf.Abs(IrohaPosition.y - spawnPoint.position.y) > Mathf.Epsilon)
            {
                differentPositions.Add(spawnPoint);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, differentPositions.Count);
            SpawnPoint spawnPoint = differentPositions[randomIndex];
            differentPositions.RemoveAt(randomIndex);

            Vector3 finalSpawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, IrohaPosition.z);
            GameObject spawnedTsunmire = Instantiate(TsunmirePrefab, finalSpawnPosition, Quaternion.identity);

            SkeletonAnimation skeletonAnimation = spawnedTsunmire.GetComponentInChildren<SkeletonAnimation>();
            if (skeletonAnimation != null)
            {
                string[] skins = { "1", "2" }; 
                string randomSkin = skins[Random.Range(0, skins.Length)]; 
                skeletonAnimation.skeleton.SetSkin(randomSkin); // 스킨 변경
                skeletonAnimation.skeleton.SetSlotsToSetupPose(); 
            }

            MeshRenderer meshRenderer = spawnedTsunmire.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = spawnPoint.sortingOrder;
            }
        }
    }
}

