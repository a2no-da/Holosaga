using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Spine.Unity;
using UnityEngine.UI;
using MonsterPattern;
using MonsterCooltime;
using System.Globalization;

namespace MonsterPattern
{
    public partial class Data
    {
        public int ID;
        public string dscName;
        public float dmgCoe;
        public int hitLim;
        public float speed;
        public int trigCount;
        public float trigProb;
        public int stunTime;
        public string P_user;
    }
}

namespace MonsterCooltime
{
    public partial class Data
    {
        public int ID;
        public string dscName;
        public int IgnRange;
        public int IsIndep;
        public float initCool;
        public float cool;
        public string P_user;
    }
}

public class Enemy : Unit
{
    public enum EnemyType  
    {
        Normal,
        Special,
        Boss
    }

    public EnemyType type;
    public float moveSpeed = 2.0f; 
    public float initialMoveSpeed = 2.0f;
    public float endPointX;
    public int experienceReward = 1;
    public int MainDamage;
    public Transform spawnPoint;
    public GameObject HPBar;

    public bool isPushedOrPulled = false;
    public bool hasAppliedForce = false;
    public Tower AttackingTower { get; set; }

    private float startTime;
    private float duration = 0.5f;
    private bool startHiding = false;
    public MonsterPattern.Data[] mP;
    public MonsterCooltime.Data[] mC;
    private float hp_i;
    private float power_i;
    private int spawnL;

    public Vector2 raycastDirection = Vector2.right;
    public Vector2 raycastStartOffset = Vector2.zero;
    public float raycastDistance = 10f;
    public Vector2 boxSize = new Vector2(1f, 1f);
    public bool iskali = false;
    public GameObject soulballPrefab;
    public bool kabedong;
    public double[] atiper;
    public Artifact[] artifact;
    public double[] EXatiper;
    public Artifact[] EXartifact;
    public GameObject dropArti;
    public GameObject chestPrefab;
    public float Probab;
    private bool sloweria = false;
    private bool isex = false;
    public GameObject[] featherSubPrefab;
    private List<GameObject> featherSubs = new List<GameObject>();
    public GameObject BombPrefab;
    private bool isp3;
    private float MA1 = 0;
    private float MA2 = 0;
    private Tower MT;
    private bool soulballCreated = false;
    private GameObject criDam;
    public bool snab = false;
    public bool isFire = false;
    public float fireDuration;
    public float fireDamageTimer;
    private bool buruburu = false;

