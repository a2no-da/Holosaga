using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Spine.Unity;
using Spine;

public class SpawnM : MonoBehaviour
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

    private bool isBreakStage = false;
    private float breakStageTimer = 0f;

    private bool isCountdownTimer = false;
    private float countdownTimer = 0f;
    private float stageTimer = 0f;

    public SpawnPoint[] spawnPoints;
    public Stage[] stages;

    public int mainStage = 0;
    public int subStage = 0;
    public int totalMonsterCount;
    public TMP_Text currentStageText;
    public TMP_Text holomoneyText;
    private const int bossStageIndex = 4;
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
    private bool isGameEnded = false;
    private bool laplaceSpawned = false;
    private bool za = true;

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
    public GameObject HellKnife;

    private bool isLATestActive = false;
    private bool isPlay = false;
    private int dady;
    public ArtifactManager artifactManager;
    public GameObject[] BackEx;
    public bool Disast;
    public int DisasterSel;
    public GameObject Disaster;
    public float[] Disastercool;
    public float[] Disaster_cooltime; //고정
    public Disaster_Position DP;
    private List<Tile> selectedTiles;
    public AudioClip[] RandomClip;
    private int Nand;
    private bool Eaj = false;
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
        Eaj = false;

        switch (DifficultyManager.Instance.Difficulty)
        {
            case (int)DifficultyButton.DifficultyLevel.Easy:
                Disast = false;
                dady = 4;
                Nand = 21;
                break;
            case (int)DifficultyButton.DifficultyLevel.Normal:
                Nand = 22;
                dady = 5;
                break;
            case (int)DifficultyButton.DifficultyLevel.Hard:
                Disast = false;
                Nand = 23;
                dady = 5;
                break;
            case (int)DifficultyButton.DifficultyLevel.Extreme:
                Disast = true;
                Eaj = true;
                Nand = 24;
                dady = 5;
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
                SpawnMonster(spawnStageIndex);
                isSpawning = false;
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

                if (!za && mainStage == 4)
                {
                    SpawnMonster(spawnStageIndex);
                    isSpawning = false;
                }
            }
        }

        if (isLATestActive)
        {
            if (!laplaceSpawned)
            {
                Instantiate(LaBack, transform.position, Quaternion.identity);
                stageBarr.gameObject.SetActive(false);
                BossHpBar.gameObject.SetActive(true);
                BossBar.fillAmount = 1.0f;
                mainStage = 4;
                currentStageText.text = "Boss Stage " + (mainStage + 1).ToString();
                SpawnMonster(0);
                laplaceSpawned = true;
            }
            return;
        }

        if (subStage == (bossStageIndex + 1) || !za)
        {
            bool isBossDead = false;

            switch (DifficultyManager.Instance.Difficulty)
            {
                case (int)DifficultyButton.DifficultyLevel.Easy:
                    isBossDead = IsBossDeadd();
                    break;
                case (int)DifficultyButton.DifficultyLevel.Normal:
                case (int)DifficultyButton.DifficultyLevel.Hard:
                case (int)DifficultyButton.DifficultyLevel.Extreme:
                    isBossDead = IsBossDead();
                    break;
            }

            if (isBossDead)
            {
                if (!IsGameCleared() && mainStage != dady)
                {
                    NextStage();
                    stageTimer = 0f;
                }
                else
                {
                    if (isGameEnded)
                    {
                        return;
                    }

                    isGameEnded = true;
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false).Complete += AnimationComplete;
                    Instantiate(HellKnife, Vector3.zero, Quaternion.identity);
                    void AnimationComplete(TrackEntry trackEntry)
                    {
                        gameClearImage.SetActive(true);

                        DateTime gameEndTimeUTC = DateTime.UtcNow;
                        TimeSpan clearTime = gameEndTimeUTC - gameStartTimeUTC;
                        string clearTimeString = string.Format("{0:D2}:{1:D2}", clearTime.Minutes, clearTime.Seconds);
                        elapsedStageText.text = " : Boss Stage 5";

                        ClearTimeText.text = $" : {clearTimeString}";
                        Time.timeScale = 0f;

                        End();
                        UZ();
                    }
                }
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

        if (isCountingDown && !isLATestActive)
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

        if (isBreakStage && !isLATestActive)
        {
            if (!za)
            {
                isBreakStage = false;
                return;
            }
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
                NextStage();
            }
        }

        if (isCountdownTimer && !isLATestActive)
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
            if (isBreakStage){ return; }

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

        if(DEND)
        {
            Time.timeScale = 0f;
        }

        if(GameManager.Instance.isBreakSta != isBreakStage)
        {
            GameManager.Instance.isBreakSta = isBreakStage; 
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
        if (mainStage == 4 && !laplaceSpawned)
        {
            Vector3 laplaceSpawnPosition = new Vector3(5.775f, -3.38f, 0);
            GameObject laplacePrefab = stages[mainStage].subStages[stageIndex].monsterPrefabs[0];

            GameObject laplace = Instantiate(laplacePrefab, laplaceSpawnPosition, Quaternion.identity);

            MeshRenderer meshRenderer = laplace.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingOrder = 2;
            }

            stages[mainStage].subStages[stageIndex].monsterCounts[0]--;
            laplaceSpawned = true;
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

            stages[mainStage].subStages[stageIndex].monsterCounts[randomPrefabIndex]--;

            usedIndices.Add(randomSpawnIndex);
            if (usedIndices.Count == spawnPoints.Length)
                usedIndices.Clear();

            spawnedMonsterCount++;
        }
    }

    public void MonsterDeath()
    {
        if (subStage != 0 || mainStage != 4)
        {
            if (subStage != (bossStageIndex + 1) && !isBreakStage)
            {
                currentMonsterCount--;

                if (currentMonsterCount <= 0 && spawnedMonsterCount >= totalMonsterCount)
                {
                    float elapsedTime = Time.time - stageStartTime;
                    float remainingTime = 45 - elapsedTime;
                    stageBar.IncreaseProgress(remainingTime);

                    NextStage();
                }
            }
        }
    } 

    public void NextStage()
    {
        if(laplaceSpawned) { return; }
        if (isLATestActive) { return; }

        if (mainStage == 4)
        {
            if (za)
            {
                if (!za) { return; }
                PlayMusic(normalStageClip);
                StartBreakStage();
                currentStageText.text = "Break Stage";

                float maxTime = 15;
                stageBar.SetMaxTime(maxTime);
                GameManager.Instance.mainStage = mainStage + 1;
                GameManager.Instance.subStage = subStage;
                za = false;
                return;
            }
            else if (!za)
            {
                PlayMusic(BossClip);
                skeletonAnimation.state.SetAnimation(0, "announce_bossStage", false);
                StartSpawningMonsters(subStage, stages[mainStage].subStages[subStage].stageDuration);
                currentStageText.text = "Boss Stage " + (mainStage + 1).ToString();

                Instantiate(LaBack, transform.position, Quaternion.identity);
                stageBar.SetMaxTime(1);
                stageBarr.gameObject.SetActive(false);
                BossHpBar.gameObject.SetActive(true);
                BossBar.fillAmount = 1.0f;
                return;
            }
        }
        else
        {
            if(!za) { return; }
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
                    BossHpBar.gameObject.SetActive(true);
                    BossBar.fillAmount = 1.0f;
                }
                else
                {
                    PlayMusic(normalStageClip);
                    StartSpawningMonsters(subStage, stages[mainStage].subStages[subStage].stageDuration);
                    currentStageText.text = "Stage " + (mainStage + 1).ToString() + " - " + (subStage + 1).ToString();

                    if (subStage == 0)
                    {
                        stageBarr.gameObject.SetActive(true);
                        BossHpBar.gameObject.SetActive(false);

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
            BossHpBar.gameObject.SetActive(false);

            if (mainStage < 4)
            {
                if (!isBossDead)
                {
                    if (mainStage < 4)
                    {
                        Instantiate(HellKnife, Vector3.zero, Quaternion.identity);
                        skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                    }
                }

                isBossDead = false;
                return true;
            }
            else
            {
                if (!isBossDead) 
                {
                    Instantiate(HellKnife, Vector3.zero, Quaternion.identity);
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                    isBossDead = true;
                }
                return true;
            }
        }

        return false;
    }

    public bool IsBossDeadd()
    {
        BOSS bossMonster = FindObjectOfType<BOSS>();

        if (bossMonster == null || bossMonster.IsDead)
        {
            stageBarr.gameObject.SetActive(true);
            BossHpBar.gameObject.SetActive(false);

            if (mainStage < 3)
            {
                if (!isBossDead)
                {
                    if (mainStage < 3)
                    {
                        Instantiate(HellKnife, Vector3.zero, Quaternion.identity);
                        skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                    }
                }

                isBossDead = false;
                return true;
            }
            else
            {
                if (!isBossDead)
                {
                    Instantiate(HellKnife, Vector3.zero, Quaternion.identity);
                    skeletonAnimation.state.SetAnimation(0, "announce_stageClear", false);
                    isBossDead = true;
                }
                return true;
            }
        }

        return false;
    }

    private bool IsGameCleared()
    {
        switch (DifficultyManager.Instance.Difficulty)
        {
            case (int)DifficultyButton.DifficultyLevel.Easy:
                if (mainStage == 3)
                {
                    if (IsBossDeadd())
                    {
                        return true;
                    }
                }
                return false;
            case (int)DifficultyButton.DifficultyLevel.Normal:
            case (int)DifficultyButton.DifficultyLevel.Hard:
            case (int)DifficultyButton.DifficultyLevel.Extreme:
                if (mainStage == 4 && laplaceSpawned)
                {
                    if (IsBossDead())
                    {
                        return true;
                    }
                }

                return false;
            default:
                return false;
        }
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

    public void UZ()
    {
        Joku.Instance.Qok(Nand - 1);

        bool allTowersAreProjectiles = true;
        bool allTowersAreAttacker = true;
        bool allTowersAreSupporter = true;

        bool FANTASY = true;
        bool holoForce = true;
        bool IceWater = true;
        List<string> requiredTUnits = new List<string> { "노엘", "마린", "페코라", "후레아" };
        List<string> requiredFUnits = new List<string> { "와타메", "토와", "루나", "카나타" };
        List<string> requiredIWUnits = new List<string> { "페코라", "구라", "코보", "라미" };

        foreach (var tower in GameManager.Instance.AllTowers)
        {
            if (!tower.isEx)
            {
                if (tower.dicton.rolePlay != "투사체")
                {
                    allTowersAreProjectiles = false;
                    break;
                }
            }
        }

        foreach (var tower in GameManager.Instance.AllTowers)
        {
            if (!tower.isEx)
            {
                if (tower.dicton.rolePlay != "어택커")
                {
                    allTowersAreAttacker = false;
                    break;
                }
            }
        }

        foreach (var tower in GameManager.Instance.AllTowers)
        {
            if (!tower.isEx)
            {
                if (tower.dicton.rolePlay != "서포터")
                {
                    allTowersAreSupporter = false;
                    break;
                }
            }
        }

        foreach (var unitId in requiredTUnits)
        {
            if (!GameManager.Instance.selectedUnitIds.Contains(unitId))
            {
                FANTASY = false;
                break;
            }
        }

        foreach (var unitId in requiredFUnits)
        {
            if (!GameManager.Instance.selectedUnitIds.Contains(unitId))
            {
                holoForce = false;
                break;
            }
        }

        foreach (var unitId in requiredIWUnits)
        {
            if (!GameManager.Instance.selectedUnitIds.Contains(unitId))
            {
                IceWater = false;
                break;
            }
        }

        int unitCount = UnitSelectionScreen.Instance.selectedUnitCount;
        if (unitCount == 1 && Eaj)
        {
            Joku.Instance.Qok(37);
        }

        if (allTowersAreProjectiles)
        {
            Joku.Instance.Qok(31);
        }
        
        if (allTowersAreAttacker)
        {
            Joku.Instance.Qok(32);
        }
         
        if (allTowersAreSupporter)
        {
            Joku.Instance.Qok(33);
        }
        
        if (FANTASY)
        {
            Joku.Instance.Qok(34);
        }
        
        if (holoForce)
        {
            Joku.Instance.Qok(35);
        }
        
        if(IceWater && Eaj)
        {
            Joku.Instance.Qok(38);
        }
    }

    public void GameOver()
    {
        int dex;
        int maindex;

        gameOverImage.SetActive(true);
        Time.timeScale = 0f;
        dex = subStage;
        maindex = mainStage + 1;
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

    public void LATest()
    {
        isLATestActive = true;
    }
}
