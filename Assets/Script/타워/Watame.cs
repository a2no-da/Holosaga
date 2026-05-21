using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Watame : Tower
{
    public GameObject notePrefab;
    public GameObject notePlatePrefab;
    public Transform noteSpawnPoint;
    public AudioSource AttackS;
    public AudioSource BuffS;
    public Vector3[] notePlateSpawnPoints;
    public GameObject sheepPrefab;
    public GameObject currentNotePlate = null;

    public override void Start()
    {
        base.Start();

        if (LevelS > 1)
        {
            myActive = true;
            Active_cooltime = myC[2].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "pattern1_damage":
                NoteGO();
                AttackS.Play();
                break;
            case "pattern2_buff":
                NotePlate();
                BuffS.Play();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[2].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
            act = false;
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                SetAnimation(0, AnimClip[2], false);
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

    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();
        SetAnimation(0, AnimClip[1], false);
        act = false;
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 1)
        {
            ising = true;
            SetAnimation(0, AnimClip[2], false);
            ActiveCooldown = Active_cooltime;
            act = true;
            ResetAct();
        }
    }

    void NoteGO()
    {
        GameObject noteGO = Instantiate(notePrefab, noteSpawnPoint.position, Quaternion.identity);
        Note note = noteGO.GetComponent<Note>();

        if (note != null)
        {
            note.Initialize(Power * myP[1].dmgCoe, myP[1].speed, myP[1].hitLim, myP[1].slowPower, myP[1].slowTime, this);
        }
    }

    void NotePlate()
    {
        Vector3 closestSpawnPoint = Vector3.zero;
        float shortestDistance = Mathf.Infinity;

        foreach (Vector3 spawnPoint in notePlateSpawnPoints)
        {
            float distanceToWatame = Vector3.Distance(transform.position, spawnPoint);
            if (distanceToWatame < shortestDistance)
            {
                shortestDistance = distanceToWatame;
                closestSpawnPoint = spawnPoint;
            }
        }

        if (closestSpawnPoint != Vector3.zero)
        {
               
            if (currentNotePlate != null)
            {
                Destroy(currentNotePlate);
            }
            currentNotePlate = Instantiate(notePlatePrefab, closestSpawnPoint, Quaternion.identity);
            Noteplate noteplate = currentNotePlate.GetComponent<Noteplate>();
           
            if (noteplate != null)
            {
                noteplate.Initialize(myP[2].slowPower, myP[2].slowTime);
            }
        }
    }
}