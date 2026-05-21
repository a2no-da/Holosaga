using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Marine : Tower
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public SkeletonDataAsset spineAnimationData;
    public AudioClip fireSound; 
    private AudioSource audioSource;
    private int L;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                FireBullet(L);
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name && trackEntry.Animation.Name != "retire")
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();

        if (LevelS > 2)
        {
            SetAnimation(0, AnimClip[3], false);
            L = 3;
        }
        else if (LevelS > 1)
        {
            SetAnimation(0, AnimClip[2], false);
            L = 2;
        }
        else
        {
            SetAnimation(0, AnimClip[1], false);
            L = 1;
        }
    }

    void FireBullet(int L)
    {
        audioSource.PlayOneShot(fireSound);
        PlayAttackEffectAnimation(aniTransform.position);
        GameObject bulletGO = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(Power * myP[L].dmgCoe, myP[L].speed, myP[L].hitLim, this);
        }
    }

    private void PlayAttackEffectAnimation(Vector3 position)
    {
        GameObject animationObject = new GameObject("AttackAnimation");
        animationObject.transform.position = position;

        SkeletonAnimation skeletonAnimation = animationObject.AddComponent<SkeletonAnimation>();
        skeletonAnimation.skeletonDataAsset = spineAnimationData;
        skeletonAnimation.initialSkinName = "default";
        skeletonAnimation.AnimationState.SetAnimation(0, "animation", false);
        skeletonAnimation.GetComponent<Renderer>().sortingOrder = 10;

        LifeSpan lifeSpan = animationObject.AddComponent<LifeSpan>();
        lifeSpan.duration = 0.54f / animationSpeed;
    }
}