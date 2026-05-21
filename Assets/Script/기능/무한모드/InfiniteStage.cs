using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Spine.Unity;
using Spine;

public class InfiniteStage : MonoBehaviour
{
    private bool isSpawning = false;
    private float spawnTimer = 0.0f;
    private int spawnedMonsterCount = 0;
    private int currentMonsterCount = 0;
    private float spawnDuration;
    private int spawnStageIndex;
    private float stageStartTime;

    private bool isCountingDown = false;
    private float countdownTime = 0f;
    private bool isStartCountdown = false;
    private bool Start10stage = false;
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
    private int bossStageIndex = 4;
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
    public GameObject[] BackEx;
    public bool Disast;
    public int DisasterSel;
    public GameObject Disaster;
    public float[] Disastercool;
    public float[] Disaster_cooltime; //고정
    public Disaster_Position DP;
    private List<Tile> selectedTiles;
    public GameObject laBackObject;
    private int inpe = 0;
    public GameObject[] BossList;
    private bool tens = false;
    public GameObject CardSPanel;
    public SelCardSt selCardSt;
    public SelAbilityCard selAbilityCard;
    public UpdatePanel updatePanel;
    public GameObject statpanel;
    public GameObject EPPrefab;
    public AudioClip[] RandomClip;
    private bool DEND = false;

    [System.Serializable]
    public class Disaster_Position
    {
        public Vector2[] one;
        public Vector2[] two;
        public Vector2[] three;
    }

    void Start()
    {
        isBreakStage = true; //false
        currentStageText.text = "Break Stage";
        artifactManager.LoadArtifact();

        for (int i = 0; i < BackEx.Length; i++)
        {
            BackEx[i].SetActive(false);
        }
        int randomIndex = UnityEngine.Random.Range(0, BackEx.Length);
        normalStageClip = RandomClip[randomIndex];
        BackEx[randomIndex].SetActive(true);

        float maxTime = 15;

        PlayMusic(normalStageClip);
        stageBar.SetMaxTime(maxTime);
        StartCoroutine(IncreaseProgressBar());
        skeletonAnimation.state.SetAnimation(0, "announce_gameStart", false);
        gameStartTimeUTC = DateTime.UtcNow;
        DisasterSel = UnityEngine.Random.Range(0, 3);

        switch (DifficultyManager.Instance.Difficulty)
        {
            case (int)DifficultyButton.DifficultyLevel.Easy:
            case (int)DifficultyButton.DifficultyLevel.Normal:
            case (int)DifficultyButton.DifficultyLevel.Hard:
                Disast = false;
                break;
            case (int)DifficultyButton.DifficultyLevel.Extreme:
                Disast = true;
                break;
        }
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
            if (subStage == (bossStageIndex + 1))
            {
                if(mainStage < 5)
                {
                    SpawnMonster(spawnStageIndex);
                    isSpawning = false;
                }
                else
                {
                    spawnTimer += Time.deltaTime;
                    float averageSpawnInterval = 0.1f / totalMonsterCount;
                    if (spawnTimer >= averageSpawnInterval && spawnedMonsterCount < totalMonsterCount)
                    {
                        SpawnMonster(spawnStageIndex);
                        spawnTimer = 0.0f;
                    }

                    if (spawnedMonsterCount >= totalMonsterCount)
                    {
                        isSpawning = false;
                    }
                }
            }
            else
            {
                spawnTimer += Time.deltaTime;
                float averageSpawnInterval = 15.0f / totalMonsterCount;
                if (spawnTimer >= averageSpawnInterval && spawnedMonsterCount < totalMonsterCount)
                {
                    SpawnMonster(spawnStageIndex);
                    spawnTimer = 0.0f;
                }

                if (spawnedMonsterCount >= totalMonsterCount)
                {
                    isSpawning = false;
                }
            }
        }

        if (mainStage != 9)
        {
            if (subStage == (bossStageIndex + 1))
            {
                if(isSpawning) { return; }
                bool isBossDead = false;
                isBossDead = IsBossDead();

                if (isBossDead)
                {
                    if (laBackObject != null)
                    {
                        Destroy(laBackObject);
                        laBackObject = null;
                    }

                    NextStage();
                    stageTimer = 0f;
                }
            }
            else
            {
                stageTimer += Time.deltaTime;
                if (stageTimer >= 45f)
                {
                    NextStage();
                    stageTimer = 0f;
                }
            }
        }
        else
        {
            if(!tens)
            {
                stageTimer += Time.deltaTime;
                if (stageTimer >= 45f)
                {
                    NextStage();
                    stageTimer = 0f;
                }
            }
            else
            {
                bool isBossDead = false;
                isBossDead = IsBossDead();

                if (isBossDead)
                {
                    if (laBackObject != null && mainStage != 9)
                    {
                        Destroy(laBackObject);
                        laBackObject = null;
                    }
                    tens = false;
                    NextStage();
                    stageTimer = 0f;
                }
            }
        }

