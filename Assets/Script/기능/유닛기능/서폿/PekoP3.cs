using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using System.Linq;

public class PekoP3 : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public Tower tower;
    public float damage;
    public AudioSource A1;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public GameObject prefab;

    HashSet<GameObject> DamageEnemiess = new HashSet<GameObject>();

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int towerLayer = LayerMask.NameToLayer("Tower");
        layerMask = (1 << enemyLayer) | (1 << towerLayer);
    }

    void Update()
    {
        if (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public void Initialize(float damage, Tower tower)
    {
        this.damage = damage;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_buff":
                Dama();
                buff();
                A1.Play();
                break;
        }
    }

    public void Dama()
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
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = singleHit.collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null && !DamageEnemiess.Contains(singleHit.collider.gameObject))
                {
                    enemyScript.UpdateAttackingTower(tower);
                    enemyScript.TakeDamage(damage);
                    DamageEnemiess.Add(singleHit.collider.gameObject);
                }
            }
        }
    }

    public void buff()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            foreach (var t in gameManager.AllTowers.Where(t => !t.isEx && !t.isDead))
            {
                PlayAnimationAndSound(t);
                t.ApplyBuff("황금당근");
            }
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    private void PlayAnimationAndSound(Tower tower)
    {
        Vector3 position = tower.transform.position;
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity, tower.transform);
    }
}
