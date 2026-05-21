using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Sora : Tower
{
    public Vector2 boxSize = new Vector2(2f, 1f);
    public Vector2 raycastStartOffset = new Vector2(1f, 0f);
    public LayerMask towerLayerMask;
    public Tower partnerTower;
    public Tower[] partnerupTower;
    public GameObject healPrefab;
    public GameObject bubblePrefab;
    public GameObject shootPrefab;
    public GameObject frontPrefab;
    public GameObject endPrefab;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> spawnedTObjects = new List<GameObject>();
    public GameObject sorastepPrefab;
    public List<Vector3> occupiedPositions = new List<Vector3>();
    public int Tnum = 1;
    public bool isKuma = false;
    private float kumaTimer = 0f;
    public AudioSource Pr;
    public AudioSource Su;

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        GameManager.instance.sora = this;

        List<Tower> allTowers = new List<Tower>(GameManager.Instance.AllTowers);
        partnerupTower = allTowers.FindAll(tower => tower != this).ToArray();

        if (LevelS > 2)
        {
            myActive = true;
            Active_cooltime = myC[3].cool;
            ActiveCooldown = Active_cooltime;
            inActive_cooltime = Active_cooltime;
        }

        Act();
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "partner_pattern1":
                if (!isKuma)
                {
                    Partner();
                }
                else
                {
                    UPartne();
                }
                break;
            case "sora_proceed":
                Pr.Play();
                break;
            case "sora_success":
                Su.Play();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name != skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name)
        {
            return;
        }

        /*if (trackEntry.Animation.Name == AnimClip[5].name)
        {
            ising = false;
            act = false;
            SetAnimation(0, AnimClip[0], true);
        }
        else*/
        if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                SetAnimation(0, AnimClip[5], false);
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

    public override void Update()
    {
        base.Update();

        if (isKuma)
        {
            kumaTimer += Time.deltaTime; 
            if (kumaTimer >= 10f) 
            {
                isKuma = false;
                ExecuteBuffOnFrontAndEnd();
                kumaTimer = 0f; 
            }
        }
    }

    public override void Attack()
    {
        base.Attack();
        SetAnimation(0, AnimClip[1], false);
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ActiveCooldown = Active_cooltime;
            DestroySpawnedObjects();
            occupiedPositions.Clear();
            Tnum = 1;
            Elemental(Tnum);
            ResetAct();
        }
    }

    public void Partner()
    {
        Tower newPartnerTower = null;
        Vector2 raycastStart = (Vector2)transform.position + raycastStartOffset;

        RaycastHit2D[] hitResults = Physics2D.BoxCastAll(raycastStart, boxSize, 0f, Vector2.right, 0f, towerLayerMask);

        Vector2 topLeft = raycastStart + new Vector2(-boxSize.x / 2, boxSize.y / 2);
        Vector2 topRight = raycastStart + new Vector2(boxSize.x / 2, boxSize.y / 2);
        Vector2 bottomLeft = raycastStart + new Vector2(-boxSize.x / 2, -boxSize.y / 2);
        Vector2 bottomRight = raycastStart + new Vector2(boxSize.x / 2, -boxSize.y / 2);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        foreach (RaycastHit2D hit in hitResults)
        {
            Tower hitTower = hit.collider.GetComponentInParent<Tower>();
            if (hitTower != null && hitTower != this && !hitTower.isEx && hitTower != partnerTower)
            {
                newPartnerTower = hitTower;
                break;
            }
        }

        if (partnerTower == null && newPartnerTower != null)
        {
            ExecuteBuffOnFrontAndEnd();
            partnerTower = newPartnerTower;
            GameManager.instance.PTower = partnerTower;
            dddeeelll(partnerTower);
            partnerTower.ApplyBuff("소라버프1");
            partnerTower.Heal(Power * myP[1].dmgCoe, this);
            SpawnFrontAndEndObjects(partnerTower.transform.position, partnerTower);
            PlayAnimationAndSound(healPrefab, partnerTower);
        }
        else if (partnerTower != null && newPartnerTower != null)
        {
            dddeeelll(partnerTower);
            ExecuteBuffOnFrontAndEnd();
            partnerTower = newPartnerTower;
            GameManager.instance.PTower = partnerTower;
            dddeeelll(partnerTower);
            partnerTower.ApplyBuff("소라버프1");
            partnerTower.Heal(Power * myP[1].dmgCoe, this);
            SpawnFrontAndEndObjects(partnerTower.transform.position, partnerTower);
            PlayAnimationAndSound(healPrefab, partnerTower);
        }
        else if (partnerTower != null)
        {
            partnerTower.ApplyBuff("소라버프1");
            partnerTower.Heal(Power * myP[1].dmgCoe, this);
            PlayAnimationAndSound(healPrefab, partnerTower);
        }
    }

    public void dddeeelll(Tower tower)
    {
        Transform parentTransform = tower.transform;
        if (parentTransform != null)
        {
            foreach (Transform child in parentTransform)
            {
                if (child.name == "소라버프(앞)(Clone)" || child.name == "소라버프(뒤)(Clone)")
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public void UPartne()
    {
        foreach (Tower tower in partnerupTower)
        {
            if (tower != null)
            {
                tower.ApplyBuff("소라강화버프"); 
                tower.Heal(Power * myP[1].dmgCoe, this); 
                PlayAnimationAndSound(healPrefab, tower);
                SpawnFrontAndEndObjects(tower.transform.position, tower);
            }
        }
    }

    private void SpawnFrontAndEndObjects(Vector3 position, Tower tower)
    {
        Transform partnerTransform = tower.transform;

        bool hasExistingBuff = false;
        foreach (Transform child in partnerTransform)
        {
            if (child.name == "소라버프(앞)(Clone)" || child.name == "소라버프(뒤)(Clone)")
            {
                hasExistingBuff = true;
                break; 
            }
        }

        if (!hasExistingBuff)
        {
            GameObject frontObject = Instantiate(frontPrefab, position + new Vector3(-0.5f, 0, 0), Quaternion.identity, partnerTransform);
            GameObject endObject = Instantiate(endPrefab, position + new Vector3(0.5f, 0, 0), Quaternion.identity, partnerTransform);

            spawnedObjects.Add(frontObject);
            spawnedObjects.Add(endObject);
        }
    }

    private void ExecuteBuffOnFrontAndEnd()
    {
        spawnedObjects.RemoveAll(obj => obj == null);

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) 
            {
                Transform parentTransform = obj.transform.parent;

                bool isPartnerTower = false;
                foreach (Tower tower in partnerupTower)
                {
                    if (parentTransform != null && partnerTower != null)
                    {
                        if (tower != null && parentTransform == partnerTower.transform)
                        {
                            isPartnerTower = true;
                        }
                    }
                }

                if (!isPartnerTower)
                {
                    SoraBuffE buffComponent = obj.GetComponent<SoraBuffE>();
                    if (buffComponent != null)
                    {
                        buffComponent.EEDD();
                    }
                }
            }
        }
    }
    //spawnedObjects.Clear();

    public void BB(Tower tower)
    {
        if (tower != null)
        {
            Vector3 bubblePosition = new Vector3(tower.transform.position.x, tower.transform.position.y, tower.transform.position.z);
            Vector3 shootPosition = new Vector3(tower.transform.position.x, tower.transform.position.y + 1.5f, tower.transform.position.z);
            GameObject bubbleGO = Instantiate(bubblePrefab, bubblePosition, Quaternion.identity);
            GameObject shootGO = Instantiate(shootPrefab, shootPosition, Quaternion.identity);
            Bubble bubble = bubbleGO.GetComponent<Bubble>();

            if (bubble != null)
            {
                bubble.Initialize(Power * myP[2].dmgCoe, myP[2].speed, myP[2].hitLim, this);
            }
        }
    }

    private void PlayAnimationAndSound(GameObject prefab, Tower tower)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity, tower.transform);
    }

    public void ActivateKuma()
    {
        isKuma = true;
        kumaTimer = 0f; 
    }

    public void Elemental(int num)
    {
        if (num < 5)
        {
            Vector3 randomPosition = GetRandomTilePosition();

            while (occupiedPositions.Contains(randomPosition))
            {
                randomPosition = GetRandomTilePosition();
            }

            occupiedPositions.Add(randomPosition);

            GameObject sorastep = Instantiate(sorastepPrefab, randomPosition, Quaternion.identity);
            spawnedTObjects.Add(sorastep);
            Soraott soraott = sorastep.GetComponent<Soraott>();

            if (soraott != null)
            {
                soraott.Initialize(this);
            }

            Tile targetTile = GetTileAtPosition(randomPosition);
            if (targetTile != null)
            {
                sorastep.transform.SetParent(targetTile.transform);
            }
        }
    }

    public void Sakana(bool sccc)
    {
        ising = true;
        if(!sccc)
        {
            SetAnimation2(0, AnimClip[2], false);
        }
        else
        {
            SetAnimation2(0, AnimClip[3], false);
        }
    }

    private Vector3 GetRandomTilePosition()
    {
        List<Tile> tiles = new List<Tile>(LevelManager.Instance.Tiles.Values);

        if (tiles.Count > 0)
        {
            int randomIndex = Random.Range(0, tiles.Count);
            return tiles[randomIndex].transform.position;
        }

        return Vector3.zero;
    }

    private Tile GetTileAtPosition(Vector3 position)
    {
        foreach (Tile tile in LevelManager.Instance.Tiles.Values)
        {
            if (tile.transform.position == position)
            {
                return tile;
            }
        }
        return null;
    }

    private void DestroySpawnedObjects()
    {
        if (spawnedTObjects.Count > 0)
        {
            foreach (GameObject obj in spawnedTObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            spawnedTObjects.Clear();
        }
    }

    protected void SetAnimation2(int trackIndex, AnimationReferenceAsset animation, bool loop)
    {
        skeletonAnimation.AnimationState.ClearTrack(trackIndex);
        skeletonAnimation.AnimationState.SetAnimation(trackIndex, animation.name, loop);
        CurrentAnimation = animation.name; 
    }
}