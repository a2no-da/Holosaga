using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class MikoLuck : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public Miko miko;
    public float damage;

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
        int towerLayer = LayerMask.NameToLayer("Tower");
        layerMask = (1 << towerLayer);

        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        foreach (RaycastHit2D singleHit in hitResults)
        {
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Tower"))
            {
                Tower towerScript = singleHit.collider.gameObject.GetComponentInParent<Tower>();
                if (towerScript != null)
                {
                    towerScript.ApplyBuff("미코행운");
                }
            }
        }
    }

    void Update()
    {
    }

    public void Initialize(float damage, Miko tower)
    {
        this.damage = damage;
        this.miko = tower;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "healAndBuff":
                Dama();
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
            if (singleHit.collider != null && singleHit.collider.gameObject.CompareTag("Tower"))
            {
                Tower towerScript = singleHit.collider.gameObject.GetComponentInParent<Tower>();
                if (towerScript != null)
                {
                    towerScript.Heal(damage, miko);
                    PlayAnimationAndSound(towerScript);
                }
            }
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (miko.LevelS > 2)
        {
            Vector3 toriiPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            Vector3 roadPosition = new Vector3(0, transform.position.y - 0.2f, transform.position.z);
            GameObject torii = Instantiate(miko.toriiPrefab, transform.position, Quaternion.identity);
            GameObject road = Instantiate(miko.roadPrefab, roadPosition, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void PlayAnimationAndSound(Tower tower)
    {
        Vector3 position = tower.transform.position;
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity, tower.transform);
    }
}

