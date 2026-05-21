using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;

public class Luna : Tower
{
    public GameObject[] lunaP1;
    public GameObject lunaP2;

    public float sCooldown;
    private bool LunaTaimu;

    public GameObject LuKnight;
    private float spawnInterval = 0.2f;
    private float spawnTimer = 0f; 
    private List<Tile> selectedTiles;
    private int currentSpawnIndex = 0;

    public AudioSource S1;
    public AudioSource S2;
    public AudioSource S3;

    private int remainingCount;

    public override void Start()
    {
        base.Start();

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2_cooltime = myC[2].cool;
            P2T.text = myC[2].cool.ToString();
        }

        if (LevelS > 2)
        {
            P3T.gameObject.SetActive(true);
            P3T.text = (10 - 1).ToString();
        }
        Act();
        sCooldown = P2_cooltime;
    }

    public override void Update()
    {
        base.Update();

        if (!LunaTaimu)
        {
            if (sCooldown > 0 && !isStunned && LevelS > 1)
            {
                sCooldown -= Time.deltaTime;
                P2T.text = ((int)sCooldown).ToString();
            }

            if (sCooldown <= 0 && !isStunned && enemiesInRange.Count > 0 && LevelS > 1 && !ising)
            {
                ising = true;
                SetAnimation(0, AnimClip[3], false);
                sCooldown = P2_cooltime;
            }
            else if (isStunned)
            {
                sCooldown = P2_cooltime;
            }
        }
        else
        {
            sCooldown = P2_cooltime;
            P2T.text = ((int)sCooldown).ToString();
        }

        if (P2Cooldown > 0 && LevelS > 2 && LunaTaimu)
        {
            P2Cooldown -= Time.deltaTime;
        }

        if (P2Cooldown <= 0 && LevelS > 2 && LunaTaimu)
        {
            LunaTaimu = false;
            P2Cooldown = P2_cooltime;
        }

        if(!LunaTaimu)
        {
            P2Cooldown = P2_cooltime;
        }

        if (selectedTiles != null && currentSpawnIndex < selectedTiles.Count)
        {
            spawnTimer += Time.deltaTime; 

            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f; 

                Tile tile = selectedTiles[currentSpawnIndex];
                if (tile != null)
                {
                    GameObject luKnightInstance = Instantiate(LuKnight, tile.transform.position, Quaternion.identity);
                    luKnightInstance.transform.SetParent(tile.transform);
                    luKnightInstance.GetComponent<LunaP22>().Initialize(Power * myP[3].dmgCoe, Power * myP[4].dmgCoe, myP[4].stunTime, this);
                }

                currentSpawnIndex++; 
            }
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "summon_pattern1":
                SpawnP1();
                remainingCount = (10 - 1) - attackCount1;
                P3T.text = remainingCount.ToString();
                break;
            case "luna_pattern2_sfx":
                S2.Play();
                break;
            case "summon_pattern2":
                SpawnP2();
                break;
            case "summon_magicCandy":
                SpawnP4();
                S3.Play();
                attackCount1 = 0;
                remainingCount = (10 - 1) - attackCount1;
                P3T.text = remainingCount.ToString();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            SetAnimation(0, AnimClip[0], true);
            ising = false;
        }
    }

    public override void Attack()
    {
        if(!LunaTaimu)
        {
            base.Attack();

            List<Tile> validTiles = GetValidTiles();
            if (validTiles.Count < 1)
            {
                SetAnimation(0, AnimClip[0], true);
                attackCooldown = 0;
                ising = false;
                act = false;
                return;
            }

            if (LevelS > 2)
            {
                attackCount1 += 1;
            }

            if (attackCount1 >= 10)//myP[2].trigCount)
            {
                SetAnimation(0, AnimClip[2], false);
                attackCount1 = 0;
            }
            else
            {
                SetAnimation(0, AnimClip[1], false);
            }
            act = false;
        }
        else
        {
            if (!isStunned && enemiesInRange.Count > 0)
            {
                base.Attack();
                SetAnimation(0, AnimClip[3], false);
            }
            else
            {
                SetAnimation(0, AnimClip[0], true);
                attackCooldown = 0;
                ising = false;
                act = false;
                return;
            }
        }
    }

    private List<Tile> GetValidTiles()
    {
        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        List<Tile> validTiles = allTiles.Where(t => t.x >= 0 && t.x <= 2 && t.GetComponentInChildren<LunaP1>() == null && t.GetComponentInChildren<LunaCandy>() == null).ToList();
        return validTiles;
    }

    public void SpawnP1()
    {
        List<Tile> validTiles = GetValidTiles();
        if (validTiles.Count < 1)
        {
            return;
        }

        Tile selectedTile = validTiles.OrderBy(x => Random.value).First();

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, lunaP1.Length);
        } while (randomIndex == 3);

        GameObject lunaInstance = Instantiate(lunaP1[randomIndex], selectedTile.transform.position, Quaternion.identity);
        lunaInstance.transform.SetParent(selectedTile.transform);

        LunaP1 lunaComponent = lunaInstance.GetComponent<LunaP1>();
        lunaComponent.Initialize(Power * myP[1].dmgCoe, this);
    }

    public void SpawnP4()
    {
        List<Tile> validTiles = GetValidTiles();
        if (validTiles.Count < 1)
        {
            return;
        }

        Tile selectedTile = validTiles.OrderBy(x => Random.value).First();
        GameObject lunaInstance = Instantiate(lunaP1[3], selectedTile.transform.position, Quaternion.identity);
        lunaInstance.transform.SetParent(selectedTile.transform);

        LunaCandy lunaComponent = lunaInstance.GetComponent<LunaCandy>();
        lunaComponent.Initialize(Power * myP[1].dmgCoe, this);
    }

    public void SpawnP2()
    {
        if (lunaP2 != null)
        {
            GameObject LunaP2 = Instantiate(lunaP2, transform.position, transform.rotation);
            LunaP2.GetComponent<LunaP21>().Initialize(this, Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim, Power * myP[3].dmgCoe, Power * myP[4].dmgCoe, myP[4].stunTime);
        }
    }

    public void LunaTime()
    {
        ApplyBuff("루나타임");
        ising = true;
        SetAnimation(0, AnimClip[4], false);
        LunaTaimu = true;
        attackCooldown = 0;

        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);

        if (allTiles.Count < 10)
        {
            return;
        }

        selectedTiles = new List<Tile>(); 
        while (selectedTiles.Count < 10)
        {
            Tile randomTile = allTiles[Random.Range(0, allTiles.Count)];
            if (!selectedTiles.Contains(randomTile)) 
            {
                selectedTiles.Add(randomTile);
            }
        }

        currentSpawnIndex = 0; 
    }
}
