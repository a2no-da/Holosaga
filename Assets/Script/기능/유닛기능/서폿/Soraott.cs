using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class Soraott : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;
    public Sora sora;
    private bool succ = false;
    private float timer = 0f;
    private bool isDestroyed = false;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        layerMask = (1 << LayerMask.NameToLayer("Tower"));
        SetSkinBasedOnTnum(sora.Tnum);
        Destroy(this.gameObject, 5f);
    }

    public void Initialize(Sora tower)
    {
        this.sora = tower;
    }

    void Update()
    {
        if (!succ)
        {
            Corre();
        }

        if (!isDestroyed) 
        {
            timer += Time.deltaTime; 

            if (timer >= 5f) 
            {
                Destroy(this.gameObject);
                isDestroyed = true; 
            }
            else if (succ) 
            {
                isDestroyed = true; 
            }
        }
    }

    void Corre()
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

                if (hitObject.CompareTag("Tower"))
                {
                    GameObject parentObject = hitObject.transform.parent.gameObject;
                    Tower tower = parentObject.GetComponent<Tower>();

                    if (tower == sora)
                    {
                        succ = true;

                        if (sora.Tnum == 4)
                        {
                            sora.UPartne();
                            sora.ActivateKuma();
                            sora.Sakana(true);
                        }
                        else
                        {
                            sora.Sakana(false);
                        }

                        sora.Tnum += 1;
                        sora.Elemental(sora.Tnum);
                        if (sora.Tnum > 4)
                        {
                            sora.Tnum = 1;
                            sora.occupiedPositions.Clear();
                        }
          
                        skeletonAnimation.AnimationState.ClearTracks();
                        skeletonAnimation.AnimationState.SetAnimation(0, "end", false);
                        return; 
                    }
                }
            }
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "end")
        {
            Destroy(gameObject);
            return;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, "idle", false);
    }

    private void SetSkinBasedOnTnum(int tnum)
    {
        string skinName = tnum.ToString(); 
        skeletonAnimation.Skeleton.SetSkin(skinName); 
        skeletonAnimation.Skeleton.SetToSetupPose(); 
    }
}