        if (isCountingDown)
        {
            if (countdownTime > 0f)
            {
                countdownTime -= Time.deltaTime;

                TimeSpan timeSpan = TimeSpan.FromSeconds(countdownTime);

                if (countdownTime <= 0f)
                {
                    NextStage();
                    isCountingDown = false;
                }
            }
        }

        if (isBreakStage)
        {
            unlocksw();
            breakStageTimer += Time.deltaTime;

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
                locksw();
                NextStage();
                stageTimer = 0f;

                if (selCardSt.test && mainStage == 0)
                {
                    Time.timeScale = 0f;
                    selCardSt.CardAPanel.gameObject.SetActive(true);
                    selAbilityCard.Fir();
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

        if (Disast)
        {
            if (isBreakStage) { return; }

            Disastercool[DisasterSel] -= Time.deltaTime;
            if (Disastercool[DisasterSel] <= 0)
            {
                Disastercool[DisasterSel] = Disaster_cooltime[DisasterSel];

                Vector2[] selectedPositions = null;
                switch (DisasterSel)
                {
                    case 0:
                        List<Tile> validTiles = GetValidTiles();
                        if (validTiles.Count > 0)
                        {
                            int randomTileIndex = UnityEngine.Random.Range(0, validTiles.Count);
                            Tile selectedTile = validTiles[randomTileIndex];

                            GameObject disaster = Instantiate(Disaster, selectedTile.transform);
                            disaster.transform.localPosition = Vector3.zero;
                            if (disaster != null)
                            {
                                disaster.GetComponent<Disasters>().Initialize(DisasterSel);
                            }
                        }
                        break;
                    case 1:
                        selectedPositions = DP.two;
                        break;
                    case 2:
                        selectedPositions = DP.three;
                        break;
                }

                if (selectedPositions != null && selectedPositions.Length > 0 && DisasterSel != 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, selectedPositions.Length);
                    Vector2 spawnPosition = selectedPositions[randomIndex];

                    GameObject disaster = Instantiate(Disaster, new Vector3(spawnPosition.x, spawnPosition.y, 0f), Quaternion.identity);
                    if (disaster != null)
                    {
                        disaster.GetComponent<Disasters>().Initialize(DisasterSel);
                    }
                }
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

    public void unlocksw()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                if (updatePanel.AllTowers[i].isSW)
                {
                    updatePanel.AllTowers[i].isSW = false;
                }
            }
        }
    }

    public void locksw()
    {
        for (int i = 0; i < updatePanel.AllTowers.Count; i++)
        {
            if (updatePanel.AllTowers[i] != null)
            {
                if (updatePanel.AllTowers[i].isCardSW)
                {
                    updatePanel.AllTowers[i].isSW = true;
                    Instantiate(EPPrefab, updatePanel.AllTowers[i].transform.position, Quaternion.identity, updatePanel.AllTowers[i].transform);
                }
            }
        }
    }

    private List<Tile> GetValidTiles()
    {
        List<Tile> allTiles = new List<Tile>(LevelManager.Instance.Tiles.Values);
        return allTiles;
    }

    public void StartSpawningMonsters(int stageIndex, float duration)
    {
        stageStartTime = Time.time;
        spawnStageIndex = stageIndex;
        spawnDuration = duration;
        isSpawning = true;
        spawnedMonsterCount = 0;
        spawnTimer = 0.0f;

        stageTimer = 0f;

        List<int> usedIndices = new List<int>();
        totalMonsterCount = stages[mainStage].subStages[stageIndex].monsterCounts.Sum();
        currentMonsterCount = totalMonsterCount;
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
        if (stages[mainStage].subStages[stageIndex].monsterPrefabs[0].name.StartsWith("La+"))
        {
            Vector3 laplaceSpawnPosition = new Vector3(5.775f, -3.38f, 0);
            GameObject laplacePrefab = stages[mainStage].subStages[stageIndex].monsterPrefabs[0];

            GameObject laplace = Instantiate(laplacePrefab, laplaceSpawnPosition, Quaternion.identity);

            laBackObject = Instantiate(LaBack, transform.position, Quaternion.identity);
            stageBarr.gameObject.SetActive(false);

            MeshRenderer meshRenderer = laplace.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = 2;
            }

            stages[mainStage].subStages[stageIndex].monsterCounts[0]--;
            spawnedMonsterCount++;
        }
        else
        {
            int randomPrefabIndex;
            do
            {
                randomPrefabIndex = UnityEngine.Random.Range(0, stages[mainStage].subStages[stageIndex].monsterPrefabs.Length);
            } while (stages[mainStage].subStages[stageIndex].monsterCounts[randomPrefabIndex] <= 0);

            int randomSpawnIndex;
            do
            {
                randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            } while (usedIndices.Contains(randomSpawnIndex));

            SpawnPoint spawnPoint = spawnPoints[randomSpawnIndex];
            GameObject monster = Instantiate(stages[mainStage].subStages[stageIndex].monsterPrefabs[randomPrefabIndex], spawnPoint.position, Quaternion.identity);

            MeshRenderer meshRenderer = monster.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = spawnPoint.sortingOrder;
            }

