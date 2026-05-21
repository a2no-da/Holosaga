using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine;
using Spine.Unity;
using UnityEngine.Audio;

[System.Serializable]
public class CharacterPanel
{
    public GameObject panelObject;
    public Image towerImage;
    public Image towerImage2;
    public Image towerImage3;
    public TMP_Text healthText;
    public TMP_Text maxHealthText;
    public TMP_Text PowerText;
    public TMP_Text levelText;
    public Image HealthBar;
    public Image cooldownImage;
    public TMP_Text cooldownText;
    public Button activeButton;
    public GameObject DeathTx;
    public TMP_Text DeathCount;
    public Image[] starImages;
    public Image ExperienceBar;
    public Image levelUpImage;
    public TMP_Text Expper;
    public FadeOutImage whiteImage;
    public Button levelUpButton;
    public SkeletonGraphic levelUpAnimation;
    public Image ati;
    public float Wind;
    public GameObject swBt;
    public GameObject[] OverI;
    public TMP_Text[] Over1;

    public void StartFadeOut()
    {
        whiteImage.StartFadeOut();
    }
}

[System.Serializable]
public class Num
{
    public Image nua;
    public Image Number;
}

public class UpdatePanel : MonoBehaviour
{
    public List<Tower> AllTowers = new List<Tower>();
    public List<CharacterPanel> characterPanels = new List<CharacterPanel>();
    public Num[] numB;
    public GameObject LeUpPrefab;
    public GameObject SWP;
    private bool oneD;
    private bool carD;
    public int pl = 0;
    public int hpp = 0;
    public GameObject oneP;

    void Start()
    {
        GameManager.Instance.AllTowers = AllTowers;
        int unitCount = UnitSelectionScreen.Instance.selectedUnitCount;
        switch (unitCount)
        {
            case 4:
                break;
            case 3:
                characterPanels[3].panelObject.SetActive(false);
                characterPanels[3].towerImage2.gameObject.SetActive(false);
                characterPanels[3].towerImage3.gameObject.SetActive(false);
                numB[3].Number.gameObject.SetActive(false);
                characterPanels[3].swBt.SetActive(false);
                SWP.GetComponent<RectTransform>().anchoredPosition = new Vector2(-691.1f, SWP.GetComponent<RectTransform>().anchoredPosition.y);
                break;
            case 2:
                characterPanels[2].panelObject.SetActive(false);
                characterPanels[3].panelObject.SetActive(false);
                characterPanels[2].towerImage2.gameObject.SetActive(false);
                characterPanels[3].towerImage2.gameObject.SetActive(false);
                characterPanels[2].towerImage3.gameObject.SetActive(false);
                characterPanels[3].towerImage3.gameObject.SetActive(false);
                numB[2].Number.gameObject.SetActive(false);
                numB[3].Number.gameObject.SetActive(false);
                numB[2].nua.gameObject.SetActive(false);
                numB[3].nua.gameObject.SetActive(false);
                characterPanels[3].swBt.SetActive(false);
                characterPanels[2].swBt.SetActive(false);
                SWP.GetComponent<RectTransform>().anchoredPosition = new Vector2(-947.5f, SWP.GetComponent<RectTransform>().anchoredPosition.y);
                break;
            case 1:
                characterPanels[1].panelObject.SetActive(false);
                characterPanels[2].panelObject.SetActive(false);
                characterPanels[3].panelObject.SetActive(false);
                characterPanels[1].towerImage2.gameObject.SetActive(false);
                characterPanels[2].towerImage2.gameObject.SetActive(false);
                characterPanels[3].towerImage2.gameObject.SetActive(false);
                characterPanels[1].towerImage3.gameObject.SetActive(false);
                characterPanels[2].towerImage3.gameObject.SetActive(false);
                characterPanels[3].towerImage3.gameObject.SetActive(false);
                numB[1].Number.gameObject.SetActive(false);
                numB[2].Number.gameObject.SetActive(false);
                numB[3].Number.gameObject.SetActive(false);
                numB[1].nua.gameObject.SetActive(false);
                numB[2].nua.gameObject.SetActive(false);
                numB[3].nua.gameObject.SetActive(false);
                characterPanels[3].swBt.SetActive(false);
                characterPanels[2].swBt.SetActive(false);
                characterPanels[1].swBt.SetActive(false);
                SWP.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1204.4f, SWP.GetComponent<RectTransform>().anchoredPosition.y);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePanels();
        UpdatTxOver();

        for (int i = 0; i < characterPanels.Count; i++)
        {
            KeyCode[] activeKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
            if (i < activeKeys.Length && Input.GetKeyDown(activeKeys[i]) && characterPanels[i].activeButton.gameObject.activeSelf)
            {
                characterPanels[i].activeButton.onClick.Invoke();
            }
        }

        for (int i = 0; i < AllTowers.Count; i++)
        {
            KeyCode[] activeKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };
            if (i < activeKeys.Length && Input.GetKeyDown(activeKeys[i]))
            {
                GameManager.Instance.Kageoff();
                AllTowers[i].OnTowerDragEnd();
            }
        }

