using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Spine.Unity;
using Spine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public int sortingOrder;
}

[System.Serializable]
public struct StageData
{
    public GameObject[] monsterPrefabs;
    public int[] monsterCounts;
    public float stageDuration;
}

[System.Serializable]
public class Stage
{
    public StageData[] subStages;
}

public class GameManager : Singleton<GameManager>
{
    public static GameManager instance;
    public List<string> selectedUnitIds;
    public Tile[,] tiles;
    public GameObject koronePrefab, noelPrefab, marinePrefab, pekoraPrefab,
                      guraPrefab, watamePrefab, sheepPrefab, soraPrefab, oliPrefab,
                      subaruPrefab, kaliPrefab, flarePrefab, watsonPrefab, skullPrefab,
                      kanataPrefab, lunaPrefab, fubukiPrefab, towaPrefab, mumeiPrefab,
                      ramiPrefab, suiseiPrefab, faunaPrefab, koboPrefab, mikoPrefab;
    private Dictionary<string, GameObject> unitPrefabs;
    public Btn ClickedBtn { get; set; }
    public int mainHealth;
    private int maxMainHealth = 200;
    public BOSS bossMonster;
    public int mainStage = 1;
    public int subStage = 0;
    public Image Consol;
    public Image Setting;
    public Image mainHealthBar;
    private Coroutine fadeOutCoroutine;
    private bool hasPlayedSpecialAnimation = false;

    public GameObject stageBarr;
    public GameObject BossHpBar;
    public BossBBAARR[] bosbar;

    [System.Serializable]
    public class BossBBAARR
    {
        public GameObject BossHpBar;
        public TextMeshProUGUI CurHealthText;
        public TextMeshProUGUI MHealthText;
        public Image BossBarr;
    }

    public Image BossBar;
    public TextMeshProUGUI CurrentHealthText;
    public TextMeshProUGUI MaxHealthText;
    public CanvasGroup mainHealthGroup;
    public Dicton Watame;
    public GameObject HDPrefab;
    public GameObject Prefab30;
    public SkeletonGraphic Back;
    public int currentStageMonsterCount;

    [SerializeField]
    private TMP_Text mainHealthText;

    [SerializeField]
    private GameObject FirstPanel;

    public List<Tower> AllTowers = new List<Tower>();
    public SpawnM spawnM;
    public InfiniteStage infiniteStage;
    public Sandbag sandbag;
    public int DCo;
    public bool isDCo;
    public bool isokCamera = true;
    public bool isinfinite;
    public bool isbag = false;
    public UpdatePanel updatePanel;
    public GameObject towerKage;
    public bool tttsss = false;
    public string animationName;
    public GameObject spineAnimationPrefab;
    public GameObject currentSpineAnimation;
    private bool DoM = false;
    public GameObject Criti;
    public Holomoney holomoney;
    public GameObject mprefab;
    public GameObject oprefab;

    public double[] atiper;
    public Artifact[] artifact;
    public double[] EXatiper;
    public Artifact[] EXartifact;
    public TextMeshProUGUI speedT;
    public float SPPD;
    private int boxStack;
    public TextMeshProUGUI dropboxt;
    public List<(Artifact artifact, int quantity)> droppedArtifacts = new List<(Artifact, int)>();
    public Image[] DropAtiI;
    public TextMeshProUGUI[] AtiQ;
    public int plusatim;
    private bool ten = false;
    private bool tenten = false;
    private bool tententen = false;
    private bool tentententen = false;
    public bool isBreakSta = false;
    public Kali kariii;
    public GameObject upEffect;
    public GameObject bosssoulballPrefab;
    public int GiveHM = 0;
    public Tower PTower;
    public Sora sora;
    public Miko miko;
    public GameObject zenlossPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        selectedUnitIds = new List<string>();
        unitPrefabs = new Dictionary<string, GameObject>
        {
            {"코로네", koronePrefab},
            {"노엘", noelPrefab},
            {"마린", marinePrefab},
            {"페코라", pekoraPrefab},
            {"구라", guraPrefab},
            {"와타메", watamePrefab},
            {"양", sheepPrefab},
            {"소라", soraPrefab},
            {"왓슨", watsonPrefab},
            {"올리", oliPrefab},
            {"해골", skullPrefab},
            {"스바루", subaruPrefab},
            {"칼리", kaliPrefab},
            {"후레아", flarePrefab},
            {"카나타", kanataPrefab},
            {"루나", lunaPrefab},
            {"후부키", fubukiPrefab},
            {"토와", towaPrefab},
            {"무메이", mumeiPrefab},
            {"라미", ramiPrefab},
            {"스이세이", suiseiPrefab},
            {"파우나", faunaPrefab},
            {"코보", koboPrefab},
            {"미코", mikoPrefab}
        };

