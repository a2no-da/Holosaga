using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using BuffInfo;
using System.Globalization;

namespace BuffInfo
{
    public partial class Data
    {
        public int ID;
        public string DescName;
        public int duration;
        public float powerUp;
        public float speedUp;
        public float HpUp;
        public string Pattern;
        public string P_user;
        public string buffName;
    }
}

public class Tower : Unit
{
    public SkeletonDataAsset towerSkeletonData;
    public string skinName;
    public string skinName2;
    public Sprite towerP;
    public Sprite towerP2;
    public Sprite towerP3;
    public Sprite towerSWim;
    public int Level;
    public static int experience;
    public int maxExperience = 1;
    protected List<GameObject> enemiesInRange = new List<GameObject>();
    public BuffInfo.Data[] myB;
    public Button towerDragButton;
    public Transform towerPosition;
    public MeshRenderer meshRenderer;
    private int originalSortingOrder;
    private SpriteRenderer hpBarRenderer;
    public Vector3 originalPosition;
    public Transform aniTransform;
    public float animationSpeed = 1f;
    public GameObject spineAnimation;
    public GameObject Body;
    public GameObject balpanPrefab;
    public GameObject balpan2Prefab;
    private GameObject balpanObject;
    private GameObject balpan2Object;
    public float originalPatternCooltime;
    public float originalAnimationSpeed;
    public float oriActive_cooltimes;
    public float oneinitialPower;
    public float oneinitialHealth;

    public GameObject debodyPrefab;
    public GameObject respawnPrefab;
    private GameObject respawnObject;
    public GameObject animationPrefab;
    private GameObject SWEF;
    private MeshRenderer respawnMeshRenderer;
    public int originrespawn;

    public bool isDragging = false;
    public bool act;
    bool maeSwitchingMode = false;
    bool isSwitchingMode = false;

    public Tile CurrentTile { get; set; }
    private Coroutine healCoroutine;
    private Coroutine respawnCountdownCoroutine;
    private Tile previousTile;
    public int onerespawnTime;
    public TextMeshProUGUI respawnTimeText;

    public int minAnimationIndex;
    public int maxAnimationIndex;

    //private Tile lastHitTile = null;
    public int LevelS;
    public LayerMask enemyLayer;

    public int IsIndependent = 0;
    public int IgnoreRange = 0;

    public int attackCount1;
    public int attackCount2;
    public int attackCount3;

    public bool myActive;

    private int IattackCount1;
    private int IattackCount2;
    private int IattackCount3;

    public GameObject spineObject;
    private Vector2 newOffset;

    public string animationName;
    private GameObject CurrentDraggingSprite;
    private DS ds;
    public GameObject Ptimer1;
    public float isingTimer = 0f;

    public TextMeshProUGUI P2T;
    public TextMeshProUGUI P3T;

    public float P2Cooldown;
    public float P2_cooltime;
    public float P3Cooldown;
    public float P3_cooltime;
    public float P2_in_cooltime;
    public float P3_in_cooltime;
    public bool isSup = false;
    public GameObject buftex;
    public bool isCardSW;
    public bool isSW;
    public bool isAngel;
    public GameObject SWEFs;
    public GameObject LevUpok;

    public SkeletonDataAsset TowerSkeletonData
    {
        get { return towerSkeletonData; }
        set { towerSkeletonData = value; }
    }