        for (int i = 0; i < characterPanels.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()) && characterPanels[i].levelUpButton.gameObject.activeSelf)
            {
                characterPanels[i].levelUpButton.onClick.Invoke();
            }
        }
    }

    public void UpdatePanels()
    {
        for (int i = 0; i < AllTowers.Count; i++)
        {
            if (AllTowers[i].towerP != null)
            {
                int index = i;
                int towerLevel = AllTowers[i].Level;
                int towerLev = AllTowers[i].LevelS;
                int requiredExperience = AllTowers[index].maxExperience;
                int currentExperience = Tower.experience;
                float experiencePercentage = ((float)currentExperience / requiredExperience) * 100;

                characterPanels[i].Expper.text = Mathf.RoundToInt(experiencePercentage).ToString() + "%";
                Dicton dicton = AllTowers[i].dicton;
                IEnumerator currentDeactivateCoroutine = null;

                if (AllTowers[i].dicton.myartifact != null)
                {
                    if (characterPanels[i].ati.GetComponent<Image>().sprite != AllTowers[i].dicton.myartifact.sprit)
                    {
                        characterPanels[i].ati.GetComponent<Image>().sprite = AllTowers[i].dicton.myartifact.sprit;
                    }
                }
                else
                {
                    if (characterPanels[i].ati.gameObject.activeSelf)
                    {
                        characterPanels[i].ati.gameObject.SetActive(false);
                    }
                }

                if (!AllTowers[i].myActive)
                {
                    numB[index].Number.gameObject.SetActive(false);
                    numB[index].nua.gameObject.SetActive(false);
                }
                else
                {
                    numB[index].Number.gameObject.SetActive(true);
                    numB[index].nua.gameObject.SetActive(true);
                }

                if (AllTowers[i].myActive)
                {
                    if (AllTowers[i].ActiveCooldown > 0)
                    {
                        if (currentDeactivateCoroutine != null)
                        {
                            StopCoroutine(currentDeactivateCoroutine);
                            currentDeactivateCoroutine = null;
                        }

                        characterPanels[i].cooldownImage.gameObject.SetActive(true);
                        characterPanels[i].cooldownImage.color = new Color(0f, 0f, 0f, 1f);
                        float cooldownRatio = AllTowers[i].ActiveCooldown / AllTowers[i].Active_cooltime;
                        StartCoroutine(FadeOutCooldownImage(characterPanels[i].cooldownImage, AllTowers[i].ActiveCooldown));

                        characterPanels[i].cooldownText.text = Mathf.FloorToInt(AllTowers[i].ActiveCooldown).ToString();
                        characterPanels[i].cooldownText.gameObject.SetActive(true);

                        if (AllTowers[i].ActiveCooldown < 1)
                        {
                            characterPanels[i].cooldownText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        characterPanels[i].cooldownText.gameObject.SetActive(false);
                        characterPanels[i].cooldownImage.color = new Color(1f, 1f, 1f, 1f);
                        currentDeactivateCoroutine = DeactivateAfterDelay(characterPanels[i].cooldownImage, 0.5f, index);
                        StartCoroutine(currentDeactivateCoroutine);
                    }
                }
                else
                {
                    characterPanels[i].cooldownText.gameObject.SetActive(false);
                    characterPanels[i].cooldownImage.gameObject.SetActive(false);
                }

                float progress = (float)dicton.MYexp / dicton.exp;

                characterPanels[i].activeButton.onClick.RemoveAllListeners();
                characterPanels[i].activeButton.onClick.AddListener(() => ActivateTower(index));
                characterPanels[i].towerImage.sprite = AllTowers[i].towerP;
                characterPanels[i].towerImage2.sprite = AllTowers[i].towerP2;
                characterPanels[i].towerImage3.sprite = AllTowers[i].towerP3;
                float hp = AllTowers[i].Health;
                float Mhp = AllTowers[i].MaxHealth;
                characterPanels[i].healthText.text = $"{Mathf.RoundToInt(hp)}";
                characterPanels[i].maxHealthText.text = $"{Mathf.RoundToInt(Mhp)}";
                characterPanels[i].PowerText.text = ((int)AllTowers[i].Power).ToString();
                characterPanels[i].levelText.text = "LV " + AllTowers[i].Level.ToString();
                characterPanels[i].HealthBar.fillAmount = (float)AllTowers[i].Health / (float)AllTowers[i].MaxHealth;
                characterPanels[i].ExperienceBar.fillAmount = (float)currentExperience / requiredExperience;
                characterPanels[i].ExperienceBar.gameObject.SetActive(!(currentExperience >= requiredExperience && AllTowers[index].Level <= 30));
                characterPanels[i].starImages[0].gameObject.SetActive(towerLev >= 1);
                characterPanels[i].starImages[1].gameObject.SetActive(towerLev > 1);
                characterPanels[i].starImages[2].gameObject.SetActive(towerLev > 2);
                characterPanels[i].swBt.GetComponent<Image>().sprite = AllTowers[i].towerSWim;

                if (AllTowers[index].Level < 30)
                {
                    if (currentExperience >= requiredExperience && AllTowers[index].Level < 30)
                    {
                        characterPanels[i].levelUpImage.gameObject.SetActive(true);
                        if(oneP != null)
                        {
                            oneP.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        characterPanels[i].levelUpImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    characterPanels[i].Expper.gameObject.SetActive(false);
                    characterPanels[i].levelUpImage.gameObject.SetActive(true);
                    characterPanels[index].levelUpAnimation.Skeleton.SetSkin("normal");
                    characterPanels[index].levelUpAnimation.Skeleton.SetSlotsToSetupPose();
                }

                if (AllTowers[i].isDead)
                {
                    characterPanels[i].DeathTx.gameObject.SetActive(true);
                    characterPanels[i].levelText.gameObject.SetActive(false);
                    foreach (Image starImage in characterPanels[i].starImages)
                    {
                        starImage.gameObject.SetActive(false);
                    }
                    characterPanels[i].towerImage.color = new Color(0.4f, 0.4f, 0.4f, 1);
                    characterPanels[i].levelText.text = AllTowers[i].Level.ToString() + " LV";
                    characterPanels[i].DeathCount.text = AllTowers[i].respawnTime.ToString();
                    AllTowers[i].respawnTimeText.text = AllTowers[i].respawnTime.ToString();
                }
                else
                {
                    characterPanels[i].DeathTx.gameObject.SetActive(false);
                    foreach (Image starImage in characterPanels[i].starImages)
                    {
                        characterPanels[i].starImages[0].gameObject.SetActive(towerLev >= 1);
                        characterPanels[i].starImages[1].gameObject.SetActive(towerLev > 1);
                        characterPanels[i].starImages[2].gameObject.SetActive(towerLev > 2);
                    }
                    characterPanels[i].levelText.gameObject.SetActive(true);

                    characterPanels[i].towerImage.color = Color.white;
                }

                if (Tower.experience >= requiredExperience && AllTowers[index].Level < 30)
                {
                    characterPanels[index].levelUpButton.gameObject.SetActive(true);
                    AllTowers[index].LevUpok.SetActive(true);
                    characterPanels[index].levelUpAnimation.Skeleton.SetSkin("canUp");
                    characterPanels[index].levelUpAnimation.Skeleton.SetSlotsToSetupPose();
                    characterPanels[index].levelUpButton.onClick.RemoveAllListeners();

                    characterPanels[index].levelUpButton.onClick.AddListener(() =>
                    {
                        if (AllTowers[index].Level < 30)
                        {
                            AllTowers[index].LevelUp();
                            characterPanels[index].levelUpButton.gameObject.SetActive(false);
                            LU(index);
                        }
                    });
                }
                else
                {
                    AllTowers[index].LevUpok.SetActive(false);
                }
            }
            if (AllTowers[i].isDead && AllTowers[i].towerP == null)
            {
                AllTowers[i].respawnTimeText.text = AllTowers[i].respawnTime.ToString();
            }
        }
    }

    public void deleteLV()
    {
        if(oneP != null)
        {
            Destroy(oneP); 
            oneP = null;
        }
    }

    public void StopDrag()
    {
        GameManager.Instance.tttsss = false;
        for (int i = 0; i < AllTowers.Count; i++)
        {
            AllTowers[i].isDragging = false;
        }
    }

    public void UpdatTxOver()
    {
        for (int i = 0; i < AllTowers.Count; i++)
        {
            if (AllTowers[i].towerP != null)
            {
                if (AllTowers[i].dicton.PowerUp > 0)
                {
                    characterPanels[i].Over1[0].text = AllTowers[i].dicton.PowerUp.ToString();
                }
                else
                {
                    characterPanels[i].OverI[0].SetActive(false);
                }

                if (AllTowers[i].dicton.PowerInUp > 0)
                {
                    characterPanels[i].Over1[1].text = AllTowers[i].dicton.PowerInUp.ToString();
                }
                else
                {
                    characterPanels[i].OverI[1].SetActive(false);
                }

                if (AllTowers[i].dicton.HpUp > 0)
                {
                    characterPanels[i].Over1[2].text = AllTowers[i].dicton.HpUp.ToString();
                }
                else
                {
                    characterPanels[i].OverI[2].SetActive(false);
                }

                if (AllTowers[i].dicton.HpInUp > 0)
                {
                    characterPanels[i].Over1[3].text = AllTowers[i].dicton.HpInUp.ToString();
                }
                else
                {
                    characterPanels[i].OverI[3].SetActive(false);
                }
            }
        }
    }

    public void resetSand()
    {
        for (int i = 0; i < AllTowers.Count; i++)
        {
            AllTowers[i].ActiveCooldown = 0;
        }
    }

    public void ArtiSpot()
    {
        for (int i = 0; i < AllTowers.Count; i++)
        {
            if (!AllTowers[i].isEx)
            {
                if (AllTowers[i].dicton.myartifact != null)
                {
                    if (AllTowers[i].dicton.myartifact.artifactId == "ae6")
                    {
                        pl += 8;
                        hpp += 8;
                    }
                }
            }
        }

        for (int i = 0; i < AllTowers.Count; i++)
        {
            if (!AllTowers[i].isEx)
            {
                if (AllTowers[i].dicton.myartifact != null)
                {
                    if (AllTowers[i].dicton.myartifact.artifactId == "ae6")
                    {
                        AllTowers[i].dicton.myartifact.hpIncrease = 1;
                        AllTowers[i].dicton.myartifact.SpeedIncrease = 1;
                        AllTowers[i].dicton.myartifact.PowerIncrease = 1;

                        if ((hpp - 8) > 0)
                        {
                            AllTowers[i].dicton.myartifact.hpIncrease += (hpp - 8);
                        }
                        AllTowers[i].dicton.myartifact.SpeedIncrease += pl;
                        AllTowers[i].dicton.myartifact.PowerIncrease += pl;
                    }
                }
            }
        }
    }

    public void LEUp(int index)
    {
        int requiredExperience = AllTowers[index].maxExperience;
        if (Tower.experience >= requiredExperience && AllTowers[index].Level < 30)
        {
            AllTowers[index].LevelUp(); 
            LU(index); 
        }
    }

    public void SW(int index)
    {
        AllTowers[index].OnTowerDragEnd();
    }

    public void ActCardWind(Tower tower, int im)
    {
        bool isActiveCardFound = false;

        for (int j = 0; j < 2; j++)
        {
            if (AllTowers[0].abilityCards[j] != null &&
                AllTowers[0].abilityCards[j].abilitytype == AbilityType.PlusActive)
            {
                isActiveCardFound = true;
                break; 
            }
        }

        if (isActiveCardFound)
        {
            for (int i = 0; i < AllTowers.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (AllTowers[i].abilityCards[j] != null &&
                        AllTowers[i].abilityCards[j].abilitytype == AbilityType.PlusActive)
                    {
                        AllTowers[i].abilityCards[j].abilityFunction.FunctionPulsACard(AllTowers[i]);
                    }
                }
            }
        }

        if(isActiveCardFound)
        {
            characterPanels[im].Wind += 0.01f;
            tower.inPlusStat.cardcool = characterPanels[im].Wind;
            tower.mydasdasd();
        }
    }

    public void LU(int index)
    {
        GameObject leUpInstance = Instantiate(LeUpPrefab, AllTowers[index].transform.position, Quaternion.identity);
        leUpInstance.transform.SetParent(AllTowers[index].transform, false);
        leUpInstance.transform.localPosition = Vector3.zero;

        characterPanels[index].whiteImage.gameObject.SetActive(true);
        characterPanels[index].StartFadeOut();
    }

    void ActivateTower(int index)
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (index >= 0 && index < AllTowers.Count)
        {
            var tower = AllTowers[index];
            if (tower != null)
            {
                if (tower.ActiveCooldown > 0 || tower.isStunned || tower.isDead)
                {
                    return;
                }
                else
                {
                    tower.Active();
                    if(tower.myActive)
                    {
                        ActCardWind(tower, index);
                    }
                }
            }
        }
    }

    IEnumerator FadeOutCooldownImage(Image cooldownImage, float duration)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, currentTime / duration);
            cooldownImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        cooldownImage.color = new Color(1f, 1f, 1f, 0);
    }

    public IEnumerator DeactivateAfterDelay(Image image, float delay, int index)
    {
        if (index >= 0 && index < AllTowers.Count)
        {
            var tower = AllTowers[index];
        }

        if (image.gameObject.activeSelf)
        {
            float currentTime = 0;
            image.color = new Color(1f, 1f, 1f, 1f);
            Color startColor = new Color(1f, 1f, 1f, 1f);
            Color endColor = new Color(1f, 1f, 1f, 0);
            image.gameObject.SetActive(true);

            while (currentTime < delay)
            {
                float alpha = Mathf.Lerp(1, 0, currentTime / delay);
                image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                currentTime += Time.deltaTime;

                if (AllTowers[index].ActiveCooldown > 0)
                {
                    image.gameObject.SetActive(true);
                    image.color = new Color(0f, 0f, 0f, 1f);
                    yield break;
                }

                yield return null;
            }

            image.color = endColor;
            image.gameObject.SetActive(false);
        }
    }
}
