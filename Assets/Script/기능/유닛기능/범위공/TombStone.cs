using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TombStone : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Tower tower;
    public GameObject Mob1;
    public GameObject Mob2;
    public int mNum;

    private float timer = 0f; 
    private float spawnTimer = 0f; 
    private bool hasDied = false;
    public AudioSource a1;

    public void Initialize(Tower tower, int num)
    {
        this.tower = tower;
        this.mNum = num;
    }

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        skeletonAnimation.AnimationState.SetAnimation(0, "summon" + mNum, false);
        SP();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "olie_VFX_pattern3_die":
                a1.Play();
                break;
        }
    }

    public void SP()
    {
        float localY = transform.localPosition.y;

        if (Mathf.Approximately(localY, 0.8600003f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = -2;
        }
        else if (Mathf.Approximately(localY, -1.16f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 0;
        }
        else if (Mathf.Approximately(localY, -3.18f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 2;
        }
        else if (Mathf.Approximately(localY, -5.2f))
        {
            skeletonAnimation.GetComponent<Renderer>().sortingOrder = 4;
        }
    }

    void Update()
    {
        if (!hasDied)
        {
            if (!GameManager.Instance.isBreakSta)
            {
                timer += Time.deltaTime;
                if (timer >= 240f)
                {
                    hasDied = true;
                    skeletonAnimation.AnimationState.SetAnimation(0, "die" + mNum, false);
                }

                spawnTimer += Time.deltaTime;
                if (spawnTimer >= 10f)
                {
                    SpawnMob();
                    spawnTimer = 0f;
                }
            }


            if (tower.isDead)
            {
                hasDied = true;
                skeletonAnimation.AnimationState.SetAnimation(0, "die" + mNum, false);
            }
        }
    }

    private void SpawnMob()
    {
        GameObject mobToSpawn;

        if (mNum == 1)
        {
            mobToSpawn = Mob1;
        }
        else if (mNum == 2)
        {
            mobToSpawn = Mob2;
        }
        else
        {
            return; 
        }

        GameObject zomb = Instantiate(mobToSpawn, transform.position, Quaternion.identity);
        Zom zomdareComponent = zomb.GetComponent<Zom>();

        if (zomdareComponent != null)
        {
            zomdareComponent.tower = tower;
            zomdareComponent.mN = mNum;
        }
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "die" + mNum)
        {
            Destroy(gameObject);
        }
        else if (trackEntry.Animation.Name == "summon" + mNum)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle" + mNum, true);
        }
    }
}
