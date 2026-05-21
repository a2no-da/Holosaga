using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Lamy : Tower
{
    public GameObject fd;
    public float cool1;
    public float Mcool1;
    private Vector2 newwOffset;
    public bool isDrink;
    private float drinkDuration = 10f;
    public float drinkTimer = 0f;
    public GameObject P3;
    private bool muteki;
    public AudioSource A1;
    public AudioSource A2;

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        fd.GetComponent<LamyFd>().Initialize(Power * myP[1].dmgCoe, Power * myP[2].dmgCoe, Power * myP[3].dmgCoe, Power * myP[4].dmgCoe, this);

        cool1 = Mcool1;
        Mcool1 = myC[1].cool;
        P2_cooltime = myC[2].cool;
        P2Cooldown = P2_cooltime;
        Active_cooltime = myC[3].cool;
        ActiveCooldown = Active_cooltime;
        inActive_cooltime = Active_cooltime;

        Act();

        if (LevelS > 2)
        {
            myActive = true;
        }

        if (LevelS > 1)
        {
            P2T.gameObject.SetActive(true);
            P2_cooltime = myC[2].cool;
            P2T.text = myC[2].cool.ToString();
        }
    }

    public override void Update()
    {
        base.Update();

        if (LevelS > 1)
        {
            if (!isDead && !muteki)
            {
                if (!ising && P2Cooldown > 0 && !isStunned)
                {
                    P2Cooldown -= Time.deltaTime;
                    P2T.text = ((int)P2Cooldown).ToString();
                }

                if (!ising && P2Cooldown <= 0 && !isStunned)
                {
                    ising = true;
                    P2Cooldown = P2_cooltime;
                    Drink();
                }
                else if (isStunned)
                {
                    ising = false;
                    P2Cooldown = P2_cooltime;
                    P2T.text = ((int)P2Cooldown).ToString();
                }
            }
        }

        if(muteki)
        {
            skeletonAnimation.timeScale = 1;
        }

        if (isDrink)
        {
            drinkTimer += Time.deltaTime;

            if (drinkTimer >= drinkDuration)
            {
                toCollider();
                isDrink = false;
                drinkTimer = 0f;
            }
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage_pattern1":
                cbomb();
                A1.Play();
                break;
            case "change_pattern2":
                A2.Play();
                Drinking();
                break;
            case "damage_pattern3":
                kongkong();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[3].name)
        {
            ising = false;
            muteki = false;
            SetAnimation(0, AnimClip[0], true);
            act = false;
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                SetAnimation(0, AnimClip[3], false);
                act = false;
            }
            else
            {
                SetAnimation(0, AnimClip[0], true);
                ising = false;
                act = false;
            }
        }
    }

    public void Drink()
    {
        SetAnimation(0, AnimClip[2], false);
    }

    public void Drinking()
    {
        oriCollider();
        isDrink = true;
        fd.GetComponent<LamyFd>().isDrink = true;
        fd.GetComponent<LamyFd>().drinkTimer = 0;
        fd.GetComponent<LamyFd>().skeletonAnimation.AnimationState.SetAnimation(0, "pattern2_field", true);
    }

    public override void TakeDamage(float damage)
    {
        if(muteki) return;
        base.TakeDamage(damage);
    }

    public override void Stun(float stunDuration)
    {
        if (muteki) return;
        base.Stun(stunDuration);
    }

    public void kongkong()
    {
        muteki = true;
        GameObject p3p3 = Instantiate(P3, new Vector3(0, 0, 0), Quaternion.identity);
        Lamyp3 lamyp3 = p3p3.GetComponent<Lamyp3>();

        if (lamyp3 != null)
        {
            lamyp3.Initialize(0.5f, Power * myP[5].dmgCoe, this);
        }
    }

    public override void toCollider()
    {
        BoxCollider2D towerCollider = GetComponent<BoxCollider2D>();
        Tile towerTile = GetComponentInParent<Tile>();

        float xSize = RangeX * 1.65f;
        float ySize = RangeY * 2.02f;

        if (RangeY % 2 == 0)
        {
            ySize = 2.02f;
        }

        if (xSize == 0)
        {
            xSize = 0.82f;
        }

        if (RangeY == 0f)
        {
            ySize = 2.02f;
        }

        Vector2 newSize = new Vector2(xSize, ySize);
        towerCollider.size = newSize;

        Vector3 localTowerPosition = transform.InverseTransformPoint(towerTile.transform.position);

        newwOffset = new Vector2(localTowerPosition.x, localTowerPosition.y + (1.01f) + (centerY * 2.02f));

        towerCollider.offset = newwOffset;
    }

    public void oriCollider()
    {
        BoxCollider2D towerCollider = GetComponent<BoxCollider2D>();
        Tile towerTile = GetComponentInParent<Tile>();

        float xSize = 5 * 1.65f;
        float ySize = 5 * 2.02f;

        if (RangeY % 2 == 0)
        {
            ySize = 2.02f;
        }

        if (xSize == 0)
        {
            xSize = 0.82f;
        }

        if (RangeY == 0f)
        {
            ySize = 2.02f;
        }

        Vector2 newSize = new Vector2(xSize, ySize);
        towerCollider.size = newSize;

        Vector3 localTowerPosition = transform.InverseTransformPoint(towerTile.transform.position);

        newwOffset = new Vector2(localTowerPosition.x, localTowerPosition.y + (1.01f) + (centerY * 2.02f));

        towerCollider.offset = newwOffset;
    }

    public override void Attack()
    {
        base.Attack();

        SetAnimation(0, AnimClip[1], false);
    }

    public void cbomb()
    {
        fd.GetComponent<LamyFd>().boom();
    }

    protected override void Die()
    {
        base.Die();
        fd.SetActive(false);
    }

    public override void Respawn()
    {
        base.Respawn();
        fd.SetActive(true);
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation(0, AnimClip[3], false);
            act = true;
            ResetAct();
        }
    }
}