            if (mainStage != 9)
            {
                stages[mainStage].subStages[stageIndex].monsterCounts[randomPrefabIndex]--;
            }

            usedIndices.Add(randomSpawnIndex);
            if (usedIndices.Count == spawnPoints.Length)
                usedIndices.Clear();

            spawnedMonsterCount++;
        }
    }

    public void MonsterDeath()
    {
        if (subStage != 0)
        {
            if (subStage != (bossStageIndex + 1) && !isBreakStage)
            {
                currentMonsterCount--;

                if (currentMonsterCount <= 0 && spawnedMonsterCount >= totalMonsterCount)
                {
                    float elapsedTime = Time.time - stageStartTime;
                    float remainingTime = 45 - elapsedTime;

                    if(remainingTime > 0)
                    {
                        stageBar.IncreaseProgress(remainingTime);
                    }
                    NextStage();
                }
            }
        }
    }

    public void NextStage()
    {
        if(isSpawning) { return; }
        if (mainStage == 4)
        {
            bossStageIndex = 0;
        }
        else
        {
            bossStageIndex = 4;
        }

        if (mainStage != 9)
        {
            if (subStage == (bossStageIndex + 1))
            {
                PlayMusic(normalStageClip);
                mainStage++;
                subStage = 0;
                StartBreakStage();
                currentStageText.text = "Break Stage";

                float maxTime = 15;
                stageBar.SetMaxTime(maxTime);
                GameManager.Instance.mainStage = mainStage + 1;
                GameManager.Instance.subStage = subStage;
            }
            else
            {
                if (subStage == bossStageIndex)
                {
                    PlayMusic(BossClip);
                    skeletonAnimation.state.SetAnimation(0, "announce_bossStage", false);
                    StartSpawningMonsters(subStage, stages[mainStage].subStages[subStage].stageDuration);
                    currentStageText.text = "Boss Stage " + (mainStage + 1).ToString();

                    stageBar.SetMaxTime(1);
                    stageBarr.gameObject.SetActive(false);

                }
                else
                {
                    PlayMusic(normalStageClip);
                    StartSpawningMonsters(subStage, stages[mainStage].subStages[subStage].stageDuration);
                    currentStageText.text = "Stage " + (mainStage + 1).ToString() + " - " + (subStage + 1).ToString();

                    if (subStage == 0)
                    {
                        stageBarr.gameObject.SetActive(true);

                        float maxTime = 45;
                        for (int i = 2; i < stages[mainStage].subStages.Length; i++)
                        {
                            maxTime += stages[mainStage].subStages[i].stageDuration;
                        }
                        stageBar.SetMaxTime(maxTime);
                    }
                    else
                    {
                        skeletonAnimation.state.SetAnimation(0, "announce_NextRound", false);
                    }
                }
                subStage++;
                GameManager.Instance.subStage = subStage;
            }
        }
        else
        {
            if(!Start10stage)
            {
                normalStageClip = EndClip;
                laBackObject = Instantiate(LaBack, transform.position, Quaternion.identity);
                Start10stage = true;
            }

            if (inpe > 0 && (inpe + 1) % 10 == 0)
            {
                inpe++;
                PlayMusic(normalStageClip);
                StartBreakStage();
                currentStageText.text = "Break Stage";

                float maxTime = 15;
                stageBar.SetMaxTime(maxTime);
            }
            else
            {
                inpe++;
                if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
                {
                    int randomIndex = UnityEngine.Random.Range(0, BossList.Length);
                    GameObject boss = Instantiate(BossList[randomIndex]);

                    /*if (randomIndex == 4) 
                    {
                        Vector3 laplaceSpawnPosition = new Vector3(5.775f, -3.38f, 0);
                        boss.transform.position = laplaceSpawnPosition;

                        laBackObject = Instantiate(LaBack, transform.position, Quaternion.identity);
                        MeshRenderer meshRenderer = boss.GetComponentInChildren<MeshRenderer>();
                        if (meshRenderer != null)
                        {
                            meshRenderer.sortingOrder = 2;
                        }
                    }
                    else
                    {*/
                    int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
                    boss.transform.position = spawnPoints[randomSpawnIndex].position;
                    //}

                    tens = true;
                    PlayMusic(BossClip);
                    skeletonAnimation.state.SetAnimation(0, "announce_bossStage", false);
                    stageBar.SetMaxTime(1);
                    stageBarr.gameObject.SetActive(false);
                    BossBar.fillAmount = 1.0f;
                }
                else
                {
                    stageBarr.gameObject.SetActive(true);

                    PlayMusic(normalStageClip);
                    skeletonAnimation.state.SetAnimation(0, "announce_NextRound", false);
                }

                stageBar.SetMaxTime(45);

                StartSpawningMonsters(subStage, stages[mainStage].subStages[subStage].stageDuration);
                currentStageText.text = "Stage " + (mainStage + 1).ToString() + " - " + (subStage + inpe).ToString();
                GameManager.Instance.subStage = subStage + inpe;
            }
        }
    }

    public void StartCountdown(float duration, bool startCountdown = false)
    {
        countdownTime = duration;
        isStartCountdown = startCountdown;
        isCountingDown = true;
    }

    private int CalculateHolomoney()
    {
        holomoneyValue = 0;
        for (int mainIndex = 0; mainIndex < stages.Length && mainIndex <= mainStage; mainIndex++)
        {
            for (int subIndex = 0; subIndex < stages[mainIndex].subStages.Length; subIndex++)
            {
                if (mainIndex == 4 && subIndex == 0)
                {
                }
                else if (subIndex == 4)
                {
                }
                else
                {
                    if (mainIndex < mainStage || (mainIndex == mainStage && subStage > subIndex))
                    {
                        holomoneyValue += 20;
                    }
                }
            }
        }

        holomoneyValue = holomoneyValue + GameManager.Instance.GiveHM;

        if (inpe - 1 > 0)
        {
            holomoneyValue += (inpe - 1) * 20;
        }

        switch (DifficultyManager.Instance.Difficulty)
        {
            case (int)DifficultyButton.DifficultyLevel.Easy:
                holomoneyValue = (int)(holomoneyValue * 0.5f);
                break;
            case (int)DifficultyButton.DifficultyLevel.Normal:
                holomoneyValue = (int)(holomoneyValue * 0.7f);
                break;
            case (int)DifficultyButton.DifficultyLevel.Hard:
                break;
            case (int)DifficultyButton.DifficultyLevel.Extreme:
                holomoneyValue = (int)(holomoneyValue * 1.5f);
                break;
        }

        PlayerData data = holomoney.LoadPlayerData();
        data.holomoney += holomoneyValue + GameManager.Instance.plusatim;
        holomoney.SavePlayerData(data);

        return holomoneyValue + GameManager.Instance.plusatim;
    }

    public bool IsBossDead()
    {
        BOSS bossMonster = FindObjectOfType<BOSS>();

        if (bossMonster == null || bossMonster.IsDead)
        {
            stageBarr.gameObject.SetActive(true);

            if (!isBossDead)
            {
                if (subStage != (bossStageIndex + 1))
                {
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                }
                else
                {
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false).Complete += AnimationComplete;
                    void AnimationComplete(TrackEntry trackEntry)
                    {
                        CardSPanel.SetActive(true);
                        selCardSt.Ran();
                        Time.timeScale = 0f;
                    }
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

    public void coru()
    {
        statpanel.SetActive(true);

        //StartCoroutine(FadeOutStatPanel());
    }

    private IEnumerator FadeOutStatPanel()
    {
        CanvasGroup canvasGroup = statpanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = statpanel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        float fadeDuration = 1f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - (elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f; 
        statpanel.SetActive(false); 
    }

    public void GameOver()
    {
        int dex;
        int maindex;

        gameOverImage.SetActive(true);
        Time.timeScale = 0f;
        dex = subStage;
        maindex = mainStage + 1;
        if (mainStage == 9)
        {
            maindex = 10;
            dex = inpe;
        }
        dex -= 1;

        elapsedStageText.text = " : Stage " + maindex.ToString() + " - " + dex.ToString();

        if (mainStage <= 0 && dex <= 0)
        {
            elapsedStageText.text = " : Break Stage";
        }

        if (mainStage > 0 && dex <= 0)
        {
            maindex -= 1;
            elapsedStageText.text = " : Boss Stage " + maindex.ToString();
        }

        if(mainStage == 9 && dex > 0)
        {
            elapsedStageText.text = " : Stage " + maindex.ToString() + " - " + dex.ToString();
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
