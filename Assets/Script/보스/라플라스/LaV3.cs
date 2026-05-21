using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class LaV3 : MonoBehaviour
{
    public int mynum;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    private SkeletonAnimation skeletonAnimation;
    public bool ed;
    public bool isRemoving = false;

    public AudioSource spn;
    public AudioSource cor;

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "spawn")
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }

        if (trackEntry.Animation.Name == "remove")
        {
            isRemoving = true;
            hitTargets.Clear();
            Destroy(gameObject);
        }
        else
        {
            isRemoving = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isRemoving) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Tower") || other.gameObject.layer == LayerMask.NameToLayer("DeTower"))
        {
            hitTargets.Add(other.transform.parent.gameObject);

            Tower tower = other.gameObject.transform.parent.GetComponent<Tower>();
            if (tower != null)
            {
                int towerIndex = GameManager.Instance.AllTowers.IndexOf(tower);

                if (towerIndex == mynum && !ed)
                {
                    cor.Play();
                    skeletonAnimation.AnimationState.SetAnimation(0, "correct", false);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isRemoving) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Tower") || other.gameObject.layer == LayerMask.NameToLayer("DeTower"))
        {
            hitTargets.Remove(other.transform.parent.gameObject);

            Tower tower = other.gameObject.transform.parent.GetComponent<Tower>();
            if (tower != null)
            {
                int towerIndex = GameManager.Instance.AllTowers.IndexOf(tower);

                if (towerIndex == mynum && !ed)
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
                }
            }
        }
    }

    public bool IsTowerInContact(GameObject tower)
    {
        if(isRemoving)
        {
            hitTargets.Clear();
        }
        bool result = hitTargets.Contains(tower);
        return result;
    }

    void Start()
    {
        spn.Play();
        ed = false;
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
    }

    private void HandleAnimationEvent(TrackEntry trackEntry, Event e)
    {

    }
}