    public override void Start()
    {
        base.Start();

        UpdateHealthBarSortingOrder(5);  // 3 

        int Bindex = 1;
        var buffInfo = CsvData.Read("BuffInfo");
        myB = new BuffInfo.Data[35];

        foreach (var val in buffInfo)
        {
            myB[Bindex] = new BuffInfo.Data
            {
                duration = int.Parse(val["Duration"]),
                powerUp = float.Parse(val["PowerUp"], CultureInfo.InvariantCulture),
                speedUp = float.Parse(val["SpeedUp"], CultureInfo.InvariantCulture),
                HpUp = float.Parse(val["HpUp"], CultureInfo.InvariantCulture),
                Pattern = val["Pattern"],
                P_user = val["Pattern_user"],
            };
            Bindex++;
        }

        if (isStart)
        {
            oneinitialHealth = Health;
            oneinitialPower = Power;
            inActive_cooltime = Active_cooltime;
            P2_in_cooltime = P2_cooltime;
            P3_in_cooltime = P3_cooltime;
            Power = (int)(oneinitialPower + (oneinitialPower * powerIncreasePerLevel * (Level - 1)));
            MaxHealth = oneinitialHealth + (int)Math.Round(oneinitialHealth * healthIncreasePerLevel * (Level - 1));
            if (!isEx)
            {
                Power = Power + (Power * 0.2f * dicton.PowerUp);
            }
            oneinitialPower = (int)Power;
            if (!isEx)
            {
                MaxHealth = oneinitialHealth + (oneinitialHealth * 0.2f * dicton.HpUp);
            }
            oneinitialHealth = (int)MaxHealth;
            if (!isEx)
            {
                healthIncreasePerLevel = healthIncreasePerLevel + (healthIncreasePerLevel * 0.05f * dicton.HpInUp);
                powerIncreasePerLevel = powerIncreasePerLevel + (powerIncreasePerLevel * 0.05f * dicton.PowerInUp);
            }

            Health = MaxHealth;
            HealthBarImage.fillAmount = Health / MaxHealth;
            originalAnimationSpeed = 1;
            if (!isEx && (i != 17 || i != 18))
            {
                originalPatternCooltime = myC[1].cool;
            }
            if (!isEx)
            {
                debodyPrefab.SetActive(false);
            }

            initialPower = Power;
            initialHealth = (int)MaxHealth;
            maxExperience = initialEXP;

            BuffManager.Instance.ReapplyBuffsForTarget(gameObject);

            toCollider();
            healCoroutine = StartCoroutine(HealOverTime());
            maeSwitchingMode = isSwitchingMode;
            Handler = HealthBarInstance.GetComponent<AnimationHandler>();
            ds = GetComponentInChildren<DS>();
            isStart = false;

            if (!isEx)
            {
                StopD();
                experience = 0;

                LevUpok = Instantiate(GameManager.instance.upEffect, transform.position, Quaternion.identity, transform);
                LevUpok.SetActive(false);
            }

            if (!isEx)
            {
                if (dicton.myartifact != null)
                {
                    if (dicton.myartifact.mytype == MyType.Passive_Attack)
                    {
                        dicton.myartifact.artfunction.FunctionAti(this);
                    }

                    if (dicton.myartifact.mytype == MyType.SW)
                    {
                        dicton.myartifact.artfunction.FunctionAti(this);
                    }

                    if (dicton.myartifact.artifactId == "ae6")
                    {
                        Instantiate(dicton.myartifact.vfxarti, transform.position, Quaternion.identity, transform);
                    }
                }
            }

            if (dicton != null)
            {
                string selectedSkinName = dicton.GetSelectedSkinName();
                if (selectedSkinName != null)
                {
                    skeletonAnimation.initialSkinName = selectedSkinName;
                    skinName2 = selectedSkinName;
                    skeletonAnimation.Initialize(true);
                }
            }
        }

        dicstatdata();

        act = false;
        respawnTime = 10;
        originrespawn = respawnTime;
        onerespawnTime = respawnTime;
        if (balpanObject == null && !isEx)
        {
            balpanObject = Instantiate(balpanPrefab, this.transform);
            balpanObject.transform.localPosition = Vector3.zero;
        }


        SWEF = Instantiate(SWEFs, this.transform);
        SWEF.SetActive(true);

        if (balpan2Object == null && !isEx)
        {
            float totalValue = dicton.PowerUp + dicton.HpUp + dicton.HpInUp + dicton.PowerInUp;

            if (totalValue >= 5)
            {
                balpan2Object = Instantiate(balpan2Prefab, this.transform);
                balpan2Object.transform.localPosition = Vector3.zero;
                Balpan2 bp2 = balpan2Object.GetComponent<Balpan2>();
                if (bp2 != null)
                {
                    bp2.towerRenderer = meshRenderer;
                }

                string animationName = "";

                if (totalValue >= 20)
                {
                    animationName = "20";
                }
                else if (totalValue >= 15)
                {
                    animationName = "15";
                }
                else if (totalValue >= 10)
                {
                    animationName = "10";
                }
                else
                {
                    animationName = "5";
                }

                SkeletonAnimation skeletonAnimation = balpan2Object.GetComponent<SkeletonAnimation>();
                if (skeletonAnimation != null)
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, animationName, true);
                }
            }
        }

        if (LevelS > 1)
        {
            SkeletonAnimation skeletonAnimation = balpanObject.GetComponent<SkeletonAnimation>();
            string skinName = "upgrade2";
            skeletonAnimation.Skeleton.SetSkin(skinName);
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
        }

        if (LevelS > 2)
        {
            SkeletonAnimation skeletonAnimation = balpanObject.GetComponent<SkeletonAnimation>();
            string skinName = "upgrade3";
            skeletonAnimation.Skeleton.SetSkin(skinName);
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
        }

        if (meshRenderer != null)
        {
            if(isEx && CurrentTile != null)
            {
                meshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
            }
        }
    }

    public override void Update()
    {
        base.Update();

        skeletonAnimation.timeScale = animationSpeed;
        if (!isDead)
        {
            if (!ising && attackCooldown > 0 && !isStunned)
            {
                attackCooldown -= Time.deltaTime;
            }

            if (!ising && attackCooldown <= 0 && !isStunned && (enemiesInRange.Count > 0 || isSup))
            {
                ising = true;
                attackCooldown = Pattern_cooltime;
                Attack();
            }
            else if (isStunned)
            {
                ising = false;
                attackCooldown = Pattern_cooltime;
            }

            if (ActiveCooldown > 0 && myActive)
            {
                ActiveCooldown -= Time.deltaTime;
            }
        }

        if (isDragging && CurrentDraggingSprite != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            CurrentDraggingSprite.transform.position = mousePosition + new Vector3(0, 0.1f, 0);
        }

        if (isStunned)
        {
            if(isEx && i != 18)
            {
                SetAnimation(0, AnimClip[0], true);
            }
            ising = false;
            act = false;
            if (isDragging) { StopDragging(); }
        }

        if (isSW)
        {
            RestoreOriginalColor();
            StopD();
            isDragging = false;
            GameManager.Instance.DestroyCurrentSpineAnimation();
        }

        if (isDead)
        {
            respawnMeshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
            if (isAngel)
            {
                isAngel = false;
                BuffManager.Instance.AngelBuff(gameObject, "엔젤");
                respawnTime = 0;
            }
        }

        if (ising)
        {
            isingTimer += Time.deltaTime;
            if (isingTimer >= 20f)
            {
                SetAnimation(0, AnimClip[0], true);
                ising = false;
                isingTimer = 0f;
            }
        }
        else
        {
            isingTimer = 0f;
        }

        if (act && ising == false)
        {
            ising = true;
        }

        if (isDragging)
        {
            SWEF.SetActive(true);
            Ch();
        }
        else
        {
            SWEF.SetActive(false);
        }

        Handler.hpperText.text = ((Health / MaxHealth) * 100).ToString("F0") + "%";

        if(!isStunned && ising && skeletonAnimation.AnimationName == "idle")
        {
            ising = false;
        }

        /*if(animationSpeed <= 0)
        {
            animationSpeed = 1;
            if (dicton != null)
            {
                Artifact artifact = dicton.myartifact;
                if (dicton.myartifact != null)
                {
                    if (artifact.Speedplus != 0)
                    {
                        animationSpeed = animationSpeed + artifact.Speedplus;
                    }

                    if (artifact.SpeedIncrease != 0)
                    {
                        animationSpeed = animationSpeed + animationSpeed * (artifact.SpeedIncrease / 100f);
                    }
                }
            }

            if (inPlusStat.IpSpeed != 0)
            {
                animationSpeed += inPlusStat.IpSpeed;
            }
        }*/

        if(Active_cooltime <= 0)
        {
            Active_cooltime = 1;
        }
    }

    public virtual void LevelUp()
    {
        if (!isEx)
        {
            int requiredExperience = maxExperience;

            if (Level < 30 && experience >= requiredExperience)
            {
                Level += 1;
                experience -= requiredExperience;
                maxExperience = initialEXP + (int)(initialEXP * expIncreasePerLevel * (Level - 1));
                Power = (int)(oneinitialPower + (oneinitialPower * powerIncreasePerLevel * (Level - 1)));
                MaxHealth = oneinitialHealth + Mathf.FloorToInt(oneinitialHealth * healthIncreasePerLevel * (Level - 1));

                HealthBarImage.fillAmount = Health / MaxHealth;

                initialPower = (int)Power;
                initialHealth = (int)MaxHealth;
                if (!isDead)
                {
                    Health = MaxHealth;
                    originrespawn = ((int)(Level * respawnTimeIncreasePerLevel) + onerespawnTime);
                    respawnTime = originrespawn;
                }

                BuffManager.Instance.ReapplyBuffsForTarget(gameObject);

                if (dscName == "Kaliope")
                {
                    BuffManager.Instance.UpdateKailBuff(gameObject, "칼리");
                }

                GameManager.instance.Levveeeel();
            }

            checkstat();
        }
    }

    public override void TakeDamage(float damage)
    {
        if (!isEx)
        {
            if (dicton != null)
            {
                if (dicton.myartifact != null)
                {
                    if (dicton.myartifact.artifactId == "ae7")
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
                        {
                            Instantiate(dicton.myartifact.vfxarti, transform.position, Quaternion.identity, transform);
                            return;
                        }
                    }
                }
            }
        }

        base.TakeDamage(damage);
    }

    public void checkstat()
    {
        myStatIn(false);
    }

    public virtual void Active()
    {
        if (ActiveCooldown > 0 || isStunned || isDead)
        {
            return;
        }
        else
        {
            if (myActive)
            {
                if (!isEx)
                {
                    if (dicton.myartifact != null)
                    {
                        if (dicton.myartifact.artifactId == "au9")
                        {
                            float randomValue = UnityEngine.Random.value; 

                            if (randomValue <= 0.99f) 
                            {
                                dicton.myartifact.artfunction.FunctionAti(this);
                            }
                            else 
                            {
                                Die(); 
                            }
                        }

                        if (dicton.myartifact.artifactId == "au7")
                        {
                            dicton.myartifact.artfunction.FunctionAti(this);
                        }
                    }
                }
            }
        }
    }

    public void ResetAct()
    {
        if (!isEx)
        {
            if (dicton.myartifact != null)
            {
                if (dicton.myartifact.artifactId == "au2")
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
                    {
                        Instantiate(dicton.myartifact.vfxarti, transform.position, Quaternion.identity, transform);
                        ActiveCooldown = 1;
                    }
                }
            }
        }
    }

    public void dicstatdata()
    {
        if (dicton != null)
        {
            LevelS = dicton.LevelSS;
            if (dicton.myartifact != null)
            {
                Artifact artifact = dicton.myartifact;
                GameManager.Instance.ArtiSpot();

                if (artifact.Powerplus != 0)
                {
                    Power = Power + artifact.Powerplus;
                }

                if (artifact.PowerIncrease != 0)
                {
                    Power = Power + Power * (artifact.PowerIncrease / 100f);
                }

                if (artifact.hpplus != 0)
                {
                    MaxHealth = MaxHealth + artifact.hpplus;
                }

                if (artifact.hpIncrease != 0)
                {
                    MaxHealth = MaxHealth + MaxHealth * (artifact.hpIncrease / 100f);
                }

                if (artifact.Speedplus != 0)
                {
                    Pattern_cooltime = Pattern_cooltime - artifact.Speedplus;
                }

                if (artifact.SpeedIncrease != 0)
                {
                    Pattern_cooltime = Pattern_cooltime - Pattern_cooltime * (artifact.SpeedIncrease / 100f);
                }

                if (artifact.Speedplus != 0)
                {
                    animationSpeed = animationSpeed + artifact.Speedplus;
                }

                if (artifact.SpeedIncrease != 0)
                {
                    animationSpeed = animationSpeed + animationSpeed * (artifact.SpeedIncrease / 100f);
                }

                if (artifact.CriIncrease != 0)
                {
                    Critical = Critical + artifact.CriIncrease;
                }

                oriActive_cooltimes = Active_cooltime;

                oneinitialPower = (int)Power;
                oneinitialHealth = (int)MaxHealth;
                Health = MaxHealth;
                originalAnimationSpeed = animationSpeed;
                originalPatternCooltime = Pattern_cooltime;

                initialPower = Power;
                initialHealth = (int)MaxHealth;
            }
        }
    }

    public void InitializeMeshRenderer(int sortingOrder)
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalSortingOrder = meshRenderer.sortingOrder;
            meshRenderer.sortingOrder = sortingOrder;
        }
    }

    public virtual void Support()
    {
    }

    protected override void Die()
    {
        if (isDead) return;

        DCC();
        respawnTime = originrespawn;
        if(respawnPrefab != null)
        {
            respawnObject = Instantiate(respawnPrefab, transform.position, Quaternion.identity);
            respawnTimeText = respawnObject.transform.Find("리스폰").GetComponent<TextMeshProUGUI>();
            respawnObject.transform.SetParent(towerPosition.transform);
        }
        HealthBarInstance.SetActive(false);

        if (!isEx)
        {
            if (dicton.myartifact != null)
            {
                if (dicton.myartifact.mytype == MyType.Passive_Attack)
                {
                    dicton.myartifact.artfunction.FunctionDesAti(this);
                }
            }
        }

        if (!isEx)
        {
            debodyPrefab.SetActive(true);
            buftex.SetActive(false);
        }
        HealthBarInstance.transform.Find("Image").gameObject.SetActive(false);
        if (respawnPrefab != null)
        {
            respawnMeshRenderer = respawnObject.GetComponentInChildren<MeshRenderer>();
            if (respawnMeshRenderer != null)
            {
                respawnMeshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
            }
        }

        isDead = true;
        act = false;
        spineAnimation.SetActive(false);
        Body.SetActive(false);
        HealthBarImage.gameObject.SetActive(false);

        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
        base.Die();

        if (respawnPrefab != null)
        {
            if (respawnCountdownCoroutine != null)
            {
                StopCoroutine(respawnCountdownCoroutine);
            }
            respawnCountdownCoroutine = StartCoroutine(RespawnCountdown());
        }
    }

    public void DCC()
    {
        if(isEx) { return; }
        if (GameManager.Instance.mainStage == 5 || GameManager.Instance.isDCo == true)
        {
            GameManager.Instance.DCo += 1;
        }
    }

    public void OnTowerDragEnd()
    {
        if (isStunned || isSW) return;

        if (spineObject != null)
        {
            Destroy(spineObject);
            spineObject = null;
        }

        GameManager.Instance.DestroyCurrentSpineAnimation();
        GameManager.Instance.tttsss = false;

        Btn clickedBtn = this.gameObject.GetComponent<Btn>();
        if (clickedBtn == null)
        {
            clickedBtn = this.gameObject.AddComponent<Btn>();
        }

        SkeletonAnimation skeletonAnimation;

        GameManager.Instance.noC();
        GameManager.Instance.tttsss = true;
        if (!isEx)
        {
            GameManager.Instance.animationName = animationName;
        }
        else
        {
            GameManager.Instance.animationName = "";
        }
        originalPosition = transform.position;

        if (!isDragging)
        {
            isDragging = true;
        }
        else
        {
            isDragging = false;
            GameManager.Instance.tttsss = false;
            GameManager.Instance.DestroyCurrentSpineAnimation();
            return;
        }

        if (ds != null)
        {
            ds.PlayDragStartSound();
        }

        if (isDead)
        {
            spineObject = Instantiate(respawnPrefab, transform.position, Quaternion.identity) as GameObject;
            skeletonAnimation = spineObject.GetComponent<SkeletonAnimation>();
        }
        else
        {
            GameObject dumy = GameObject.Find("dumy");
            spineObject = new GameObject("DraggingSpine");
            spineObject.transform.parent = dumy.transform;
            skeletonAnimation = spineObject.AddComponent<SkeletonAnimation>();
            skeletonAnimation.skeletonDataAsset = towerSkeletonData;
            skeletonAnimation.Initialize(true);
            ChangeSkin(skinName, skeletonAnimation);
            skeletonAnimation.AnimationState.ClearTracks();
        }

        skeletonAnimation.Skeleton.SetColor(new Color32(72, 255, 252, 150));
        GameManager.Instance.towerKage = skeletonAnimation.gameObject;

        spineObject.SetActive(false);
    }

    public void Ch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isStunned || isSW) return;

            if (ds != null)
            {
                ds.PlayDragEndSound();
            }

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            GameObject dumyObjectToDestroy = GameObject.Find("dumy");

            int layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePosition, layerMask);

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider != null && hitCollider.tag == "Tile")
                {
                    Tile hitTile = hitCollider.GetComponent<Tile>();
                    hitTile.MoveTower(this);

                    if (meshRenderer != null)
                    {
                        meshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
                    }
                    if (isDead && respawnMeshRenderer != null)
                    {
                        respawnMeshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
                    }

                    UpdateEnemiesInRange();

                    isDragging = false;

                    if (spineObject != null)
                    {
                        Destroy(spineObject);
                        spineObject = null;
                    }
                    GameManager.Instance.tttsss = false;
                    GameManager.Instance.DestroyCurrentSpineAnimation();

                    if (dumyObjectToDestroy != null)
                    {
                        foreach (Transform child in dumyObjectToDestroy.transform)
                        {
                            if (!child.gameObject.activeSelf)
                            {
                                Destroy(child.gameObject);
                            }
                        }
                    }

                    if (!isEx)
                    {
                        if (dicton.myartifact != null)
                        {
                            if (dicton.myartifact.mytype == MyType.SW)
                            {
                                dicton.myartifact.artfunction.FunctionAlltime(this);
                            }
                        }
                    }

                    return;
                }
            }

            transform.position = originalPosition;

            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
            }

            UpdateEnemiesInRange();

            isDragging = false;

            if (spineObject != null)
            {
                Destroy(spineObject);
                spineObject = null;
            }

            if (dumyObjectToDestroy != null)
            {
                foreach (Transform child in dumyObjectToDestroy.transform)
                {
                    if (!child.gameObject.activeSelf)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            GameManager.Instance.DestroyCurrentSpineAnimation();
            GameManager.Instance.tttsss = false;

            if (!isEx)
            {
                if (dicton.myartifact != null)
                {
                    if (dicton.myartifact.mytype == MyType.SW)
                    {
                        dicton.myartifact.artfunction.FunctionAlltime(this);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            transform.position = originalPosition;
            GameObject dumyObjectToDestroy = GameObject.Find("dumy");

            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
            }

            UpdateEnemiesInRange();

            isDragging = false;

            if (spineObject != null)
            {
                Destroy(spineObject);
                spineObject = null;
            }

            if (dumyObjectToDestroy != null)
            {
                foreach (Transform child in dumyObjectToDestroy.transform)
                {
                    if (!child.gameObject.activeSelf)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }

            GameManager.Instance.DestroyCurrentSpineAnimation();
            GameManager.Instance.tttsss = false;
        }
    }

    private IEnumerator RespawnCountdown()
    {

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1f);
            respawnTime--;
        }

        Respawn();

    }

    public virtual void Respawn()
    {
        Health = MaxHealth;
        Destroy(respawnObject);
        if (!isEx)
        {
            debodyPrefab.SetActive(false);
            buftex.SetActive(true);
        }

        isDead = false;
        healCoroutine = StartCoroutine(HealOverTime());
        HealthBarImage.gameObject.SetActive(true);
        spineAnimation.SetActive(true);
        HealthBarInstance.SetActive(true);
        HealthBarInstance.transform.Find("Image").gameObject.SetActive(true);
        Body.SetActive(true);
        StartCoroutine(FadeInCoroutine(skeletonAnimation, 1.0f));
        ising = false;
        attackCooldown = Pattern_cooltime;

        if (!isEx)
        {
            if (dicton.myartifact != null)
            {
                if (dicton.myartifact.mytype == MyType.Passive_Attack)
                {
                    dicton.myartifact.artfunction.FunctionDesAti(this);
                }
            }
        }

        SetAnimation(0, AnimClip[0], true);
    }

    private IEnumerator FadeInCoroutine(SkeletonAnimation animation, float duration)
    {
        float startAlpha = 0f;
        float endAlpha = 1f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, currentTime / duration);

            foreach (var slot in animation.Skeleton.Slots)
            {
                slot.A = alpha;
            }

            yield return null;
        }

        foreach (var slot in animation.Skeleton.Slots)
        {
            slot.A = endAlpha;
        }
    }

    public void upd()
    {
        UpdateEnemiesInRange();

        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
        }
        if (isDead && respawnMeshRenderer != null)
        {
            respawnMeshRenderer.sortingOrder = (CurrentTile.GridPosition.Y * 2 - 3);
        }
    }

    private void StopDragging()
    {
        isDragging = false;
        transform.position = originalPosition;

        if (spineObject != null)
        {
            Destroy(spineObject);
            spineObject = null;
        }

        GameManager.Instance.DestroyCurrentSpineAnimation();
        GameManager.Instance.tttsss = false;
    }

    private void StopD()
    {
        isDragging = false;

        if (spineObject != null)
        {
            Destroy(spineObject);
            spineObject = null;
        }

        GameManager.Instance.DestroyCurrentSpineAnimation();
        GameManager.Instance.tttsss = false;
    }

    public virtual void toCollider()
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

        if ((towerTile == null || towerCollider == null) && isEx)
        {
            return;
        }

        Vector2 newSize = new Vector2(xSize, ySize);
        towerCollider.size = newSize;

        Vector3 localTowerPosition = transform.InverseTransformPoint(towerTile.transform.position);

        newOffset = new Vector2((localTowerPosition.x + (0.825f) + (xSize / 2)) + (centerX * 1.65f), localTowerPosition.y + (1.01f) + (centerY * 2.02f));

        towerCollider.offset = newOffset;
    }

    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy") && collider.gameObject != Ptimer1)
        {
            if (!enemiesInRange.Contains(collider.gameObject))
            {
                enemiesInRange.Add(collider.gameObject);
            }
            targetEnemy = collider.gameObject;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            if (enemiesInRange.Contains(collider.gameObject))
            {
                if (targetEnemy == collider.gameObject)
                {
                    targetEnemy = null;
                }
                enemiesInRange.Remove(collider.gameObject);
            }
        }
    }

    public void UpdateEnemiesInRange()
    {
        enemiesInRange.Clear();
        targetEnemy = null;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = boxCollider.size;
        Vector2 offset = boxCollider.offset;
        Vector2 point = boxCollider.transform.position + new Vector3(offset.x, offset.y, 0);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(point, size, enemyLayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy") && collider.gameObject != Ptimer1)
            {
                enemiesInRange.Add(collider.gameObject);
                targetEnemy = collider.gameObject;
            }
        }
    }

    private IEnumerator HealOverTime()
    {
        if (!isDead)
        {
            while (true)
            {
                Health += H_regen;
                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public override void Heal(float amount, Tower tower)
    {
        if (isEx) { return; }
        if (isDead) { return; }
        float hh = amount;

        if (tower != null)
        {
            if (tower.dicton != null)
            {
                if (tower.dicton.myartifact != null)
                {
                    if (tower.dicton.myartifact.artifactId == "ae12")
                    {
                        hh = amount * 2f;
                        BuffManager.Instance.ApplyBuff(gameObject, "도토리", 5, 0, 0, 0.2f);
                    }
                }
            }
        }
        base.Heal(hh, tower);
    }

    public void ChangeSkin(string skinName, SkeletonAnimation skeletonAnimation)
    {
        if (string.IsNullOrEmpty(skinName)) return;
        var skeletonData = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true);
        var skin = skeletonData.FindSkin(skinName);
        if (skin == null) return;
        skeletonAnimation.initialSkinName = skinName;
        skeletonAnimation.Skeleton.SetSkin(skin);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
    }

    public void UpdateHealthBarSortingOrder(int newOrder)
    {
        Canvas hpBarCanvas = HealthBarInstance.GetComponent<Canvas>();
        if (hpBarCanvas == null)
        {
            hpBarCanvas = HealthBarInstance.GetComponentInChildren<Canvas>();
        }

        if (hpBarCanvas != null)
        {
            hpBarCanvas.sortingOrder = newOrder;
        }
    }

    public void ApplyBuff(string buffName)
    {
        if (isEx) { return; }
        switch (buffName)
        {
            case "아몬드":
                BuffManager.Instance.ApplyBuff(gameObject, "아몬드", myB[1].duration, myB[1].powerUp, myB[1].speedUp, myB[1].HpUp);
                break;
            case "치킨":
                BuffManager.Instance.ApplyBuff(gameObject, "치킨", myB[2].duration, myB[2].powerUp, myB[2].speedUp, myB[2].HpUp);
                break;
            case "황금당근":
                BuffManager.Instance.ApplyBuff(gameObject, "황금당근", myB[3].duration, myB[3].powerUp, myB[3].speedUp, myB[3].HpUp);
                break;
            case "와타장판":
                BuffManager.Instance.ApplyBuff(gameObject, "와타장판", myB[4].duration, myB[4].powerUp, myB[4].speedUp, myB[4].HpUp);
                break;
            case "왓슨3":
                BuffManager.Instance.ApplyBuff(gameObject, "왓슨3", myB[5].duration, myB[5].powerUp, myB[5].speedUp, myB[5].HpUp);
                break;
            case "칼리":
                BuffManager.Instance.ApplyKaliBuff(gameObject, "칼리", 1);
                break;
            case "칼리보스":
                BuffManager.Instance.ApplyKaliBuff(gameObject, "칼리보스", 50);
                break;
            case "카나타":
                BuffManager.Instance.ApplyKanataBuff(gameObject, "카나타", 1);
                break;
            case "카나타0":
                BuffManager.Instance.ApplyKanataBuff(gameObject, "카나타0", 0);
                break;
            case "루나초코":
                BuffManager.Instance.ApplyBuff(gameObject, "루나초코", myB[6].duration, myB[6].powerUp, myB[6].speedUp, myB[6].HpUp);
                break;
            case "루나진저":
                BuffManager.Instance.ApplyBuff(gameObject, "루나진저", myB[7].duration, myB[7].powerUp, myB[7].speedUp, myB[7].HpUp);
                break;
            case "루나차":
                BuffManager.Instance.ApplyBuff(gameObject, "루나차", myB[8].duration, myB[8].powerUp, myB[8].speedUp, myB[8].HpUp);
                break;
            case "루나피버":
                BuffManager.Instance.ApplyBuff(gameObject, "루나피버", myB[9].duration, myB[9].powerUp, myB[9].speedUp, myB[9].HpUp);
                break;
            case "루나타임":
                BuffManager.Instance.ApplyBuff(gameObject, "루나타임", myB[10].duration, myB[10].powerUp, myB[10].speedUp, myB[10].HpUp);
                break;
            case "쿠로가미":
                BuffManager.Instance.ApplyBuff(gameObject, "쿠로가미", myB[11].duration, myB[11].powerUp, myB[11].speedUp, myB[11].HpUp);
                break;
            case "무메이2":
                BuffManager.Instance.ApplyBuff(gameObject, "무메이2", myB[24].duration, myB[24].powerUp, myB[24].speedUp, myB[24].HpUp);
                break;
            case "소라버프1":
                BuffManager.Instance.ApplyBuff(gameObject, "소라버프1", myB[29].duration, myB[29].powerUp, myB[29].speedUp, myB[29].HpUp);
                break;
            case "소라강화버프":
                BuffManager.Instance.ApplyBuff(gameObject, "소라강화버프", myB[30].duration, myB[30].powerUp, myB[30].speedUp, myB[30].HpUp);
                break;
            case "미코행운":
                BuffManager.Instance.ApplyBuff(gameObject, "미코행운", myB[31].duration, myB[31].powerUp, myB[31].speedUp, myB[31].HpUp);
                break;
            case "미코벛꽃길":
                BuffManager.Instance.ApplyBuff(gameObject, "미코벛꽃길", myB[32].duration, myB[32].powerUp, myB[32].speedUp, myB[32].HpUp);
                break;
            default:
                return;
        }
    }

    public void RemoveBuff(string buffName)
    {
        Buff buffToRemove = null;
        foreach (var buff in BuffManager.Instance.activeBuffs)
        {
            if (buff.buffName == buffName && buff.target == gameObject)
            {
                buffToRemove = buff;
                break;
            }
        }

        if (buffToRemove != null)
        {
            BuffManager.Instance.RemoveBuff(buffToRemove);
        }
    }

    public void ChangeSkeletonColor()
    {
        if (skeletonAnimation == null) return;

        var skeleton = skeletonAnimation.Skeleton;

        for (int i = 0; i < skeleton.Slots.Count; i++)
        {
            var slot = skeleton.Slots.Items[i];
            slot.SetColor(new Color32(72, 255, 252, 255));
        }

        skeletonAnimation.LateUpdate();
    }

    public void RestoreOriginalColor()
    {
        if (skeletonAnimation == null) return;

        var skeleton = skeletonAnimation.Skeleton;

        for (int i = 0; i < skeleton.Slots.Count; i++)
        {
            var slot = skeleton.Slots.Items[i];
            slot.SetColor(new Color32(255, 255, 255, 255));
        }

        skeletonAnimation.LateUpdate();
    }

    public void myStatIn(bool yes)
    {
        if(isEx) { return; }
        float healthRatio = (float)Health / MaxHealth;

        if (oriActive_cooltimes == 0)
        {
            oriActive_cooltimes = Active_cooltime;
        }

        if (inPlusStat.IpPower != 0)
        {
            Power = Power + Mathf.FloorToInt(initialPower * inPlusStat.IpPower);
        }

        if (inPlusStat.IpHp != 0)
        {
            MaxHealth = MaxHealth + (30 * inPlusStat.IpHp);
            Health = (int)(MaxHealth * healthRatio);
        }

        if (yes)
        {
            if (inPlusStat.IpSpeed != 0)
            {
                animationSpeed += inPlusStat.IpSpeed;
                Pattern_cooltime = Mathf.Max(0, Pattern_cooltime - originalPatternCooltime * inPlusStat.IpSpeed);
            }
        }

        if (inPlusStat.Imcool != 0)
        {
            mydasdasd();
        }
    }

    public void myStatInt(bool yes)
    {
        if (isEx) { return; }

        float healthRatio = (float)Health / MaxHealth;

        if (oriActive_cooltimes == 0)
        {
            oriActive_cooltimes = Active_cooltime;
        }

        if (inPlusStat.IpPower != 0)
        {
            Power = Power + Mathf.FloorToInt(initialPower * inPlusStat.IpPower);
        }

        if (inPlusStat.IpHp != 0)
        {
            MaxHealth = initialHealth + (30 * inPlusStat.IpHp);
            Health = (int)(MaxHealth * healthRatio);
        }

        if (yes)
        {
            if (inPlusStat.IpSpeed != 0)
            {
                animationSpeed += inPlusStat.IpSpeed;
                Pattern_cooltime = Mathf.Max(0, Pattern_cooltime - originalPatternCooltime * inPlusStat.IpSpeed);
            }
        }

        if (inPlusStat.Imcool != 0)
        {
            mydasdasd();
        }
    }

    public void mydasdasd()
    {
        if (isEx) { return; }

        Active_cooltime = Mathf.Max(
            1,
            inActive_cooltime - (inActive_cooltime * (inPlusStat.Imcool + inPlusStat.cardcool))
        );
        P2_cooltime = Mathf.Max(
            1,
            P2_in_cooltime - (P2_in_cooltime * inPlusStat.Imcool)
        );
        P3_cooltime = Mathf.Max(
            1,
            P3_in_cooltime - (P3_in_cooltime * inPlusStat.Imcool)
        );
    }

    public void Act()
    {
        if (isEx) { return; }

        P2_in_cooltime = P2_cooltime;
        P3_in_cooltime = P3_cooltime;
        oriActive_cooltimes = Active_cooltime;

        if (dicton != null)
        {
            if (dicton.myartifact != null)
            {
                Artifact artifact = dicton.myartifact;
                if (artifact.ASpeedplus != 0)
                {
                    Active_cooltime = Active_cooltime - artifact.ASpeedplus;
                    P2_cooltime = P2_cooltime - artifact.ASpeedplus;
                    P3_cooltime = P3_cooltime - artifact.ASpeedplus;
                }

                if (artifact.ASpeedIncrease != 0)
                {
                    Active_cooltime = Active_cooltime - Active_cooltime * (artifact.ASpeedIncrease / 100f);
                    P2_cooltime = P2_cooltime - (P2_cooltime * (artifact.ASpeedIncrease / 100f));
                    P3_cooltime = P3_cooltime - (P3_cooltime * (artifact.ASpeedIncrease / 100f));
                }
            }

            P2_in_cooltime = P2_cooltime;
            P3_in_cooltime = P3_cooltime;
            oriActive_cooltimes = Active_cooltime;
        }
    }

    public void KAAKA()
    {
        if (dscName == "Kaliope")
        {
            BuffManager.Instance.UpdateKailBuff(gameObject, "칼리");
        }
    }

    public override void Stun(float stunDuration)
    {
        base.Stun(stunDuration);
    }
}
