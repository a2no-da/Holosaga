using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joku : MonoBehaviour
{
    public static Joku Instance;
    public AchievementManager achievementManager;
    public DictonManager dictonManager;
    public ArtifactManager artifactManager;
    public GameObject UChang;
    public bool allDictonsNotPurchasable = true;
    public bool allMDictonsNotPurchasable = true;
    public bool allADictonsNotPurchasable = true;
    private Queue<int> achievementQueue = new Queue<int>();
    private bool isAnimating = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(2899450);
            //Steamworks.SteamClient.Init(480); // 테스트
        }
        catch (System.Exception e) 
        {
            Debug.Log(e);
        }

        allDictonsNotPurchasable = true;
        allMDictonsNotPurchasable = true;
        allADictonsNotPurchasable = true;

        for (int i = 0; i < achievementManager.achievements.Length; i++)
        {
            var ach = new Steamworks.Data.Achievement("ACH_" + (i + 1));
            achievementManager.achievements[i].unlocked = ach.State;
            achievementManager.achievements[i].updat = ach.State;
            achievementManager.SaveAchievements();
        }
    }

    void Update()
    {
        foreach (var achievement in achievementManager.achievements)
        {
            if (achievement.unlocked && !achievement.updat)
            {
                GameObject uChangInstance = Instantiate(UChang);

                UpSS upSS = uChangInstance.GetComponent<UpSS>();
                if (upSS != null)
                {
                    achievement.updat = true;
                    upSS.ChangeAnimationBasedOnLanguage(achievement.num.ToString());
                }
            }
        }

        UpdateDictons();
        UpdateMDictons();
        UpdateArtifacts();

        if (allDictonsNotPurchasable)
        {
            Qok(28);
        }

        if (allMDictonsNotPurchasable)
        {
            Qok(29);
        }

        if(allADictonsNotPurchasable)
        {
            Qok(30);
        }
    }

    public void Qok(int numb)
    {
        if (achievementManager.achievements[numb].unlocked) return;
        
        achievementManager.achievements[numb].unlocked = true;
        if (achievementManager.achievements[numb].unlocked && !achievementManager.achievements[numb].updat)
        {
            achievementManager.achievements[numb].updat = true;
            achievementQueue.Enqueue(numb);

            if (!isAnimating)
            {
                StartCoroutine(PlayAnimations());
            }

            var ach = new Steamworks.Data.Achievement("ACH_" + (numb + 1));
            ach.Trigger();
            Debug.Log($"[스팀 업적] 업적 : {numb + 1}번째");
            Debug.Log(ach.State);

            achievementManager.SaveAchievements();
        }
    }

    private IEnumerator PlayAnimations()
    {
        isAnimating = true;

        while (achievementQueue.Count > 0)
        {
            int numb = achievementQueue.Dequeue();
            GameObject uChangInstance = Instantiate(UChang);
            UpSS upSS = uChangInstance.GetComponent<UpSS>();

            if (upSS != null)
            {
                upSS.ChangeAnimationBasedOnLanguage(achievementManager.achievements[numb].num.ToString());

                yield return new WaitForSecondsRealtime(3f); 

                Destroy(uChangInstance);
            }
        }

        isAnimating = false;

        for (int i = 0; i < achievementManager.achievements.Length; i++)
        {
            if (!achievementManager.achievements[i].unlocked && !achievementManager.achievements[i].updat)
            {
                var ach = new Steamworks.Data.Achievement("ACH_" + (i + 1));
                if (ach.State)
                {
                    Qok(i);
                }
            }
        }
    }

    void UpdateDictons()
    {
        allDictonsNotPurchasable = true; 

        foreach (var dicton in dictonManager.dictons)
        {
            if (dicton.isPurchasable)
            {
                allDictonsNotPurchasable = false;
            }
            else
            {
                if (int.TryParse(dicton.id, out int mmm))
                {
                    if (!achievementManager.achievements[mmm - 1].unlocked)
                    {
                        Qok(mmm - 1);
                    }
                }
            }
        }
    }

    void UpdateMDictons()
    {
        allMDictonsNotPurchasable = true; 

        foreach (var mdicton in dictonManager.Mdictons)
        {
            if (mdicton.isPurchasable)
            {
                allMDictonsNotPurchasable = false;
                break; 
            }
        }
    }

    void UpdateArtifacts()
    {
        allADictonsNotPurchasable = true;

        foreach (var artifact in artifactManager.artifacts)
        {
            if (artifact.Quantity < 1)
            {
                allADictonsNotPurchasable = false;
                break; 
            }
        }
    }
}
