using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;

public class Iusia : BOSS
{
    public GameObject BonePrefab;
    public Transform BoneSpawnPoint;
    public int hit_Limit = 1;
    private int layerMask;
    private float stunDuration = 3.0f;
    public GameObject skullPrefab;
    public List<Vector2> spawnPositions;

    public override void Start()
    {
        base.Start();
        layerMask = 1 << LayerMask.NameToLayer("Tower");
    }

    public override void Update()
    {
        base.Update();
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D hit = Physics2D.BoxCast(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        if (hit.collider == null || !hit.collider.CompareTag("Tower"))
        {
            if (!ising && !isStunned && !isTeleporting)
            {
                moveSpeed = initialMoveSpeed;
                SetAnimation(0, AnimClip[2], true);
            }
        }
        else if (hit.collider.CompareTag("Tower") && !ising)
        {
            moveSpeed = 0;
            SetAnimation(0, AnimClip[0], true);

            if (Pattern1Timer <= 0 && !isTeleporting && !isStunned)
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

        if (isStunned)
        {
            SetAnimation(0, AnimClip[0], true);
        }

        if (Pattern3Timer <= 0 && !ising && !isTeleporting && !isStunned)
        {
            ising = true;
            StartCoroutine(SpawnSkulls(AnimClip[4]));
            Pattern3Timer = Pattern3Cooldown;
        }
        else if (isStunned)
        {
            ising = false;
            Pattern3Timer = Pattern3Cooldown;
            SetAnimation(0, AnimClip[0], true);
        }

        MoveTowardsEndPoint();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower"))
        {
            targetEnemy = collider.transform.parent.gameObject;
            Tower tower = targetEnemy.GetComponent<Tower>();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower") && collider.transform.parent.gameObject == targetEnemy)
        {
            ising = false;
            targetEnemy = null;
            moveSpeed = initialMoveSpeed;
        }
    }

    public override void Attack()
    {
        Tower tower = targetEnemy.GetComponent<Tower>();
        if (tower != null && !ising)
        {
            ising = true;
            StartCoroutine(PerformAttack(tower, AnimClip[3]));
        }
    }

    public void BasicAttack()
    {
        Tower tower = targetEnemy.GetComponent<Tower>();
        if (tower != null && !ising)
        {
            ising = true;
            StartCoroutine(BasicAttack(tower, AnimClip[1]));
        }
    }

    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising && !isTeleporting)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator PerformAttack(Tower tower,
                              AnimationReferenceAsset animation)
    {
        SetAnimation(0, animation, false);

        yield return new WaitForSeconds(1.065f);

        if (targetEnemy != null)
        {
            BoneGo();
        }

        yield return new WaitForSeconds(0.935f);

        SetAnimation(0, AnimClip[0], true);

        ising = false;
    }

    void BoneGo()
    {
        GameObject BoneGo = Instantiate(BonePrefab, BoneSpawnPoint.position, Quaternion.identity);
        Bone bone = BoneGo.GetComponent<Bone>();

        if (bone != null)
        {
            bone.Initialize(Power, hit_Limit, stunDuration);
        }
    }

    private IEnumerator BasicAttack(Tower tower, AnimationReferenceAsset animation)
    {
        SetAnimation(0, animation, false);
        Tile tile = targetEnemy.GetComponentInParent<Tile>();

        yield return new WaitForSeconds(1.065f);

        if (targetEnemy != null)
        {
            targetEnemy.GetComponent<Tower>().TakeDamage(Power);

            if (tower.Health <= 0)
            {
                ising = false;
                targetEnemy = null;
                yield break;
            }

            yield return new WaitForSeconds(0.935f);

            SetAnimation(0, AnimClip[0], true);
            ising = false;
        }
    }

    private IEnumerator SpawnSkulls(AnimationReferenceAsset animation)
    {
        SetAnimation(0, animation, false);

        yield return new WaitForSeconds(0.825f);

        Vector3 luciaPosition = transform.position;

        foreach (Vector2 spawnPosition in spawnPositions)
        {
            if (Mathf.Abs(luciaPosition.y - spawnPosition.y) > Mathf.Epsilon)
            {
                Vector3 finalSpawnPosition = new Vector3(luciaPosition.x, spawnPosition.y, luciaPosition.z);
                Instantiate(skullPrefab, finalSpawnPosition, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(3.175f);

        SetAnimation(0, AnimClip[0], true);
        ising = false;
    }
}