using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Spine.Unity;
using Spine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Sandbag : MonoBehaviour
{
    private bool isSpawning = false;
    private int spawnedMonsterCount = 0;
    private int spawnStageIndex;
    private float stageStartTime;
    public AudioClip EndClip;

    private bool isBreakStage = false;
    private float breakStageTimer = 0f;

    private bool isCountdownTimer = false;
    private float countdownTimer = 0f;
    public float stageTimer = 0f;

    public SpawnPoint[] spawnPoints;
    public Stage[] stages;

    public int mainStage = 0;
    public int subStage = 0;
    public int totalMonsterCount;
    public TMP_Text currentStageText;
    public TMP_Text holomoneyText;
    private int bossStageIndex = 5;
    private List<int> usedIndices = new List<int>();
    private bool isBossDead = false;

    public int holomoneyValue = 0;
    public Holomoney holomoney;
    public StageBar stageBar;
    bool hasAnimationPlayed = false;
    public SkeletonAnimation skeletonAnimation;

    public GameObject stageBarr;
    public GameObject BossHpBar;
    public GameObject gameOverPanel;
    public Image BossBar;

    public GameObject gameOverImage;
    public GameObject gameClearImage;
    private DateTime gameStartTimeUTC;
    public TMP_Text elapsedStageText;
    public TMP_Text ClearTimeText;
    public Image Setting;

    public AudioSource audioSource;
    public float fadeDuration = 1f;
    public AudioClip normalStageClip;
    public AudioClip BossClip;
    public GameObject LaBack;

    private bool isPlay = false;
    public ArtifactManager artifactManager;
    private List<Tile> selectedTiles;
    public GameObject laBackObject;
    private bool DEND = false;
    public UpdatePanel updatePanel;
    public Image bosBar;
    public float endTimer = 0f; 
    public bool startEndTimer = false;
    public Image timerImage;
    private string currentLanguage;
    public Vector3 laplaceSpawnPosition;
    public GameObject whatSt;
    public GameObject timerrr;

    void Start()
    {
        isBreakStage = true; //false
        currentStageText.text = "Break Stage";
        artifactManager.LoadArtifact();

        float maxTime = 15;

        PlayMusic(normalStageClip);
        stageBar.SetMaxTime(maxTime);
        StartCoroutine(IncreaseProgressBar());

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        ChangeAnimationBasedOnLanguage("announce_bossRaidStart");
        gameStartTimeUTC = DateTime.UtcNow;
        bosBar.fillAmount = 0;
        laplaceSpawnPosition = new Vector3(5.775f, -3.38f, 0);
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        currentLanguage = newLocale.Identifier.Code;
    }

    private void ChangeAnimationBasedOnLanguage(string baseAnimationName)
    {
        string animationName = baseAnimationName;

        switch (currentLanguage)
        {
            case "ko":
                animationName = $"{baseAnimationName}Korean";
                break;
            case "en":
                animationName = $"{baseAnimationName}English";
                break;
            case "ja":
                animationName = $"{baseAnimationName}Japanese";
                break;
            default:
                animationName = $"{baseAnimationName}Korean"; 
                break;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
    }

    IEnumerator IncreaseProgressBar()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            stageBar.IncreaseProgress(1);
        }
    }

    private void Update()
    {
        if (isSpawning)
        {
            if (spawnedMonsterCount >= totalMonsterCount)
            {
                isSpawning = false;
            }
        }

        if (IsBossStage())
        {
            UpdateBossBar();
        }

        if (isBreakStage)
        {
            breakStageTimer += Time.deltaTime;
            BossHpBar.gameObject.SetActive(false);
            if (breakStageTimer >= 10f && !hasAnimationPlayed)
            {
                hasAnimationPlayed = true;
                skeletonAnimation.state.SetAnimation(0, "announce_Count", false);
            }

            if (breakStageTimer >= 15f)
            {
                isBreakStage = false;
                breakStageTimer = 0f;
                hasAnimationPlayed = false;
                updatePanel.resetSand();
                startEndTimer = true;
                endTimer = 120f;
                whatSt.SetActive(false);
                timerrr.SetActive(true);
                GameManager.Instance.mainStage = 0;
                NextStage();
                stageTimer = 0f;
            }
        }
        else if (!isBreakStage)
        {
            if (mainStage != 5)
            {
                bool isBossDead = IsBossDead();

                if (isBossDead)
                {
                    if (mainStage != 5 && subStage != 5)
                    {
                        subStage++;
                        NextStage();
                        stageTimer = 0f;
                    }

                    GameManager.Instance.subStage = subStage;
                    GameManager.Instance.mainStage = mainStage;
                }
            }
        }

        if (startEndTimer)
        {
            endTimer -= Time.deltaTime; 
            if (endTimer > 0f)
            {
                timerImage.fillAmount = endTimer / 120f;
            }
            else
            {
                startEndTimer = false;
                timerImage.fillAmount = 0; 
                if (mainStage == 5 && subStage == 0)
                {
                    End();
                    gameClearImage.SetActive(true);

                    DateTime gameEndTimeUTC = DateTime.UtcNow;
                    TimeSpan clearTime = gameEndTimeUTC - gameStartTimeUTC;
                    string clearTimeString = string.Format("{0:D2}:{1:D2}", clearTime.Minutes, clearTime.Seconds);

                    elapsedStageText.text = " : Boss Stage 25";
                    ClearTimeText.text = $" : {clearTimeString}";
                    Time.timeScale = 0f;
                }
                else
                {
                    GameOver();
                }
            }
        }

        if (isCountdownTimer)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                isCountdownTimer = false;
                countdownTimer = 0;
            }
        }

        if (GameManager.Instance.isBreakSta != isBreakStage)
        {
            GameManager.Instance.isBreakSta = isBreakStage;
        }

        if (DEND)
        {
            Time.timeScale = 0f;
        }
    }

    private bool IsBossStage()
    {
        return !isBreakStage; 
    }

    private List<Tile> GetValidTiles()
    {
        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        return allTiles;
    }

    public void StartSpawningMonsters(int stageIndex)
    {
        spawnStageIndex = stageIndex;
        isSpawning = true;
        spawnedMonsterCount = 0;

        SpawnMonster(stageIndex);
    }

    public void StartBreakStage()
    {
        isBreakStage = true;
        breakStageTimer = 0f;
    }

    public void StartCountdownTimer(float duration)
    {
        isCountdownTimer = true;
        countdownTimer = duration;
    }

    private void SpawnMonster(int stageIndex)
    {
        GameObject bossPrefab = stages[mainStage].subStages[stageIndex].monsterPrefabs[0];
        GameObject boss = Instantiate(bossPrefab, laplaceSpawnPosition, Quaternion.identity);
        Debug.Log(boss);
        MeshRenderer meshRenderer = boss.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = 2;
        }

        spawnedMonsterCount++;
    }

    public void NextStage()
    {
        if (isSpawning) return;
        if (subStage == bossStageIndex)
        {
            mainStage++;
            bosBar.fillAmount = 0;
            subStage = 0;
            GameManager.Instance.mainStage = mainStage + 1;
            GameManager.Instance.subStage = subStage;
            bosBar.fillAmount = 0;
        }

        PlayMusic(BossClip);
        skeletonAnimation.state.SetAnimation(0, "announce_bossStage", false);
        if (mainStage != 5)
        {
            StartSpawningMonsters(subStage);
            stageBar.SetMaxTime(stages[mainStage].subStages[subStage].stageDuration);
            currentStageText.text = $"Boss Stage {subStage + 1 + (mainStage * 5)}";
            stageBarr.gameObject.SetActive(false);
            BossHpBar.gameObject.SetActive(true);
            bosBar.fillAmount = subStage / 5f;
            BossBar.fillAmount = 1.0f;
        }
    }

    private int CalculateHolomoney()
    {
        return 0;
    }

    private void UpdateBossBar()
    {
        BOSS laplaceBoss = FindObjectOfType<BOSS>(); 
        if (laplaceBoss != null)
        {
            float bossMaxHealth = laplaceBoss.MaxHealth; 
            float bossCurrentHealth = laplaceBoss.Health;

            if (bossMaxHealth <= 0) return;

            float stageStartProgress = (float)subStage / 5f; 
            float stageProgress = (1f / 5f) * (1f - (bossCurrentHealth / bossMaxHealth));

            bosBar.fillAmount = stageStartProgress + stageProgress;
        }
    }

    public bool IsBossDead()
    {
        BOSS bossMonster = FindObjectOfType<BOSS>();
        if (bossMonster == null || bossMonster.IsDead)
        {
            if (!isBossDead)
            {
                if (subStage != (bossStageIndex + 1))
                {
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                }
                else
                {
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                }
            }

            isBossDead = false;
            return true;
        }

        return false;
    }

    private void End()
    {
        gameOverPanel.SetActive(true);

        artifactManager.SaveArtifact();
        int holomoney = CalculateHolomoney();
        holomoneyText.text = holomoney.ToString();
        DifficultyManager.Instance.ResetDifficulty();
        GameManager.Instance.TextBoxend();
        GameManager.Instance.PrintDroppedArtifacts();
        DEND = true;
    }

    public void obuJoku()
    {
        bool IceWater = true;
        List<string> requiredIWUnits = new List<string> { "페코라", "구라", "코보", "라미" };

        foreach (var unitId in requiredIWUnits)
        {
            if (!GameManager.Instance.selectedUnitIds.Contains(unitId))
            {
                IceWater = false;
                break;
            }
        }

        if (IceWater)
        {
            Joku.Instance.Qok(38);
        }
    }

    public void GameOver()
    {
        int dex;

        gameOverImage.SetActive(true);
        Time.timeScale = 0f;
        dex = subStage + (mainStage * 5);

        elapsedStageText.text = " : Boss Stage " + dex.ToString();

        if (mainStage <= 0 && dex <= 0)
        {
            elapsedStageText.text = " : Break Stage";
        }

        DateTime gameEndTimeUTC = DateTime.UtcNow;
        TimeSpan clearTime = gameEndTimeUTC - gameStartTimeUTC;
        string clearTimeString = string.Format("{0:D2}:{1:D2}", clearTime.Minutes, clearTime.Seconds);

        ClearTimeText.text = $" : {clearTimeString}";
        Time.timeScale = 0f;

        End();
    }

    public void GiveUp()
    {
        Setting.gameObject.SetActive(false);
        GameOver();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            return;
        }

        if (isPlay)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void Down()
    {
        isPlay = true;
        StartCoroutine(FadeOut(audioSource, fadeDuration));
    }

    public void Up()
    {
        if (!audioSource.isPlaying)
        {
            isPlay = false;
            audioSource.Play();
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
