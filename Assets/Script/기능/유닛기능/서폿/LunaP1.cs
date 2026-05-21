 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class LunaP1 : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public string myName;
    public GameObject Eprefab;
    public GameObject Hprefab;
    private LayerMask layerMask;
    private int index;
    private int Power;
    private bool byb;
    public AudioSource Eat;
    public Tower towr;

    void Start()
    {
        byb = false;
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        layerMask = LayerMask.GetMask("Tower");
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        SP();
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "summon")
        {
            byb = true;
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", false);
        }

        if (trackEntry.Animation.Name == "get" || trackEntry.Animation.Name == "idle")
        {
            byb = false;
            Destroy(gameObject);
        }
    }

    public void Initialize(float Power, Tower tower)
    {
        this.Power = (int)Power;
        this.towr = tower;
    }

    public void SP()
    {
        float localY = transform.parent.position.y;

        if (Mathf.Approximately(localY, 0.6600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(localY, -1.36f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(localY, -3.38f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(localY, -5.4f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    void Update()
    {
        if (byb)
        {
            Vector2 raycastStart = new Vector2(transform.position.x, transform.position.y + 1f);
            Vector2 boxSize = new Vector2(1.65f, 2.02f);
            Vector2 direction = Vector2.right;
            float raycastDistance = 0f;

            RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, direction, raycastDistance, layerMask);

            Vector2 bottomLeft = raycastStart - new Vector2(boxSize.x / 2, boxSize.y / 2);
            Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);
            Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
            Vector2 topLeft = raycastStart - new Vector2(boxSize.x / 2, -boxSize.y / 2);

            Debug.DrawLine(bottomLeft, bottomRight, Color.red, 2f);
            Debug.DrawLine(bottomRight, topRight, Color.red, 2f);
            Debug.DrawLine(topRight, topLeft, Color.red, 2f);
            Debug.DrawLine(topLeft, bottomLeft, Color.red, 2f);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
                {
                    Tower tower = hit.collider.GetComponentInParent<Tower>();

                    if (tower != null)
                    {
                        skeletonAnimation.AnimationState.SetAnimation(0, "get", false);
                        Eat.Play();
                        tower.ApplyBuff(myName);
                        tower.Heal(Power, towr);
                        PlayAnimationAndSound(tower);
                        byb = false;
                    }
                }
            }
        }
    }

    private void PlayAnimationAndSound(Tower tower)
    {
        if(Eprefab != null)
        {
            Instantiate(Eprefab, Vector3.zero, Quaternion.identity, tower.transform);
        }
        Instantiate(Hprefab, Vector3.zero, Quaternion.identity, tower.transform);
    }
}
