using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Gura : Tower
{
    public SkeletonDataAsset spineAnimationData1; 
    public GameObject explosionEffect;
    public GameObject wavePrefab; 
    public Transform waveSpawnPoint;
    public GameObject wavePoint1;
    public GameObject wavePoint2;
    public GameObject wavePoint3;

    private int remainingCount;
    private int remainingCount2;

    public override void Start()
    {
        base.Start();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2T.text = (myP[2].trigCount - 1).ToString();
        }

        if (LevelS > 2)
        {
            P3T.gameObject.SetActive(true);
            P3T.text = (myP[3].trigCount - 1).ToString();
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                AttackEnemiesInRange(Power);

                if (LevelS > 1)
                {
                    attackCount1++;
                    attackCount2++;

                    remainingCount = (myP[2].trigCount - 1) - attackCount1;
                    P2T.text = remainingCount.ToString();
                    remainingCount2 = (myP[3].trigCount - 1) - attackCount2;
                    P3T.text = remainingCount2.ToString();

                    if (attackCount1 >= myP[2].trigCount)
                    {
                        attackCount1 = 0;
                        remainingCount = (myP[2].trigCount - 1) - attackCount1;
                        P2T.text = remainingCount.ToString();
                        PlayExplosionEffect();
                    }

                    if (LevelS > 2 && attackCount2 >= myP[3].trigCount)
                    {
                        attackCount2 = 0;
                        remainingCount2 = (myP[3].trigCount - 1) - attackCount2;
                        P3T.text = remainingCount2.ToString();
                        FireWaveProjectile();
                    }
                }
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name)
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

        SetAnimation(0, AnimClip[1], false);
    }

    private IEnumerator PlayAttackEffectAnimation(Vector3 position, SkeletonDataAsset animationData, string animationName, int sortingOrder) 
    {
        GameObject animationObject = new GameObject("AttackAnimation"); 
        animationObject.transform.position = position;

        SkeletonAnimation skeletonAnimation = animationObject.AddComponent<SkeletonAnimation>();
        skeletonAnimation.skeletonDataAsset = animationData; 
        skeletonAnimation.initialSkinName = "default";
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
        skeletonAnimation.GetComponent<Renderer>().sortingOrder = sortingOrder;

        LifeSpan lifeSpan = animationObject.AddComponent<LifeSpan>();
        lifeSpan.duration = 0.6f;

        yield return new WaitForSeconds(0.6f / animationSpeed);
    }

    private void FireWaveProjectile()
    {
        if (isDead) {  return; }
        GameObject waveProjectile = Instantiate(wavePrefab, waveSpawnPoint.position, Quaternion.identity);
        Wave wave = waveProjectile.GetComponent<Wave>(); 

        if (wave != null)
        {
            wave.Initialize(Power * myP[3].dmgCoe, this);
            wave.GetComponent<Renderer>().sortingOrder = 24;
        }
    }

    private void PlayExplosionEffect()
    {
        if (isDead) { return; }

        GameObject effect1 = Instantiate(explosionEffect, wavePoint1.transform.position, Quaternion.identity);
        GameObject effect2 = Instantiate(explosionEffect, wavePoint2.transform.position, Quaternion.identity);
        GameObject effect3 = Instantiate(explosionEffect, wavePoint3.transform.position, Quaternion.identity);

        effect1.GetComponent<Renderer>().sortingOrder = 20;
        effect2.GetComponent<Renderer>().sortingOrder = 21;
        effect3.GetComponent<Renderer>().sortingOrder = 23;

        effect1.AddComponent<LifeSpan>().duration = 0.583f;
        effect2.AddComponent<LifeSpan>().duration = 0.583f;
        effect3.AddComponent<LifeSpan>().duration = 0.583f;

        ExplosionEffect explosionEffect1 = effect1.AddComponent<ExplosionEffect>();
        ExplosionEffect explosionEffect2 = effect2.AddComponent<ExplosionEffect>();
        ExplosionEffect explosionEffect3 = effect3.AddComponent<ExplosionEffect>();

        explosionEffect1.gura = this;
        explosionEffect1.damage = Power * myP[2].dmgCoe;
        explosionEffect2.gura = this;
        explosionEffect2.damage = Power * myP[2].dmgCoe;
        explosionEffect3.gura = this;
        explosionEffect3.damage = Power * myP[2].dmgCoe;
    }

    void AttackEnemiesInRange(float damage = 0)
    {
        StartCoroutine(
            PlayAttackEffectAnimation(aniTransform.position, spineAnimationData1, "gura_VFX_Upgrade1", 23));
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = enemiesInRange[i];

            if (enemyObject == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyObject == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            if (enemyScript != null)
            {
                enemyScript.UpdateAttackingTower(this);
                enemyScript.TakeDamage(Power * myP[1].dmgCoe);
            }
        }
    }
}
