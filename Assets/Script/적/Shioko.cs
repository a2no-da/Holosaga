using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shioko : Enemy
{
    public bool Stopu = false;
    public GameObject Vfx;

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "damage":
                SpawnVFX();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[1].name)
        {
            ising = false;
            SetAnimation(0, AnimClip[0], true);
        }
    }

    public override void Update()
    {
        base.Update();

        if (targetEnemy == null && !kabedong && !Stopu)
        {
            MoveTowardsEndPoint();
        }

        if (transform.position.x <= 6.5f)
        {
            Stopu = true;
        }

        if (!ising && attackCooldown <= 0 && !isStunned && Stopu)
        {
            ising = true;
            attackCooldown = Pattern_cooltime;
            Attack();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower"))
        {
            if (collider.transform.parent != null)
            {
                targetEnemy = collider.transform.parent.gameObject;
                Tower tower = targetEnemy.GetComponent<Tower>();
            }
        }

        if (collider.gameObject.CompareTag("Kabe"))
        {
            kabedong = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Tower"))
        {
            if (collider.transform.parent != null && collider.transform.parent.gameObject == targetEnemy)
            {
                ising = false;
                targetEnemy = null;
                MoveTowardsEndPoint();
            }
        }

        if (collider.gameObject.CompareTag("Kabe"))
        {
            kabedong = false;
        }
    }

    private void SpawnVFX()
    {
        Tile currentTile = GetCurrentTile();
        Tile leftTile = FindLeftTile(currentTile);

        Vector3 spawnPosition = leftTile != null ? leftTile.transform.position : transform.position;

        List<Tile> leftTiles = new List<Tile>();
        for (int i = 1; i <= 9; i++)
        {
            Vector3 position = new Vector3(spawnPosition.x - (1.65f * i), spawnPosition.y, spawnPosition.z);
            Tile tile = FindTileAtPosition(position);
            if (tile != null)
            {
                leftTiles.Add(tile);
            }
        }

        if (leftTiles.Count >= 4)
        {
            List<Tile> selectableTiles = leftTiles.GetRange(3, Mathf.Min(6, leftTiles.Count - 3));
            Tile randomTile = selectableTiles[Random.Range(0, selectableTiles.Count)];

            GameObject vf2 = Instantiate(Vfx, randomTile.transform.position, Quaternion.identity);
            Kaminari kamin = vf2.GetComponent<Kaminari>();

            if (kamin != null)
            {
                kamin.Initialize(Power * mP[1].dmgCoe, 1);
            }
        }
    }

    private Tile FindTileAtPosition(Vector3 position)
    {
        foreach (var item in LevelManager.Instance.Tiles)
        {
            if (Vector3.Distance(item.Value.transform.position, position) < 0.1f)
            {
                return item.Value;
            }
        }
        return null;
    }

    private Tile GetCurrentTile()
    {
        Vector3 currentPosition = transform.position;

        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (var tile in LevelManager.Instance.Tiles)
        {
            float distance = Vector3.Distance(currentPosition, tile.Value.transform.position);
            if (distance < closestDistance)
            {
                closestTile = tile.Value;
                closestDistance = distance;
            }
        }

        return closestTile;
    }

    public Tile FindLeftTile(Tile currentTile)
    {
        Vector3 leftTilePosition = new Vector3(currentTile.transform.position.x /*- 1.65f*/, currentTile.transform.position.y, currentTile.transform.position.z);

        foreach (var item in LevelManager.Instance.Tiles)
        {
            if (Vector3.Distance(item.Value.transform.position, leftTilePosition) < 0.1f)
            {
                return item.Value;
            }
        }

        return null;
    }

    private void HandleTowerMoved(Tower tower)
    {
        if (targetEnemy == tower.gameObject)
        {
            ising = false;
            targetEnemy = null;
        }
    }

    public override void Attack()
    {
        SetAnimation(0, AnimClip[1], false);
    }


    public override void MoveTowardsEndPoint()
    {
        if (!isPushedOrPulled && !ising)
        {
            Vector3 direction = Vector3.left;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            SetAnimation(0, AnimClip[2], true);
        }
    }
}