        selectedUnitIds = UnitSelectionScreen.Instance.selectedUnitIds;

        List<string> requiredFUnits = new List<string> { "코보", "파우나" };
        bool namumuru = true;
        foreach (var unitId in requiredFUnits)
        {
            if (!selectedUnitIds.Contains(unitId))
            {
                namumuru = false;
                break;
            }
        }

        if (namumuru)
        {
            Joku.Instance.Qok(36);
        }

        UpdateMainHealthText();
        StartCoroutine(ShowStartPanelAndBegin());
        StartCoroutine(LevelManager.Instance.CreateLevel());
        StartCoroutine(WaitForLevelCreationAndSetupTiles());
        DCo = 0;
        boxStack = 0;
        GiveHM = 0;
        droppedArtifacts.Clear();
        plusatim = 0;
        SPPD = 1;
        ten = false;
        tenten = false;
        tententen = false;
        tentententen = false;

        foreach (var tower in AllTowers)
        {
            Dicton dicton = tower.dicton;
            dicton.LoadAll();
        }
        isDCo = false;
        isinfinite = false;

        if (DifficultyManager.Instance.DifficultyMode == DifficultyButton.DifficultyMode.Infinite)
        {
            isinfinite = true;
        }
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        ATi();
    }


    public GameObject GetTowerPrefabById(string unitId)
    {
        return unitPrefabs[unitId];
    }

    public void noC()
    {
        updatePanel.StopDrag();
    }

    public void ArtiSpot()
    {
        if(!DoM)
        {
            DoM = true;
            updatePanel.ArtiSpot();
        }
    }

    void Update()
    {
        if ((Setting.gameObject.activeSelf || Time.timeScale == 1 || Time.timeScale == 1.5f || Time.timeScale == 2) && Input.GetKeyDown(KeyCode.Escape) && !isokCamera)
        {
            ActiveSet();
        }

        if ((Consol.gameObject.activeSelf || Time.timeScale == 1 || Time.timeScale == 1.5f || Time.timeScale == 2) && Input.GetKeyDown(KeyCode.BackQuote))
        {
            bool isActive = Consol.gameObject.activeSelf;

            Consol.gameObject.SetActive(!isActive);
        }

        if (tttsss)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            int layerMask = 1 << LayerMask.NameToLayer("Tile");
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, layerMask);

            if (towerKage == null)
            {
                towerKage = Instantiate(towerKage);
                towerKage.SetActive(false);
            }

            if (hitCollider != null && hitCollider.CompareTag("Tile"))
            {
                Tile hitTile = hitCollider.GetComponent<Tile>();
                Vector3 targetPosition = hitTile.transform.position;
                targetPosition.y += 0.2f; 
                towerKage.transform.position = targetPosition;
                var renderer = towerKage.GetComponent<Renderer>();

                towerKage.SetActive(true);
                CreateSpineAnimation(hitTile.transform.position);
            }
            else
            {
                Kageoff();
            }
            UpdateTowerKageSortingOrder();
        }

        if(!ten && mainStage == 10 && subStage == 1)
        {
            Joku.Instance.Qok(24);
            ten = true;
        }

        if (!tenten && mainStage == 10 && subStage == 11)
        {
            Joku.Instance.Qok(25);
            tenten = true;
        }

        if (!tententen && mainStage == 10 && subStage == 101)
        {
            Joku.Instance.Qok(26);
            tententen = true;
        }

        if (!tentententen && mainStage == 10 && subStage == 1001)
        {
            Joku.Instance.Qok(27);
            tentententen = true;
        }
    }

    public void ActiveSet()
    {
        bool isActive = Setting.gameObject.activeSelf;
        Setting.gameObject.SetActive(!isActive);
        Time.timeScale = isActive ? SPPD : 0;
        if (Time.timeScale == 0)
        {
            PlaySpineUIAnimationOnce("wide");
        }
    }

    public void Kageoff()
    {
        if (towerKage == null)
        {
            return;
        }

        towerKage.SetActive(false);
    }

    public void CreateSpineAnimation(Vector3 position)
    {
        Vector3 newPosition;
        
        if (string.IsNullOrEmpty(animationName))
        {
            return;
        }

        if (spineAnimationPrefab != null)
        {
            if (animationName == "rami")
            {
                newPosition = new Vector3(position.x, position.y, position.z);
            }
            else
            {
                newPosition = new Vector3(position.x + 0.825f, position.y, position.z);
            }

            if (currentSpineAnimation != null)
            {
                currentSpineAnimation.transform.position = newPosition; 
            }
            else
            {
                currentSpineAnimation = Instantiate(spineAnimationPrefab, newPosition, Quaternion.identity);

                SkeletonAnimation skeletonAnimation = currentSpineAnimation.GetComponent<SkeletonAnimation>();

                if (skeletonAnimation != null)
                {
                    skeletonAnimation.state.SetAnimation(0, animationName, true);
                }
            }
        }
    }

    private void UpdateTowerKageSortingOrder()
    {
        if (towerKage == null) return; 

        float localY = towerKage.transform.position.y;

        if (Mathf.Approximately(localY, 0.8600003f))
        {
            towerKage.GetComponent<Renderer>().sortingOrder = -3;
        }
        else if (Mathf.Approximately(localY, -1.16f))
        {
            towerKage.GetComponent<Renderer>().sortingOrder = -1;
        }
        else if (Mathf.Approximately(localY, -3.18f))
        {
            towerKage.GetComponent<Renderer>().sortingOrder = 1;
        }
        else if (Mathf.Approximately(localY, -5.2f))
        {
            towerKage.GetComponent<Renderer>().sortingOrder = 3;
        }
    }

    public void ATi()
    {
        int unitCount = UnitSelectionScreen.Instance.selectedUnitCount;
        switch (unitCount)
        {
            case 4:
                break;
            case 3:
                for (int i = 0; i < AllTowers.Count; i++)
                {
                    if (AllTowers[i].dicton != null)
                    {
                        if (AllTowers[i].dicton.myartifact != null)
                        {
                            if (AllTowers[i].dicton.myartifact.artifactId == "ae11")
                            {
                                BuffManager.Instance.ApplyBuff(AllTowers[i].gameObject, "기능카드13", 9999f, 0.1f, 0, 0);
                            }
                        }
                    }
                }
                break;
            case 2:
                for (int i = 0; i < AllTowers.Count; i++)
                {
                    if (AllTowers[i].dicton != null)
                    {
                        if (AllTowers[i].dicton.myartifact != null)
                        {
                            if (AllTowers[i].dicton.myartifact.artifactId == "ae11")
                            {
                                BuffManager.Instance.ApplyBuff(AllTowers[i].gameObject, "기능카드13", 9999f, 0.2f, 0, 0);
                            }
                        }
                    }
                }
                break;
            case 1:
                for (int i = 0; i < AllTowers.Count; i++)
                {
                    if (AllTowers[i].dicton != null)
                    {
                        if (AllTowers[i].dicton.myartifact != null)
                        {
                            if (AllTowers[i].dicton.myartifact.artifactId == "ae11")
                            {
                                BuffManager.Instance.ApplyBuff(AllTowers[i].gameObject, "기능카드13", 9999f, 0.5f, 0, 0.5f);
                                Instantiate(oprefab, Vector3.zero, Quaternion.identity, AllTowers[0].transform);
                                Instantiate(mprefab, Vector3.zero, Quaternion.identity, AllTowers[0].transform);
                            }
                        }
                    }
                }
                break;
        }
    }

    public void DestroyCurrentSpineAnimation()
    {
        if (currentSpineAnimation != null)
        {
            Destroy(currentSpineAnimation);
            currentSpineAnimation = null;
        }
    }

    IEnumerator WaitForLevelCreationAndSetupTiles()
    {
        yield return new WaitUntil(() => LevelManager.Instance.Tiles.Count > 0);

        int desiredX = 5;
        int index = 0;
        bool canSheep = false;

        foreach (var unitId in selectedUnitIds)
        {
            int desiredY = index % 4;
            if (index >= 4)
            {
                desiredX--;
                desiredY = 0;
            }

            Point point = new Point(desiredX, desiredY);
            if (LevelManager.Instance.Tiles.ContainsKey(point))
            {
                Tile tile = LevelManager.Instance.Tiles[point];
                try
                {
                    tile.PlaceTowerID(unitId);
                    if (unitId == "와타메")
                    {
                        int watameLevel = Watame.LevelSS;
                        if (watameLevel >= 3)
                        {
                            canSheep = true;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            index++;
        }

        if (canSheep)
        {
            int sheepY = index % 4;
            if (index >= 4)
            {
                desiredX--;
                sheepY = 0;
            }

            if (index >= 5)
            {
                desiredX++;
                sheepY = 1;
            }
            Point sheepPoint = new Point(desiredX, sheepY);
            if (LevelManager.Instance.Tiles.ContainsKey(sheepPoint))
            {
                Tile sheepTile = LevelManager.Instance.Tiles[sheepPoint];
                sheepTile.PlaceTowerID("양");
            }
        }
    }

    public void CheckAndSetKali(Enemy enemy)
    {
        foreach (var unitId in selectedUnitIds)
        {
            if (unitId != null && unitId == "칼리")
            {
                enemy.iskali = true;
            }
        }
    }

    IEnumerator FadeImage(Image image, float flashInterval)
    {
        float delta = 1 / flashInterval;

        while (image.gameObject.activeSelf)
        {
            for (float alpha = 1; alpha >= 0; alpha -= delta * Time.deltaTime)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }

            for (float alpha = 0; alpha <= 1; alpha += delta * Time.deltaTime)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    public void OnMonsterReached(int damage)
    {
        if (!isbag)
        {
            if (isinfinite)
            {
                infiniteStage.MonsterDeath();
            }
            else
            {
                spawnM.MonsterDeath();
            }
        }
 
        if (damage <= 0)
        {
            return;
        }

        mainHealth -= damage;

        if (mainHealth <= 0)
        {
            mainHealth = 0;
            
            if (!isbag)
            {
                if (isinfinite)
                {
                    infiniteStage.GameOver();
                }
                else
                {
                    spawnM.GameOver();
                }
            }
            else
            {
                sandbag.GameOver();
            }
        }
        else if ((float)mainHealth / maxMainHealth < 0.3f)
        {
            SpawnAndPlaySpecialAnimation();
        }

        UpdateMainHealthText();
        mainHealthGroup.alpha = 1;
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        fadeOutCoroutine = StartCoroutine(FadeOutHealthUI());
        SpawnAndPlaySpineAnimation();
    }

    private void SpawnAndPlaySpineAnimation()
    {
        if (HDPrefab != null)
        {
            GameObject animationInstance = Instantiate(HDPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            SkeletonAnimation skeletonAnimation = animationInstance.GetComponent<SkeletonAnimation>();

            if (skeletonAnimation != null)
            {
                skeletonAnimation.state.SetAnimation(0, "damaged", false);
                skeletonAnimation.state.Complete += (trackEntry) =>
                {
                    Destroy(animationInstance);
                };
            }
        }
    }

    private void SpawnAndPlaySpecialAnimation()
    {
        if (hasPlayedSpecialAnimation) return;
        if (Prefab30 != null)
        {
            GameObject animationInstance = Instantiate(Prefab30, new Vector3(0, 0, 0), Quaternion.identity);
            SkeletonAnimation skeletonAnimation = animationInstance.GetComponent<SkeletonAnimation>();

            if (skeletonAnimation != null)
            {
                skeletonAnimation.state.SetAnimation(0, "below30", true);
            }
        }
        hasPlayedSpecialAnimation = true;
    }

    private void UpdateMainHealthText()
    {
        mainHealthText.text = mainHealth.ToString();
        mainHealthBar.fillAmount = (float)mainHealth / maxMainHealth;
    }

    public void Levveeeel()
    {
        updatePanel.deleteLV();
    }

    IEnumerator FadeOutHealthUI()
    {
        yield return new WaitForSeconds(2f);

        float duration = 1.5f;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            mainHealthGroup.alpha = Mathf.Lerp(1, 0, currentTime / duration);
            yield return null;
        }

        mainHealthGroup.alpha = 0;
        fadeOutCoroutine = null;
    }

    public void AddTower(Tower tower)
    {
        AllTowers.Add(tower);
    }

    private IEnumerator ShowStartPanelAndBegin()
    {
        FirstPanel.SetActive(true);

        CanvasGroup canvasGroup = FirstPanel.GetComponent<CanvasGroup>();

        float fadeDuration = 2.5f;
        float waitDuration = 0.7f; 
        float timer = 0f;

        while (timer < waitDuration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        timer = 0f; 

        while (timer < fadeDuration - waitDuration)
        {
            yield return null;

            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / (fadeDuration - waitDuration));
            canvasGroup.alpha = alpha;
        }

        canvasGroup.alpha = 0f;
        FirstPanel.SetActive(false);
    }

    public void PlaySpineUIAnimationOnce(string animationName)
    {
        if (Back == null)
        {
            return;
        }
        Time.timeScale = 0;
        Back.AnimationState.ClearTracks();
        Back.AnimationState.SetAnimation(0, animationName, false);
    }

    public void UpdateBossHP(float hpRatio)
    {
        BossBar.fillAmount = hpRatio;
    }

    public void inpUpdateBossHP(float hpRatio, int index)
    {
        bosbar[index].BossBarr.fillAmount = hpRatio;
    }

    public void StopM()
    {
        if (!isbag)
        {
            if (isinfinite)
            {
                infiniteStage.Down();
            }
            else
            {
                spawnM.Down();
            }
        }
        else
        {
            sandbag.Down();
        }
    }

    public void PlayM()
    {
        if (!isbag)
        {
            if (isinfinite)
            {
                infiniteStage.Up();
            }
            else
            {
                spawnM.Up();
            }
        }
        else
        {
            sandbag.Up();
        }
    }

    public void SoraB(Tower tower)
    {
        if (sora != null)
        {
            if (sora.LevelS > 1)
            {
                sora.BB(tower);
            }
        }
    }

    public void UpdateHealthTexts(float currentHealth, float maxHealth)
    {
        CurrentHealthText.text = $"{Mathf.RoundToInt(currentHealth)}";
        MaxHealthText.text = $"{maxHealth}";
    }

    public void inbossUpTexts(float currentHealth, float maxHealth, int index)
    {
        bosbar[index].CurHealthText.text = $"{Mathf.RoundToInt(currentHealth)}";
        bosbar[index].MHealthText.text = $"{maxHealth}";
    }

    public void Nekomata()
    {
        plusatim += 1;
    }

    public void FastSet(float speed)
    {
        Time.timeScale = speed;
        SPPD = speed;
        if (speed == 0)
        {
            speedT.text = "PAUSE";
        }
        else
        {
            speedT.text = speed.ToString("F1") + " x";
        }
    }

    public void DropBox()
    {
        boxStack += 1;
    }

    public void TextBoxend()
    {
        dropboxt.text = boxStack.ToString();
    }

    public void AddDroppedArtifact(Artifact artifact, int quantity)
    {
        if (droppedArtifacts.Count >= 12)
        {
            return; 
        }


        var existingArtifact = droppedArtifacts.Find(item => item.artifact.artifactId == artifact.artifactId);
        if (existingArtifact.artifact != null)
        {
            int index = droppedArtifacts.IndexOf(existingArtifact);
            droppedArtifacts[index] = (existingArtifact.artifact, existingArtifact.quantity + quantity);
        }
        else
        {
            droppedArtifacts.Add((artifact, quantity));
        }
    }

    public void PrintDroppedArtifacts()
    {
        for (int i = 0; i < DropAtiI.Length; i++)
        {
            if (i < droppedArtifacts.Count)
            {
                DropAtiI[i].gameObject.SetActive(true);
                AtiQ[i].gameObject.SetActive(true);

                var item = droppedArtifacts[i];
                DropAtiI[i].sprite = item.artifact.sprit; 
                AtiQ[i].text = item.quantity.ToString(); 
            }
            else
            {
                DropAtiI[i].gameObject.SetActive(false);
                AtiQ[i].gameObject.SetActive(false);
            }
        }
    }

    public void samco()
    {
        sandbag.endTimer = 3f;
    }
}

