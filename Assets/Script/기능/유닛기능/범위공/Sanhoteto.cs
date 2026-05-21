using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Sanhoteto : MonoBehaviour
{
    public Fubuki fubuki;
    public int Power;
    public int H;
    public float speed;

    public SkeletonAnimation skeletonAnimation;
    public string named;
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    HashSet<GameObject> DamageEnemies = new HashSet<GameObject>();

    public void Initialize(Fubuki fubuki, float Power, float Speed, int Hit, string NM)
    {
        this.fubuki = fubuki;
        this.Power = (int)Power;
        this.H = Hit;
        this.speed = Speed;
        this.named = NM;
    }

    public void ChangeSkin(string skinName, SkeletonAnimation skeletonAnimation)
    {
        if (string.IsNullOrEmpty(skinName)) return;
        var skeletonData = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true);
        var skin = skeletonData.FindSkin(skinName);
        if (skin == null) return;
        skeletonAnimation.initialSkinName = skinName;
        skeletonAnimation.Skeleton.SetSkin(skin);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
    }

    void Start()
    {
        DestroyAfterSeconds(8f);
        ChangeSkin(named, skeletonAnimation);
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        //SP();
    }

    public void SP()
    {
        float localY = transform.localPosition.y;

        if (Mathf.Approximately(localY, 0.8600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(localY, -1.16f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(localY, -3.18f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(localY, -5.2f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (H <= 0)
        {
            Destroy(gameObject);
        }

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

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemyScript != null && !DamageEnemies.Contains(hit.collider.gameObject))
            {
                if (H > 0)
                {
                    enemyScript.UpdateAttackingTower(fubuki);
                    enemyScript.TakeDamage(Power);
                    H--;
                }
                DamageEnemies.Add(hit.collider.gameObject);
            }
        }
    }

    void DestroyAfterSeconds(float seconds)
    {
        Destroy(gameObject, seconds);
    }
}
