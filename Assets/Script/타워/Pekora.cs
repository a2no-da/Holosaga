using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Event = Spine.Event;
using System.Timers;

[System.Serializable]
public class AnimationEventSoundPair
{
    public EventDataReferenceAsset animationEvent;
    public AudioSource sound;
}

public class Pekora : Tower
{
    public SkeletonDataAsset spineAnimationData;
    public GameObject healPrefab;
    public GameObject guPrefab;
    public GameObject gsPrefab;
    public GameObject Vf3;
    private Tower allyTower;
    private GameObject animationObject;
    private ItemType randomItem;
    private Tower tower;
    private Vector3 position;
    private float delayTime;
    private bool isDelaying;
    public Buffs Bf;
    private bool P3;
    
    [System.Serializable]
    public class Buffs
    {
        public string type;
        public float duration;
        public float currentTime;
        public float percentage;
    }

    public enum ItemType
    {
        Carrot,
        Almond,
        ColdChicken,
        GoldenCarrot
    }

    public override void Start()
    {
        base.Start();
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

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
            case "heal_pattern1":
                PlaySupportEffectAnimation(randomItem, tower, tower.transform.position);
                break;
            case "throw_pattern3":
                Pattern3();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimClip[5].name)
        {
            ising = false;
            act = false;
            P3 = false;
            SetAnimation(0, AnimClip[0], true);
        }
        else if (trackEntry.Animation.Name != AnimClip[0].name)
        {
            if (act)
            {
                SetAnimation(0, AnimClip[5], false);
                act = false;
                P3 = false;
                ising = false;
            }
            else
            {
                SetAnimation(0, AnimClip[0], true);
                act = false;
                P3 = false;
                ising = false;
            }
        }
    }

    public override void Update()
    {
        base.Update();

        allyTower = FindAllyTower();

        if (isDelaying)
        {
            delayTime -= Time.deltaTime;
            if (delayTime <= 0)
            {
                isDelaying = false;
                ApplySupportEffect(randomItem, tower, position);
            }
        }


        if (isStunned)
        {
            P3 = false;
        }
    }

    public override void Attack()
    {
        if (allyTower != null)
        {
            act = false;
            base.Attack();
            Support(allyTower);
        }
        else
        {
            ising = false;
            act = false;
            attackCooldown = 0;
        }
    }

    private Tower FindAllyTower()
    {
        System.Random random = new System.Random();

        if (GameManager.Instance.AllTowers.Count == 0)
        {
            return null;
        }

        List<Tower> candidates = new List<Tower>();

        foreach (Tower tower in GameManager.Instance.AllTowers)
        {
            if (tower.Health < tower.MaxHealth && !tower.isDead && !tower.isEx)
            {
                candidates.Add(tower);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        int index = random.Next(candidates.Count);
        return candidates[index];
    }

    public void Support(Tower tower)
    {
        this.tower = tower;
        if (tower == null)
        {
            return;
        }

        if (LevelS > 1)
        {
            this.randomItem = GetRandomItem();
        }
        else
        {
            this.randomItem = ItemType.Carrot;
        }

        SetSupportAnimation(this.randomItem);
    }

    private void SetSupportAnimation(ItemType randomItem)
    {
        AnimationReferenceAsset animation = AnimClip[1];
        switch (randomItem)
        {
            case ItemType.Carrot:
                animation = AnimClip[1];
                break;
            case ItemType.Almond:
                animation = AnimClip[2];
                break;
            case ItemType.ColdChicken:
                animation = AnimClip[3];
                break;
        }

        if(!P3)
        {
            SetAnimation(0, animation, false);
        }
    }

    private Vector3 GetRandomPosition()
    {
        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        int randomIndex = UnityEngine.Random.Range(0, allTiles.Count);
        Tile randomTile = allTiles[randomIndex];
        Vector3 spawnPosition = randomTile.transform.position;

        allTiles.RemoveAt(randomIndex);

        return spawnPosition + 0.3f * Vector3.up;
    }

    private void PlaySupportEffectAnimation(ItemType randomItem, Tower tower, Vector3 position)
    {
        animationObject = new GameObject("SupportAnimation");
        if (tower != null)
        {
            animationObject.transform.SetParent(tower.transform);
            animationObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            animationObject.transform.position = position;
        }

        SkeletonAnimation skeletonAnimation = animationObject.AddComponent<SkeletonAnimation>();
        skeletonAnimation.skeletonDataAsset = spineAnimationData;
        skeletonAnimation.initialSkinName = "default";

        string animationName = "item_carrot_drop";

        switch (randomItem)
        {
            case ItemType.Carrot:
                animationName = "item_carrot_drop";
                break;
            case ItemType.Almond:
                animationName = "item_almond_drop";
                break;
            case ItemType.ColdChicken:
                animationName = "item_coldChicken_drop";
                break;
        }
        this.tower = tower;
        this.position = position;

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
        skeletonAnimation.GetComponent<Renderer>().sortingOrder = 25;

        delayTime = 0.542f / animationSpeed;
        isDelaying = true;
    }

    private void ApplySupportEffect(ItemType randomItem, Tower tower, Vector3 position)
    {
        if (tower == null)
        {
            return;
        }

        switch (randomItem)
        {
            case ItemType.Carrot:
                tower.Heal(Power * myP[1].dmgCoe, this);
                StartCoroutine(PlayAnimationAndSound(healPrefab, position, tower));
                break;
            case ItemType.Almond:
                tower.Heal(Power * myP[3].dmgCoe, this);
                StartCoroutine(PlayAnimationAndSound(healPrefab, position, tower));
                StartCoroutine(PlayAnimationAndSound(guPrefab, position, tower));
                tower.ApplyBuff("아몬드");
                break;
            case ItemType.ColdChicken:
                tower.Heal(Power * myP[4].dmgCoe, this);
                StartCoroutine(PlayAnimationAndSound(healPrefab, position, tower));
                StartCoroutine(PlayAnimationAndSound(gsPrefab, position, tower));
                tower.ApplyBuff("치킨");
                break;
        }

        if (animationObject == null)
        {
            Debug.LogError("animationObject is null before calling Destroy");
        }
        else
        {
            Destroy(animationObject);
        }

        ising = false;
    }

    private ItemType GetRandomItem()
    {
        float randomValue = UnityEngine.Random.value;

        if (randomValue < myP[2].trigProb)
            return ItemType.Carrot;
        else if (randomValue < myP[2].trigProb + myP[3].trigProb)
            return ItemType.Almond;
        else
            return ItemType.ColdChicken;
    }

    private IEnumerator PlayAnimationAndSound(GameObject prefab, Vector3 position, Tower tower)
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity, tower.transform);

        float animationDuration = 0f;
        yield return new WaitForSeconds(animationDuration);
    }

    public override void Active()
    {
        base.Active();

        if (LevelS > 2)
        {
            ising = true;
            P3 = true;
            ActiveCooldown = Active_cooltime;
            SetAnimation2(0, AnimClip[5], false);
            act = true;
            ResetAct();
        }
    }

    public void Pattern3()
    {
        List<GameObject> towerParents = new List<GameObject>();
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            Tower towerComponent = tower.transform.parent.GetComponent<Tower>();
            if (tower.transform.parent != null && towerComponent != null && !towerComponent.isEx && !towerParents.Contains(tower.transform.parent.gameObject))
            {
                towerParents.Add(tower.transform.parent.gameObject);
            }
        }

        GameObject rightmostObject = null;
        float rightmostPosition = float.NegativeInfinity;

        foreach (GameObject parent in towerParents)
        {
            if (parent.transform.position.x > rightmostPosition)
            {
                rightmostPosition = parent.transform.position.x;
                rightmostObject = parent;
            }
        }

        if (rightmostObject != null && rightmostObject.transform.childCount > 0)
        {
            Transform childTransform = rightmostObject.transform.parent.GetChild(0);
            GameObject pVf3 = Instantiate(Vf3, Vector3.zero, Quaternion.identity, childTransform);

            PekoP3 peko3 = pVf3.GetComponent<PekoP3>();

            if (peko3 != null)
            {
                peko3.Initialize(Power * myP[5].dmgCoe, this);
            }
        }
    }

    protected void SetAnimation2(int trackIndex, AnimationReferenceAsset animation, bool loop)
    {
        skeletonAnimation.AnimationState.ClearTrack(trackIndex);
        skeletonAnimation.AnimationState.SetAnimation(trackIndex, animation.name, loop);
        CurrentAnimation = animation.name;
    }
}