    public override void Start()
    {
        base.Start();

        criDam = GameManager.Instance.Criti;
        if (!LTimer)
        {
            isStart = false;

            endPointX = -10.4f;
            initialMoveSpeed = moveSpeed;
            toCollider();

            if (HealthBarInstance != null)
            {
                Image[] images = HealthBarInstance.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    Color color = img.color;
                    color.a = 0f;
                    img.color = color;
                }
            }

            Handler = HealthBarInstance.GetComponent<AnimationHandler>();
            HPBar = HealthBarInstance.transform.Find("Image/HPBar").gameObject;
            GameManager.Instance.CheckAndSetKali(this);

            ising = false;
            InitializeEnemy();
        }
    }

    public void InitializeEnemy()
    {
        atiper = CombineArrays(atiper, GameManager.Instance.atiper);
        artifact = CombineArrays(artifact, GameManager.Instance.artifact);
        EXatiper = CombineArrays(EXatiper, GameManager.Instance.EXatiper);
        EXartifact = CombineArrays(EXartifact, GameManager.Instance.EXartifact);
    }

    private T[] CombineArrays<T>(T[] original, T[] toAdd)
    {
        if (toAdd == null || toAdd.Length == 0)
        {
            return original; 
        }

        T[] newArray = new T[original.Length + toAdd.Length];
        original.CopyTo(newArray, 0); 
        toAdd.CopyTo(newArray, original.Length); 

        return newArray; 
    }

    public override void Stat()
    {
        if(snab) { return; }
        if (monsterStat == null)
        {
            monsterStat = CsvData.Read("MonsterStat");
        }
        var data = monsterStat[i];

        dscName = data["Desc_name"];
        centerX = float.Parse(data["CenterPos"].Split(',')[0], CultureInfo.InvariantCulture);
        centerY = float.Parse(data["CenterPos"].Split(',')[1], CultureInfo.InvariantCulture);
        RangeX = float.Parse(data["Range"].Split(',')[0], CultureInfo.InvariantCulture);
        RangeY = float.Parse(data["Range"].Split(',')[1], CultureInfo.InvariantCulture);
        moveSpeed = float.Parse(data["moveSpeed"], CultureInfo.InvariantCulture);
        MaxHealth = float.Parse(data["Hp"], CultureInfo.InvariantCulture);
        Power = int.Parse(data["Power"]);
        hp_i = float.Parse(data["HP_inc"], CultureInfo.InvariantCulture);
        power_i = float.Parse(data["Power_inc"], CultureInfo.InvariantCulture);
        experienceReward = int.Parse(data["Exp_reward"]);
        MainDamage = int.Parse(data["Main_damage"]);
        spawnL = int.Parse(data["spawnLevel"]);

        float N = 1;
        switch (DifficultyManager.Instance.Difficulty)
        {
            case (int)DifficultyButton.DifficultyLevel.Easy:
                N = 0.65f;
                break;
            case (int)DifficultyButton.DifficultyLevel.Normal:
                N = 1;
                break;
            case (int)DifficultyButton.DifficultyLevel.Hard:
                N = 1.3f;
                break;
            case (int)DifficultyButton.DifficultyLevel.Extreme:
                isex = true;
                N = 1f;
                break;
        }
        int stageLevel = GameManager.Instance.mainStage;
        //stageLevel = 4; //테스트 용
        MaxHealth = (int)Math.Round((MaxHealth + MaxHealth * hp_i * (stageLevel - spawnL)) * N);
        Power = (int)Math.Round((Power + Power * power_i * (stageLevel - spawnL)) * N);
        Health = MaxHealth;

        switch (DifficultyManager.Instance.Difficulty)
        {
            case (int)DifficultyButton.DifficultyLevel.Easy:
            case (int)DifficultyButton.DifficultyLevel.Normal:
            case (int)DifficultyButton.DifficultyLevel.Hard:
                break;
            case (int)DifficultyButton.DifficultyLevel.Extreme:
                Power = Power + (Power * 1);
                MaxHealth = MaxHealth + (MaxHealth * 1);
                Health = MaxHealth;
                break;
        }

        var monsterPatternData = CsvData.Read("MonsterPattern");
        var monsterCooltimeData = CsvData.Read("MonsterCooltime");

        mP = new MonsterPattern.Data[20];
        mC = new MonsterCooltime.Data[20];


        int Pindex = 1;

        foreach (var val in monsterPatternData)
        {
            if (dscName.Equals(val["Pattern_user"]))
            {
                mP[Pindex] = new MonsterPattern.Data
                {
                    ID = int.Parse(val["ID"]),
                    dscName = val["Desc_name"],
                    dmgCoe = float.Parse(val["Damage_CoE"], CultureInfo.InvariantCulture),
                    hitLim = int.Parse(val["HitLimit"]),
                    speed = float.Parse(val["Speed"], CultureInfo.InvariantCulture),
                    trigCount = int.Parse(val["Trig_Count"]),
                    trigProb = float.Parse(val["Trig_Prob"], CultureInfo.InvariantCulture),
                    stunTime = int.Parse(val["Stun_time"]),
                    P_user = val["Pattern_user"]
                };
                Pindex++;
            }
        }

        int Cindex = 1;

       foreach (var val in monsterCooltimeData)
    {
        if (dscName.Equals(val["Pattern_user"]))
        {
            mC[Cindex] = new MonsterCooltime.Data
            {
                ID = int.Parse(val["ID"]),
                dscName = val["Desc_name"],
                IgnRange = int.Parse(val["IgnoreRange"]),
                IsIndep = int.Parse(val["IsIndependent"]),
                initCool = float.Parse(val["Cooltime_init"], CultureInfo.InvariantCulture),
                cool = float.Parse(val["Cooltime"], CultureInfo.InvariantCulture),
                P_user = val["Pattern_user"]
            };
            Cindex++;
        }
    }

        if (!Laplus)
        {
            Pattern_cooltime = mC[1].cool;
            attackCooldown = mC[1].initCool;
        }
    }

    public override void Update()
    {
        base.Update();

        if (!isEx || !LTimer)
        {
            bool shouldConsiderIsing1 = mC[1].IsIndep == 0;
            if ((!ising || !shouldConsiderIsing1) && attackCooldown > 0 && !isStunned)
            {
                attackCooldown -= Time.deltaTime;
            }
        }

        if (endPointX < 0)
        {
            if (transform.position.x <= endPointX)
            {
                GameManager.Instance.OnMonsterReached(MainDamage);
                Destroy(HealthBarInstance);

                HealthBarImage = null;
                HealthBarInstance = null;
                Destroy(gameObject);
            }
        }
        else
        {
            if (transform.position.x >= endPointX)
            {
                GameManager.Instance.OnMonsterReached(MainDamage);
                Destroy(HealthBarInstance);

                HealthBarImage = null;
                HealthBarInstance = null;
                Destroy(gameObject);
            }
        }
 
        if (isStunned)
        {
            ising = false;
            attackCooldown = Pattern_cooltime;
            SetAnimation(0, AnimClip[0], true);
        }

        if (isFire)
        {
            if (fireDuration <= 0)
            {
                isFire = false; 
            }
            else
            {
                fireDuration -= Time.deltaTime;
                fireDamageTimer += Time.deltaTime;
                if (fireDamageTimer >= 1f)
                {
                    FireD();
                    fireDamageTimer = 0f; 
                }
            }
        }

        if (startHiding && !LTimer)
        {
            if (HealthBarInstance == null) return;
            float t = (Time.time - startTime) / duration;
            if (HealthBarImage != null)
            {
                Image[] imgs = HealthBarInstance.transform.Find("Image").GetComponentsInChildren<Image>();
                foreach (Image img in imgs)
                {
                    if (img != null)
                    {
                        Color c = img.color;
                        c.a = Mathf.Lerp(1f, 0f, t);
                        img.color = c;
                    }
                }
            }

            if (t >= 1f)
            {
                startHiding = false;
            }
        }
    }

    public void UpdateAttackingTower(Tower attackingTower)
    {
        AttackingTower = attackingTower;
    }

    public override void TakeDamage(float damage)
    {
        if (Time.timeScale != 0)
        {
            if (HealthBarInstance != null)
            {
                HealthBarInstance.SetActive(true);
                HealthBarInstance.transform.Find("Image").gameObject.SetActive(true);
                HPBar.SetActive(true);
            }

            if (AttackingTower != null)
            {
                if (AttackingTower.i == 14)
                {
                    if (AttackingTower.LevelS > 2)
                    {
                        isp3 = true;
                    }

                    MA1 = AttackingTower.Power * AttackingTower.myP[2].dmgCoe;
                    MA2 = AttackingTower.Power * AttackingTower.myP[3].dmgCoe;
                    MT = AttackingTower;
                }
            }

            if (AttackingTower != null)
            {
                bool isCriticalHit = UnityEngine.Random.Range(0f, 100f) < AttackingTower.Critical;

                if (isCriticalHit)
                {
                    damage *= 2;
                    ShowCritiText(damage);
                }
                else
                {
                    ShowDamageText(damage);
                }

                if (AttackingTower.dicton != null)
                {
                    if (AttackingTower.dicton.myartifact != null)
                    {
                        if (AttackingTower.dicton.myartifact.artifactId == "ae9")
                        {
                            bool isjang = UnityEngine.Random.Range(0f, 100f) < 2f;//2

                            if (isjang)
                            {
                                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0);
                                GameObject jangp = Instantiate(AttackingTower.dicton.myartifact.vfxarti, spawnPosition, Quaternion.identity);
                                R9_Jang r9_Jang = jangp.GetComponent<R9_Jang>();

                                if (r9_Jang != null)
                                {
                                    r9_Jang.Initialize(5, AttackingTower);
                                }
                            }
                        }

                        if (AttackingTower.dicton.myartifact.artifactId == "au6")
                        {
                            bool isS = UnityEngine.Random.Range(0f, 100f) < 1f;
                            if (isS)
                            {
                                Stun(1f);
                            }
                        }
                    }
                }
            }

            if (Health >= damage)
            {
                Health -= damage;
            }
            else
            {
                Health = 0;
            }

            if (AttackingTower != GameManager.instance.sora)
            {
                soraBB();
            }

            if (AttackingTower != null)
            {
                if (AttackingTower.dicton != null)
                {
                    if (AttackingTower.dicton.id == "19")
                    {
                        Olie olie = AttackingTower.GetComponentInChildren<Olie>();
                        Instantiate(olie.oneTEffect, AttackingTower.transform.position, AttackingTower.transform.rotation, AttackingTower.transform);
                        AttackingTower.Heal(damage * 0.05f, AttackingTower);
                    }
                }
            }

            StartCoroutine(ChangeColorOverTime());

            if (HealthBarInstance != null)
            {
                HealthBarImage.fillAmount = Health / MaxHealth;
                if (Health <= 0)
                {
                    Destroy(HealthBarInstance);

                    HealthBarImage = null;
                    HealthBarInstance = null;
                }
                else
                {
                    if (HealthBarInstance != null)
                    {
                        startTime = Time.time;
                        startHiding = true;
                    }
                }
            }

            if (AttackingTower != null)
            {
                float offsetX = UnityEngine.Random.Range(-0.4f, 0.4f);
                float offsetY = UnityEngine.Random.Range(-0.4f, 0.4f);
                Vector3 offset = new Vector3(offsetX, offsetY, 0);

                Vector3 position = spawnPoint.position + offset;

                GameObject animationObject = Instantiate(AttackingTower.animationPrefab, position, Quaternion.identity);
                AnimationController animationController = animationObject.GetComponent<AnimationController>();
                if (animationController != null)
                {
                    if (!buruburu)
                    {
                        animationController.PlayAnimation(AttackingTower.minAnimationIndex, AttackingTower.maxAnimationIndex);
                    }
                    else
                    {
                        animationController.PlayAnimation(6, 6);
                    }
                }
            }

            if (AttackingTower != null)
            {
                if (!AttackingTower.isEx)
                {
                    if (AttackingTower.dicton.myartifact != null)
                    {
                        if (AttackingTower.dicton.myartifact.artifactId == "ae5")
                        {
                            float healAmount = damage * 0.2f;
                            AttackingTower.Heal(healAmount, AttackingTower);
                        }
                    }
                }
            }

            if (Health <= 0 && type != EnemyType.Boss)
            {
                Die();
            }
        }
    }

    public void soraBB()
    {
        if (GameManager.instance.sora != null)
        {
            if (GameManager.instance.sora.isKuma)
            {
                GameManager.instance.SoraB(AttackingTower);
                return;
            }
        }

        if (GameManager.instance.PTower != null)
        {
            if (GameManager.instance.PTower == AttackingTower)
            {
                GameManager.instance.SoraB(AttackingTower);
            }
        }
    }

    public void ShowCritiText(float damage)
    {
        if (AttackingTower.dicton.myartifact != null)
        {
            if (AttackingTower.dicton.myartifact.CriIncrease > 0)
            {
                Instantiate(AttackingTower.dicton.myartifact.vfxarti, transform.position, Quaternion.identity);
            }
        }

        GameObject damageText = Instantiate(criDam, transform.position, Quaternion.identity);
        damageText.GetComponent<CriText>().SetDamage((int)Math.Round(damage));
    }

    public virtual void MoveTowardsEndPoint()
    {
    }

    // CC (몬스터 한정)

    //슬로우 
    public bool isSlowEffectActive = false;
    private Coroutine slowEffectCoroutine = null;

    public void SlowEffect(float slow, float slowT)
    {
        if (Laplus) { return; }
        if (LTimer) { return; }

        if (!isSlowEffectActive)
        {
            moveSpeed -= (moveSpeed * slow);
            isSlowEffectActive = true;
        }

        if (slowEffectCoroutine != null)
        {
            StopCoroutine(slowEffectCoroutine);
        }

        slowEffectCoroutine = StartCoroutine(ResetSpeed(slow, slowT));
    }

    public void SlowMon(float slow)
    {
        if (Laplus) { return; }
        if (LTimer) { return; }
        if (sloweria) { return; }

        sloweria = true;
        moveSpeed = moveSpeed - slow;
    }

    public void ResetedSpeed()
    {
        moveSpeed = initialMoveSpeed;
        sloweria = false;
    }

    IEnumerator ResetSpeed(float slow, float slowT)
    {
        yield return new WaitForSeconds(slowT);
        moveSpeed = initialMoveSpeed;
        isSlowEffectActive = false;
    }

    public override IEnumerator StunDuration(float stunDuration) 
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        stunCoroutine = null;

        moveSpeed = initialMoveSpeed;
    }

    //이동제약
    public void DisableMovement()
    {
        moveSpeed = 0;
    }

    public void EnableMovement()
    {
        moveSpeed = initialMoveSpeed;
    }   
    //

    protected override void Die()
    {
        if (soulballCreated) return;

        base.Die();
        Inugami();
        GameManager.Instance.OnMonsterReached(0);
        Tower.experience += experienceReward;

        if (!snab)
        {
            for (int i = 0; i < atiper.Length; i++)
            {
                float randomValue = UnityEngine.Random.Range(0f, 100f);

                if (randomValue <= (atiper[i]))
                {
                    GameObject artti = Instantiate(dropArti, new Vector3(0, 0, 0), Quaternion.identity);
                    DropArti droparti = artti.GetComponent<DropArti>();
                    if (droparti != null)
                    {
                        droparti.artifact = artifact[i];
                        GameManager.Instance.AddDroppedArtifact(artifact[i], 1);
                    }
                    artifact[i].Quantity += 1;
                }
            }

            for (int i = 0; i < EXatiper.Length; i++)
            {
                float randomValue = UnityEngine.Random.Range(0f, 100f);

                if (isex)
                {
                    if (randomValue <= (EXatiper[i]))
                    {
                        GameObject artti = Instantiate(dropArti, new Vector3(0, 0, 0), Quaternion.identity);
                        DropArti droparti = artti.GetComponent<DropArti>();
                        if (droparti != null)
                        {
                            droparti.artifact = EXartifact[i];
                            GameManager.Instance.AddDroppedArtifact(EXartifact[i], 1);
                        }
                        EXartifact[i].Quantity += 1;
                    }
                }
            }

            GiftBox();
        }

        if (iskali && !LTimer)
        {
            if (type == EnemyType.Boss)
            {
                GameObject soulball = Instantiate(GameManager.instance.bosssoulballPrefab, GameManager.instance.kariii.transform.position, Quaternion.identity, GameManager.instance.kariii.transform);
                GameManager.instance.kariii.Bossgetsoul();
            }
            else
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                if (randomValue <= 0.05f)
                {
                    GameObject soulball = Instantiate(soulballPrefab, GameManager.instance.kariii.transform.position, Quaternion.identity, GameManager.instance.kariii.transform);
                    GameManager.instance.kariii.getsoul();
                }
            }
        }

        soulballCreated = true;

        if (featherSubPrefab != null && featherSubPrefab.Length > 0)
        {
            if(AttackingTower.i == 14)
            {
                if (AttackingTower.LevelS > 2)
                {
                    isp3 = true;
                }

                GameObject featherBomb = Instantiate(BombPrefab, transform.position, Quaternion.identity);
                OverlapBomb Overlap = featherBomb.GetComponent<OverlapBomb>();

                if (Overlap != null)
                {
                    Overlap.Eneposition = this.spawnPoint.position;
                    Overlap.Initialize(MA1, MA2, MT, isp3);
                }
            }
        }
    }

    public void Inugami()
    {
        if (AttackingTower != null)
        {
            if (!AttackingTower.isEx)
            {
                if (AttackingTower.dicton.myartifact != null)
                {
                    if (AttackingTower.dicton.myartifact.artifactId == "au10")
                    {
                        GameManager.Instance.Nekomata();
                        Instantiate(AttackingTower.dicton.myartifact.vfxarti, transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    public void Fire()
    {
        isFire = true;
        Miko miko = GameManager.instance.miko;
        Instantiate(miko.firePrefab, transform.position, Quaternion.identity, gameObject.transform);
        fireDuration = 2f;
        if (!isFire)
        {
            FireD();
        }
        fireDamageTimer = 0f;
    }

    public void FireD()
    {
        Miko miko = GameManager.instance.miko;
        AttackingTower = miko;
        buruburu = true;
        TakeDamage(miko.Power * miko.myP[4].dmgCoe);
    }

    public void ZenLoss()
    {
        if (type != EnemyType.Boss)
        {
            deathvfxPrefab = GameManager.instance.zenlossPrefab;
            Die();
        }
    }

    public void GiftBox()
    {
        if (!snab)
        {
            switch (type)
            {
                case EnemyType.Normal:
                case EnemyType.Special:
                    switch (DifficultyManager.Instance.Difficulty)
                    {
                        case (int)DifficultyButton.DifficultyLevel.Easy:
                            Probab = 0.0002f;
                            break;
                        case (int)DifficultyButton.DifficultyLevel.Normal:
                            Probab = 0.0008f;
                            break;
                        case (int)DifficultyButton.DifficultyLevel.Hard:
                            Probab = 0.001f;
                            break;
                        case (int)DifficultyButton.DifficultyLevel.Extreme:
                            Probab = 0.003f;
                            break;
                    }
                    break;
                case EnemyType.Boss:
                    switch (DifficultyManager.Instance.Difficulty)
                    {
                        case (int)DifficultyButton.DifficultyLevel.Easy:
                            Probab = 0.05f;
                            break;
                        case (int)DifficultyButton.DifficultyLevel.Normal:
                            Probab = 0.1f;
                            break;
                        case (int)DifficultyButton.DifficultyLevel.Hard:
                            Probab = 0.2f;
                            break;
                        case (int)DifficultyButton.DifficultyLevel.Extreme:
                            Probab = 0.3f;
                            break;
                    }
                    break;
            }
        }
        else
        {
            Probab = 1;
        }

        if (UnityEngine.Random.Range(0f, 1f) <= Probab)
        {
            GameManager.Instance.DropBox();
            Instantiate(chestPrefab, transform.position, Quaternion.identity);
        }
    }

    public override void Heal(float amount, Tower tower)
    {
        if (isEx) { return; }
        base.Heal(amount, tower);
    }

    public virtual void toCollider()
    {
        if (!Laplus)
        {
            float xSize = RangeX * 1.65f;
            float ySize = RangeY * 2.02f;

            if (RangeY % 2 == 0)
            {
                ySize -= 0.3f;
            }

            if (RangeY == 0f)
            {
                ySize = 0.6f;
            }

            if (xSize == 0)
            {
                xSize = 1.3f;
                Vector2 newSize = new Vector2(xSize, ySize);
                boxSize = newSize;

                Vector2 newOffset = new Vector2(0, 0.33f);

                if (newOffset.y == 0)
                {
                    newOffset.y = 0.1f;
                }
                raycastStartOffset = newOffset;
            }
            else
            {
                Vector2 newSize = new Vector2(xSize, ySize);
                boxSize = newSize;
                Vector2 newOffset = new Vector2(-((xSize / 2) + (centerX * 1.65f)), centerY * 2.02f);

                if (newOffset.y == 0)
                {
                    newOffset.y = 0.1f;
                }
                raycastStartOffset = newOffset;
            }
        }
    }

    public void AddFeatherSub(GameObject featherSub)
    {
        if (featherSubPrefab != null)
        {
            System.Array.Resize(ref featherSubPrefab, featherSubPrefab.Length + 1);
            featherSubPrefab[featherSubPrefab.Length - 1] = featherSub;
        }
        else
        {
            featherSubPrefab = new GameObject[1] { featherSub };
        }

        featherSubs.Add(featherSub);
    }

    public void resetF()
    {
        if (featherSubPrefab.Length > 2)
        {
            foreach (GameObject sub in featherSubs)
            {
                if (sub != null)
                {
                    Destroy(sub);
                }
            }

            featherSubPrefab = new GameObject[0];
            featherSubs.Clear();
        }
    }

    public int GetFeatherSubCount()
    {
        return featherSubs.Count;
    }
}