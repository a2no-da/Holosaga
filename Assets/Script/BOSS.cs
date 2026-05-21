using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System;
using TMPro;
using System.Globalization;

public class BOSS : Enemy
{
    //공용패턴1, 공용패턴2..., 패턴1, 패턴2...
    public float teleportCooldown = 10f; // 텔레포트 쿨타임.
    public float teleportTimer = 0f; // 텔레포트 타이머.
    public float Pattern1Cooldown = 10f; // 패턴 1 쿨.
    public float Pattern1Timer = 0f; // 패턴 1 타이머.
    public float Pattern2Cooldown = 10f; // 패턴 2 쿨.
    public float Pattern2Timer = 0f; // 패턴 2 타이머.
    public float Pattern3Cooldown = 10f; // 패턴3 쿨.
    public float Pattern3Timer = 0f; // 패턴3 타이머.
    public float Pattern4Cooldown = 10f; // 패턴4 쿨.
    public float Pattern4Timer = 0f; // 패턴4 타이머.
    public float Pattern5Cooldown = 10f; // 패턴5 쿨 (일반몹 소환)
    public float Pattern5Timer = 0f; // 패턴5 타이머.
    private string T;
    public bool IsDead { get; private set; } = false;

    public SpawnPoint[] teleportPoints;
    protected bool isTeleporting = false;

    public Image BossHP;
    public TextMeshProUGUI CurrentHealthText;
    public TextMeshProUGUI MaxHealthText;
    public GameObject HellKnife;

    public static event Action<BOSS> OnBossSpawned;

    public bool shouldConsiderIsing1;
    public bool shouldIgnoreRange1;
    public bool shouldConsiderIsing2;
    public bool shouldIgnoreRange2;
    public bool shouldConsiderIsing3;
    public bool shouldIgnoreRange3;
    public bool shouldConsiderIsing4;
    public bool shouldIgnoreRange4;
    public bool shouldConsiderIsing5;
    public bool shouldIgnoreRange5;
    public bool shouldConsiderIsing6;
    public bool shouldIgnoreRange6;

    public Camera mainCamera;
    public Sprite bossIndicatorSprite;
    private Image bossIndicatorInstance;
    private GameObject bossIndicatorObject;
    public int bossIndex = 0;
    public int myVal = 100;

    public override void Start()
    {
        base.Start();
        OnBossSpawned?.Invoke(this);
        if(!GameManager.Instance.isinfinite)
        {
            BossHP = GameManager.Instance.BossBar.GetComponent<Image>();
            BossHP.fillAmount = 1.0f;
            GameManager.Instance.UpdateHealthTexts(Health, MaxHealth);
        }
        else
        {
            GameManager.Instance.bosbar[bossIndex].BossHpBar.SetActive(true);
            BossHP = GameManager.Instance.bosbar[bossIndex].BossBarr.GetComponent<Image>();
            BossHP.fillAmount = 1.0f;
            GameManager.Instance.inbossUpTexts(Health, MaxHealth, bossIndex);
        }

        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        var monsterCooltimeData = CsvData.Read("MonsterCooltime");

        T = "BossCommon";

        if (!Laplus)
        {
            foreach (var val in monsterCooltimeData)
            {
                if (T.Equals(val["Pattern_user"]))
                {
                    mC[10] = new MonsterCooltime.Data
                    {
                        ID = int.Parse(val["ID"]),
                        dscName = val["Desc_name"],
                        IgnRange = int.Parse(val["IgnoreRange"]),
                        IsIndep = int.Parse(val["IsIndependent"]),
                        initCool = float.Parse(val["Cooltime_init"], CultureInfo.InvariantCulture),
                        cool = float.Parse(val["Cooltime"], CultureInfo.InvariantCulture),
                        P_user = val["Pattern_user"]
                    };
                }
            }

            Pattern1Timer = mC[2].initCool;
            Pattern2Timer = mC[3].initCool;
            Pattern3Timer = mC[4].initCool;
            Pattern4Timer = mC[5].initCool;
            Pattern5Timer = mC[1].initCool;
            teleportTimer = mC[10].initCool;

            Pattern1Cooldown = mC[2].cool;
            Pattern2Cooldown = mC[3].cool;
            Pattern3Cooldown = mC[4].cool;
            Pattern4Cooldown = mC[5].cool;
            Pattern5Cooldown = mC[1].cool;
            teleportCooldown = mC[10].cool;

            shouldConsiderIsing1 = mC[1].IsIndep == 0;
            shouldIgnoreRange1 = mC[1].IgnRange == 0;
            shouldConsiderIsing2 = mC[2].IsIndep == 0;
            shouldIgnoreRange2 = mC[2].IgnRange == 0;
            shouldConsiderIsing3 = mC[3].IsIndep == 0;
            shouldIgnoreRange3 = mC[3].IgnRange == 0;
            shouldConsiderIsing4 = mC[4].IsIndep == 0;
            shouldIgnoreRange4 = mC[4].IgnRange == 0;
            shouldConsiderIsing5 = mC[5].IsIndep == 0;
            shouldIgnoreRange5 = mC[5].IgnRange == 0;
            shouldConsiderIsing6 = mC[10].IsIndep == 0;
            shouldIgnoreRange6 = mC[10].IgnRange == 0;
        }

        mainCamera = Camera.main;

        if (bossIndicatorSprite != null)
        {
            bossIndicatorObject = new GameObject("BossIndicator");
            bossIndicatorObject.transform.SetParent(GameObject.Find("UICanvas").transform, false);
            bossIndicatorInstance = bossIndicatorObject.AddComponent<Image>();
            bossIndicatorInstance.sprite = bossIndicatorSprite;
            bossIndicatorInstance.gameObject.SetActive(false);
        }
    }

