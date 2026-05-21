using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Noteplate : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public float slow;
    public float slowT;
    public AudioSource AttackS;

    void Start()
    {
        layerMask = (1 << LayerMask.NameToLayer("Tower")) | (1 << LayerMask.NameToLayer("Enemy"));
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    public void Initialize(float Slow, float SlowT)
    {
        this.slow = Slow;
        this.slowT = SlowT;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "watame_field":
                AttackS.Play();
                TBuff();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void TBuff()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset.x, transform.position.y + raycastStartOffset.y);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 lowerLeft = raycastStart - boxSize / 2;
        Vector2 upperLeft = lowerLeft + new Vector2(0, boxSize.y);
        Vector2 upperRight = lowerLeft + boxSize;
        Vector2 lowerRight = lowerLeft + new Vector2(boxSize.x, 0);
        HashSet<GameObject> currentHitTargets = new HashSet<GameObject>();

        Debug.DrawLine(lowerLeft, upperLeft, Color.red, 2f);
        Debug.DrawLine(upperLeft, upperRight, Color.red, 2f);
        Debug.DrawLine(upperRight, lowerRight, Color.red, 2f);
        Debug.DrawLine(lowerRight, lowerLeft, Color.red, 2f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                currentHitTargets.Add(hitObject);

                if (hitObject.CompareTag("Tower") && !hitObject.transform.parent.gameObject.name.Contains("Watame(Clone)"))
                {
                    ApplyBuffToTower(hitObject);
                }

                if (hitObject.CompareTag("Enemy"))
                {
                    Enemy enemyScript = hitObject.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.SlowEffect(slow, slowT);
                    }
                }
            }
        }
    }

    void ApplyBuffToTower(GameObject hitObject)
    {
        if (hitObject.CompareTag("Tower") && !hitObject.transform.parent.gameObject.name.Contains("Watame(Clone)"))
        {
            GameObject parentObject = hitObject.transform.parent.gameObject;
            Tower tower = parentObject.GetComponent<Tower>();
            if (tower != null && !tower.isEx)
            {
                tower.ApplyBuff("와타장판");
            }
        }
    }

    void RemoveBuffFromTower(GameObject hitObject)
    {
        if (hitObject != null)
        {
            if (hitObject.CompareTag("Tower"))
            {
                GameObject parentObject = hitObject.transform.parent.gameObject;
                Tower tower = parentObject.GetComponent<Tower>();

                if (tower != null)
                {
                    tower.RemoveBuff("와타장판");
                }
            }
        }
    }
}
