using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;

public class DeathSense : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Kali kali;
    public AudioSource audioSource;
    public GameObject P2effectPrefab;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 0f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    private int layerMask;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    public void Initialize(Kali kali)
    {
        this.kali = kali;
    }

    void Start()
    {
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        SP();
        smP();
    }

    public void smP()
    {
        if (P2effectPrefab != null)
        {
            Instantiate(P2effectPrefab, transform.position, transform.rotation);
        }
    }

    public void SP()
    {
        float localY = transform.position.y;

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

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "pattern1_damage1":
                FireBullet();
                audioSource.Play();
                break;
        }
    }

    void FireBullet()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        DeathTama deathTama = bulletGO.GetComponent<DeathTama>();

        if (deathTama != null)
        {
            deathTama.Initialize(kali.Power * kali.myP[5].dmgCoe, kali.myP[5].speed, kali.myP[5].hitLim, kali);
        }
    }


    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