    void HandleAnimationEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "teleport":
                PerformTeleport();
                break;
        }
    }

    void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (!Laplus && !snab)
        {
            if (trackEntry.Animation.Name == AnimClip[6].name)
            {
                isTeleporting = false;
                ising = false;
                SetAnimation(0, AnimClip[0], true);
            }
        }
    }

    public override void Update()
    {
        if (HealthBarImage != null)
        {
            HealthBarImage.fillAmount = Health / MaxHealth;

            if (Health <= 0)
            {
                if (unitType != UnitType.Tower)
                {
                }
            }
            else
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(HealthBarPosition.position);
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hpCanvas.GetComponent<RectTransform>(),
                screenPos,
                Camera.main, out localPoint))
                {
                    RectTransform instanceRect = HealthBarInstance.GetComponent<RectTransform>();
                    instanceRect.anchoredPosition = localPoint;
                }
            }
        }

        if (!Laplus)
        {
            if (transform.position.x <= endPointX)
            {
                GameManager.Instance.OnMonsterReached(MainDamage);
                IsDead = true;
                Destroy(gameObject);

                if (GameManager.Instance.isinfinite)
                {
                    GameManager.Instance.bosbar[bossIndex].BossHpBar.SetActive(false);
                }
            }

            if ((!ising || !shouldConsiderIsing2) && Pattern1Timer > 0 && !isStunned && !isTeleporting)
            {
                Pattern1Timer -= Time.deltaTime;
            }

            if ((!ising || !shouldConsiderIsing3) && Pattern2Timer > 0 && !isStunned && !isTeleporting)
            {
                Pattern2Timer -= Time.deltaTime;
            }

            if ((!ising || !shouldConsiderIsing4) && Pattern3Timer > 0 && !isStunned && !isTeleporting)
            {
                Pattern3Timer -= Time.deltaTime;
            }

            if ((!ising || !shouldConsiderIsing5) && Pattern4Timer > 0 && !isStunned && !isTeleporting)
            {
                Pattern4Timer -= Time.deltaTime;
            }

            if ((!ising || !shouldConsiderIsing1) && Pattern5Timer > 0 && !isStunned && !isTeleporting)
            {
                Pattern5Timer -= Time.deltaTime;
            }

            if ((!ising || !shouldConsiderIsing6) && teleportTimer > 0 && !isStunned && !isTeleporting)
            {
                teleportTimer -= Time.deltaTime;
            }

            if (!ising && teleportTimer <= 0 && !isStunned && (targetEnemy != null || !shouldIgnoreRange6) && !isTeleporting)
            {
                Teleport();
                teleportTimer = teleportCooldown;
            }
            else if (isStunned)
            {
                teleportTimer = teleportCooldown;
                isTeleporting = false;
            }
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

        Vector3 bossViewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (bossViewportPosition.x > 1 && bossIndicatorInstance != null)
        {
            RectTransform rectTransform = bossIndicatorInstance.rectTransform;

            rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(1, 0.5f);

            rectTransform.anchoredPosition = new Vector2(-50f, 150);

            bossViewportPosition.x = Mathf.Clamp(bossViewportPosition.x, 0f, 1f);
            bossViewportPosition.y = Mathf.Clamp(bossViewportPosition.y, 0f, 1f);

            bossIndicatorInstance.gameObject.SetActive(true);
            bossIndicatorInstance.rectTransform.anchorMin = bossIndicatorInstance.rectTransform.anchorMax = bossViewportPosition;
            bossIndicatorInstance.rectTransform.anchoredPosition = Vector2.zero;
        }
        else if (bossIndicatorInstance != null)
        {
            bossIndicatorInstance.gameObject.SetActive(false);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (!GameManager.Instance.isinfinite)
        {
            if(BossHP != null)
            {
                BossHP.fillAmount = Health / MaxHealth;
            }
            GameManager.Instance.UpdateBossHP(Health / MaxHealth);
            GameManager.Instance.UpdateHealthTexts(Health, MaxHealth);
        }
        else
        {
            BossHP.fillAmount = Health / MaxHealth;
            GameManager.Instance.inpUpdateBossHP(Health / MaxHealth, bossIndex);
            GameManager.Instance.inbossUpTexts(Health, MaxHealth, bossIndex);
        }

        if (Health <= 0 && type == EnemyType.Boss)
        {
            Die();
        }
    }

    public virtual void Teleport()
    {
        if (!isTeleporting && !Laplus)
        {
            isTeleporting = true;
            ising = true;
            SetAnimation(0, AnimClip[6], true);
        }
    }

    private void PerformTeleport()
    {
        if (isStunned)
        {
            return;
        }

        List<SpawnPoint> possiblePositions = new List<SpawnPoint>(teleportPoints);
        possiblePositions.RemoveAll(sp => Mathf.Abs(sp.position.y - transform.position.y) < Mathf.Epsilon);

        int randomIndex = UnityEngine.Random.Range(0, possiblePositions.Count);

        SpawnPoint chosenPoint = possiblePositions[randomIndex];
        transform.position = new Vector3(transform.position.x, chosenPoint.position.y, transform.position.z);

        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = chosenPoint.sortingOrder;
        }
    }

    protected override void Die()
    {
        base.Die();

        IsDead = true;
        if (bossIndicatorObject != null)
        {
            Destroy(bossIndicatorObject);
        }
        Instantiate(HellKnife, Vector3.zero, Quaternion.identity);

        if (GameManager.Instance.isinfinite)
        {
            GameManager.Instance.bosbar[bossIndex].BossHpBar.SetActive(false);
        }
        GameManager.Instance.GiveHM += myVal;
    }
}

