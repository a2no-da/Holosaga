using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBag : BOSS
{
    public int SandHp;
    public GameObject yeonchul;
    public int kaisu = 0;
    public bool saigo = false;
    private float lastHealthThreshold;
    private bool enddddd = false;

    public override void Start()
    {
        base.Start();

        float multiplier = 0f;

        switch (GameManager.instance.mainStage)
        {
            case 0:
                multiplier = 0f; 
                break;
            case 1:
                multiplier = 1f; 
                break;
            case 2:
                multiplier = 2f; 
                break;
            case 3:
                multiplier = 3f; 
                break;
            case 4:
                multiplier = 4f; 
                break;
            default:
                multiplier = 0f; 
                break;
        }

        MaxHealth = SandHp + (9000 * multiplier);
        Health = MaxHealth;
        snab = true;
        Power = 0;
        GameManager.Instance.UpdateHealthTexts(Health, MaxHealth);
        experienceReward = 2000;
        lastHealthThreshold = MaxHealth;
        saigo = false;

        if(GameManager.instance.mainStage == 4 && GameManager.instance.subStage == 4)
        {
            saigo = true;
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Teleport()
    {
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[1].name)
        {
        }
    }

    protected override void Die()
    {
        base.Die();

        for (int i = 0; i < kaisu; i++)
        {
            GiftBox();
        }

        if (saigo)
        {
            enddddd = true;
            SetAnimation(0, AnimClip[1], false);
            GameManager.instance.samco();
        }
        else
        {
            GameObject deathEffect = Instantiate(deathvfxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public override void TakeDamage(float damage)
    {
        if (!enddddd)
        {
            base.TakeDamage(damage);
        }
    }
}