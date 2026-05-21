using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Disasters : MonoBehaviour
{
    public int num;  
    public int Times;
    private SkeletonAnimation skeletonAnimation;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    public Vector2 raycastDirection = Vector2.right;
    public Vector2[] raycastStartOffset;
    public float raycastDistance = 0f;
    public Vector2[] boxSize;
    private int layerMask;
    private bool Oi;
    private float timer = 0f;
    private float delay = 0.2f;
    public AudioSource A1;
    public AudioSource Dkoko;
    public AudioSource LLL;
    public bool St;
    public float per;
    private MeshRenderer meshRenderer;
    public AudioClip[] sound;

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Tower");
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        meshRenderer = GetComponent<MeshRenderer>();

        switch (num)
        {
            case 0:
                per = 0.2f;
                A1.clip = sound[0];
                Times = 3;
                skeletonAnimation.AnimationState.SetAnimation(0, "shion", false); 
                break;
            case 1:
                per = 0.5f;
                A1.clip = sound[1];
                skeletonAnimation.AnimationState.SetAnimation(0, "koko", false); 
                break;
            case 2:
                per = 0.3f;
                A1.clip = sound[2];
                skeletonAnimation.AnimationState.SetAnimation(0, "laplus", false); 
                break;
        }
    }

    public void Initialize(int num)
    {
        this.num = num;
    }

    void Update()
    {
        if (Oi)
        {
            CheckRaycast();
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                Oi = false;
                timer = 0;
            }
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_koko":
                A1.Play();
                Oi = true;
                St = false;
                break;
            case "damage_laplus":
                A1.Play();
                Oi = true;
                St = false;
                meshRenderer.sortingOrder = 5;
                break;
            case "damage_robocco":
                A1.Play();
                Oi = true;
                St = true;
                break;
            case "disaster_SFX_koko1":
                Dkoko.Play();
                break;
            case "laplus_VFX_pattern6_summon":
                LLL.Play();
                break;
        }
    }

    private void CheckRaycast()
    {
        Vector2 raycastStart = new Vector2(transform.position.x + raycastStartOffset[num].x, transform.position.y + raycastStartOffset[num].y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastStart, boxSize[num], 0f, raycastDirection, raycastDistance, layerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize[num].x / 2, boxSize[num].y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize[num].x / 2, boxSize[num].y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize[num].x / 2, -boxSize[num].y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize[num].x / 2, -boxSize[num].y / 2);

        Debug.DrawLine(topLeft, topRight, Color.green);
        Debug.DrawLine(topRight, bottomRight, Color.green);
        Debug.DrawLine(bottomRight, bottomLeft, Color.green);
        Debug.DrawLine(bottomLeft, topLeft, Color.green);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))
            {
                GameObject parentObject = hit.collider.gameObject.transform.parent.gameObject;
                Tower tower = parentObject.GetComponent<Tower>();

                if (tower != null && !hitTargets.Contains(parentObject))
                {
                    hitTargets.Add(parentObject);
                    tower.TakeDamage(tower.MaxHealth * per);
                    if (St)
                    {
                        tower.Stun(Times);
                    }
                }
            }
        }
    }
}
