using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Fauna : Tower
{
    public GameObject seed;
    public List<Tower> Towers = new List<Tower>();
    public GameObject JangPF;
    public GameObject TSeed;
    public GameObject hasirama;
    public GameObject buff3;
    public bool okkk;

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        if (LevelS > 2)
        {
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
            myActive = true;
        }
        Act();
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            act = true;
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[3], true);

            ResetAct();
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "grow_pattern1":
                PPP();
                break;
            case "heal_pattern1":
                PPP();
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.44f, transform.position.z);
                GameObject Jan = Instantiate(JangPF, spawnPosition, Quaternion.identity);

                FaunaF faunaF = Jan.GetComponent<FaunaF>();

                if (faunaF != null)
                {
                    faunaF.Initialize(Power * myP[1].dmgCoe, this, "healField");
                    faunaF.PlayAni("healField");
                }

                FaunaSeed existingSeed = null;

                foreach (Transform child in this.transform)
                {
                    if (child.CompareTag("Seed"))
                    {
                        existingSeed = child.GetComponent<FaunaSeed>();
                        break;
                    }
                }

                existingSeed.PlayAni("heal" + existingSeed.seedLev);
                okkk = false;
                break;
            case "summon_pattern3":
                Mokdun();
                break;
        }
    }

    void PPP()
    {
        Grow(this, "heal");
        List<Tower> towersCopy = new List<Tower>(Towers);
        
        for (int i = 0; i < towersCopy.Count; i++)
        {
            Tower tower = towersCopy[i];
            string seedn = null;
            if (tower != null)
            {
                if (tower.isEx) { return; }
                switch (tower.dicton.role)
                {
                    case DictonRole.Projectiler:
                        seedn = "speed";
                        break;
                    case DictonRole.Supporter:
                        seedn = "hpup";
                        break;
                    case DictonRole.Attacker:
                        seedn = "power";
                        break;
                    default:
                        break;
                }

                Grow(tower, seedn);
            }
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[3].name)
        {
            ising = false;
            act = false;
            SetAnimation(0, AnimClip[0], true);
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                SetAnimation(0, AnimClip[3], false);
                act = false;
                ising = false;
            }
            else
            {
                SetAnimation(0, AnimClip[0], true);
                act = false;
                ising = false;
            }
        }
    }

    public override void Attack()
    {
        base.Attack();
        ising = true;

        if(okkk)
        {
            okkk = false;
            SetAnimation(0, AnimClip[2], true);
        }
        else
        {
            SetAnimation(0, AnimClip[1], true);
        }
    }

    public override void Update()
    {
        base.Update();

        for (int i = Towers.Count - 1; i >= 0; i--)
        {
            Tower tower = Towers[i];

            if (tower.isDead)
            {
                FaunaSeed existingSeed = null;

                foreach (Transform child in tower.transform)
                {
                    if (child.CompareTag("Seed"))
                    {
                        existingSeed = child.GetComponent<FaunaSeed>();

                        if (existingSeed != null)
                        {
                            Destroy(existingSeed.gameObject);
                        }
                        break;
                    }
                }

                Towers.RemoveAt(i);
            }
        }

        if (isDead)
        {
            FaunaSeed existingSeed = null;

            foreach (Transform child in this.transform)
            {
                if (child.CompareTag("Seed"))
                {
                    existingSeed = child.GetComponent<FaunaSeed>();

                    if (existingSeed != null)
                    {
                        Destroy(existingSeed.gameObject);
                    }
                    break;
                }
            }
        }
    }

    public void Grow(Tower tower, string jongpa)
    {
        bool hasSeed = false;
        FaunaSeed existingSeed = null;

        foreach (Transform child in tower.transform)
        {
            if (child.CompareTag("Seed"))
            {
                hasSeed = true;
                existingSeed = child.GetComponent<FaunaSeed>();
                break;
            }
        }

        if (hasSeed)
        {
            if (existingSeed != null)
            {
                existingSeed.seedLev += 1;

                if (jongpa == null)
                {
                    jongpa = existingSeed.seedName;
                }

                if (jongpa == "heal")
                {
                    if (existingSeed.seedLev >= 3)
                    {
                        okkk = true;
                    }
                }

                if (existingSeed.seedLev >= 4)
                {
                    existingSeed.seedLev = 0;
                    if (jongpa == "heal")
                    {
                    }
                    else
                    {
                        Towers.Remove(tower);
                        Vector3 spawnPosition = new Vector3(tower.transform.position.x, tower.transform.position.y - 0.22f, transform.position.z);
                        GameObject Jan = Instantiate(JangPF, spawnPosition, Quaternion.identity);

                        FaunaF faunaF = Jan.GetComponent<FaunaF>();

                        if (faunaF != null)
                        {
                            faunaF.Initialize(Power * myP[1].dmgCoe, this, jongpa + "Field");
                            faunaF.PlayAni(jongpa + "Field");
                        }
                    }
                }
                else
                {
                    existingSeed.PlayAni(jongpa + existingSeed.seedLev);
                }

                if (jongpa != "heal")
                {
                    existingSeed.PlayAni(jongpa + existingSeed.seedLev);
                }
                //Debug.Log(tower + " + " + jongpa + existingSeed.seedLev);
            }
        }
        else
        {
            GameObject newSeed = Instantiate(seed, tower.transform.position, Quaternion.identity);
            newSeed.transform.SetParent(tower.transform);
            newSeed.tag = "Seed";

            FaunaSeed newFaunaSeed = newSeed.GetComponent<FaunaSeed>();
            if (newFaunaSeed != null)
            {
                newFaunaSeed.seedLev = 1;
                newFaunaSeed.seedName = jongpa;
                newFaunaSeed.PlayAni(jongpa + 1);
            }
            //Debug.Log(tower + " + " + jongpa + newFaunaSeed.seedLev);
        }
    }

    public void PaJong()
    {
        float animationDuration = 1; 

        foreach (Tower tower in Towers)
        {
            if (tower == this) return;
            if (tower != null)
            {
                if(tower.isEx) { return; }
                Vector3 towerPosition = tower.transform.position;

                GameObject tSeedInstance = Instantiate(TSeed, transform.position, Quaternion.identity);
                Speeds speeds = tSeedInstance.GetComponent<Speeds>();

                float distance = Vector3.Distance(transform.position, towerPosition);

                float speed = distance / animationDuration;
                string seedn = null;

                switch (tower.dicton.role) 
                {
                    case DictonRole.Projectiler:
                        seedn = "speed";
                        break;
                    case DictonRole.Supporter:
                        seedn = "hpup";
                        break;
                    case DictonRole.Attacker:
                        seedn = "power";
                        break;
                    default:
                        break;
                }

                speeds.SetAnimation(seedn + "Seed");
                speeds.OnAnimationEnd += () =>
                {
                    Grow(tower, seedn); 
                };

                speeds.MoveTo(towerPosition, animationDuration);
            }
        }
    }

    public void Mokdun()
    {
        foreach (Tower tower in GameManager.Instance.AllTowers)
        {
            if (tower != null)
            {
                BuffManager.Instance.ApplyBuff(
                    tower.gameObject,
                    "파우나패턴3",
                    tower.myB[28].duration,
                    tower.myB[28].powerUp,
                    tower.myB[28].speedUp,
                    tower.myB[28].HpUp
                );

                GameObject buffInstance = Instantiate(buff3, tower.transform.position, Quaternion.identity, tower.transform);
            }
        }

        Vector3 spawnPosition = new Vector3(transform.position.x + 1.65f, 0f, transform.position.z);
        GameObject hasi = Instantiate(hasirama, spawnPosition, Quaternion.identity);

        MD md = hasi.GetComponent<MD>();

        if (md != null)
        {
            md.Initialize(myP[3].addForce, this);
        }
    }
}
