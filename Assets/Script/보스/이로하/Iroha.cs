using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;

public class Iroha : BOSS
{
    public GameObject GeohapBaegi;
    public GameObject SwordGiPrefab;
    public Transform SwordGiSpawnPoint;
    public int hit_Limit = 1;
    public GameObject PokobePrefab;
    public SpawnPoint[] spawnPoints;
    private LayerMask layerMask;
    private bool endd = false;
    public Transform aniTransform;
    public Transform aniTransform2;
    public SkeletonDataAsset spineAnimationData2;
    public GameObject GBPoint1;
    public GameObject GBPoint2;
    public GameObject GBPoint3;
    private Tower hitTower;
    private bool isDelaying = false;
    private float delayTime = 0.0067f;
    private float delayTimer = 0;
    public AudioSource A1;
    public AudioSource A2s;
    public AudioSource A2d;
    public AudioSource A4s;
    public AudioSource A4d;

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
                A1.Play();
                break;
            case "iroha_pattern2_start":
                A2s.Play();
                break;
            case "damage_pattern2":
                Attack2();
                A2d.Play();
                break;
            case "iroha_pattern4_start":
                A4s.Play();
                break;
            case "damage_teleport_pattern4":
                Pattern4Attacked();
                A4d.Play();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name && trackEntry.Animation.Name != AnimClip[2].name && trackEntry.Animation.Name != AnimClip[6].name)
        {
            ising = false;
            endd = false;
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
            if (!ising && !isStunned && !isTeleporting && !endd)
            {
                if(!isSlowEffectActive)
                {
                    moveSpeed = initialMoveSpeed;
                }
                SetAnimation(0, AnimClip[2], true);
            }
        }
        else if (hit.collider != null && (hit.collider.CompareTag("Tower") && !ising))
        {
            if (!endd && !isTeleporting)
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
            if (!endd && !isTeleporting)
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

        if (Pattern4Timer <= 0 && !isTeleporting && !isStunned & !ising && (targetEnemy != null || !shouldIgnoreRange5))
        {      
            endd = true;
            Pattern4Attack();
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
            SetAnimation(0, AnimClip[0], true);
        }

        if (Pattern5Timer <= 0 && !isTeleporting && !isStunned && (targetEnemy != null || !shouldIgnoreRange1))
        {
            SpawnPokobes();
            Pattern5Timer = Pattern5Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern5Timer = Pattern5Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        if (isDelaying)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayTime)
            {
                delayTimer = 0;
                isDelaying = false;
                AfterDelay();
            }
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

    public override void Attack()
    { 
        if (!ising)
        {
            ising = true;
            SetAnimation(0, AnimClip[3], false);
        }
    }

    public override void Attack2()
    {
        PlayAttackEffectAnimation(aniTransform.position, spineAnimationData2, "iroha_pattern2", 20);
        SwordGiGo();
    }

    void SwordGiGo()
    {
        GameObject SwordGiGo = Instantiate(SwordGiPrefab, SwordGiSpawnPoint.position, Quaternion.identity);
        SwordGi swordgi = SwordGiGo.GetComponent<SwordGi>();

        if (swordgi != null)
        {
            swordgi.Initialize(Power * mP[3].dmgCoe, mP[3].speed, mP[3].hitLim);
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

    public void Pattern4Attack()
    {
        if (!ising)
        {
            ising = true;
            SetAnimation(0, AnimClip[4], false);
        }
    }

    private void Pattern4Attacked()
    {
        hitTower = null;

        Vector2 raycastStart = new Vector2(transform.position.x - (1.65f * 3f) / 2f, transform.position.y + 0.21f);
        Vector2 boxSize = new Vector2(1.65f * 3f, 0.2f); 
        float raycastDistance = 0f; 

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, transform.right, raycastDistance, layerMask);

        Vector2 bottomLeft = raycastStart - new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 topLeft = raycastStart - new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(bottomLeft, bottomRight, Color.red, 2f);
        Debug.DrawLine(bottomRight, topRight, Color.red, 2f);
        Debug.DrawLine(topRight, topLeft, Color.red, 2f);
        Debug.DrawLine(topLeft, bottomLeft, Color.red, 2f);

        if (hit.collider != null && hit.collider.CompareTag("Tower"))
        {
            hitTower = hit.collider.transform.parent.GetComponent<Tower>();
        }

        PlayAttackEffectAnimation(aniTransform.position, spineAnimationData2, "iroha_pattern4_end", 20);
        isDelaying = true;

        if (hit.collider != null && hit.collider.CompareTag("Tower"))
        {
            hitTower = hit.collider.transform.parent.GetComponent<Tower>();
        }

        PlayAttackEffectAnimation(aniTransform.position, spineAnimationData2, "iroha_pattern4_end", 20);
        isDelaying = true;
    }

    private void AfterDelay()
    {
        if (hitTower != null)
        {
            transform.position = new Vector3(hitTower.transform.position.x - 1.65f, transform.position.y, transform.position.z);
            hitTower.TakeDamage(Power * mP[5].dmgCoe);
            hitTower.Stun(mP[5].stunTime);
            PlayAttackEffectAnimation(aniTransform.position, spineAnimationData2, "iroha_pattern4_start", 20);
        }
        else
        {
            transform.position += new Vector3(-1.65f * 3, 0, 0);
            PlayAttackEffectAnimation(aniTransform.position, spineAnimationData2, "iroha_pattern4_start", 20);
        }
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising && !isTeleporting && !endd && !isStunned)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private void SpawnPokobes()
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
            GameObject spawnedPokobe = Instantiate(PokobePrefab, finalSpawnPosition, Quaternion.identity);

            MeshRenderer meshRenderer = spawnedPokobe.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = spawnPoint.sortingOrder;
            }
        }
    }

    private void PlayAttackEffectAnimation(Vector3 position, SkeletonDataAsset animationData, string animationName, int sortingOrder)
    {
        GameObject animationObject = new GameObject("AttackAnimation");
        animationObject.transform.position = position;
        animationObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        SkeletonAnimation skeletonAnimation = animationObject.AddComponent<SkeletonAnimation>();
        skeletonAnimation.skeletonDataAsset = animationData;
        skeletonAnimation.initialSkinName = "default";
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
        skeletonAnimation.GetComponent<Renderer>().sortingOrder = sortingOrder;

        skeletonAnimation.AnimationState.Complete += (trackEntry) => {
            if (trackEntry.TrackIndex == 0)
            {
                Destroy(animationObject);
            }
        };
    }
}
