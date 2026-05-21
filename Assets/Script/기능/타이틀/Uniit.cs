using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Uniit : MonoBehaviour
{
    public GameObject UnitC;
    public GameObject SkinC;
    public GameObject BackC;
    public GameObject UpC;

    public GameObject Back1;
    public GameObject Back2;

    public Button UnitButton;
    public Button SkinButton;
    public Button BackButton;
    public Button UpButton;

    public Image warning;
    public int difficulty;
    public TextMeshProUGUI LevelT;
    public TextMeshProUGUI SM;
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset japaneseFont;
    public LocalizeStringEvent localizeStringEvent;
    public Upgrade upgrade;

    private Button selectedButton;
    public UnlockM unlockM;
    public ArtifactS artifacts;
    public ArtifactD artifactD;
    public GameObject gamePart;
    public GameObject gameMO;
    public Image def;
    public Sprite[] sprites;
    public Holomoney holomoney;
    public TextMeshProUGUI TEX;
    public Image dddd;
    public Image dddd2;
    public GameObject nomineral;
    public GameObject TX;
    public GameObject JwaU;
    public SelController selController;
    public List<string> MaeselectedUnitIds;
    public UnitSelectionScreen unitSelectionScreen;

    void Start()
    {
        if (BackC != null)
        {
            UnitCC();
        }else
        {
            UnitCCC();
        }

        if (localizeStringEvent != null)
        {
            switch (DifficultyManager.Instance.DifficultyMode)
            {
                case DifficultyButton.DifficultyMode.Basics:
                    if (LevelT != null)
                    {
                        difficulty = DifficultyManager.Instance.Difficulty;
                        UpdateLevelText(difficulty);
                        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
                        UpdateFont(LocalizationSettings.SelectedLocale.Identifier.Code);
                        dddd.GetComponent<Image>().sprite = sprites[difficulty];
                        if (dddd2 != null)
                        {
                            dddd2.GetComponent<Image>().sprite = sprites[difficulty];
                        }
                    }
                    else
                    {
                        difficulty = DifficultyManager.Instance.Difficulty;
                        UpdateImageText(difficulty);
                    }
                    break;
                case DifficultyButton.DifficultyMode.Infinite:
                    if (LevelT != null)
                    {
                        difficulty = DifficultyManager.Instance.Difficulty;
                        if (difficulty == 0)
                        {
                            DifficultyManager.Instance.Difficulty = difficulty + 1;
                            difficulty = DifficultyManager.Instance.Difficulty;
                        }

                        if (difficulty == 1)
                        {
                            DifficultyManager.Instance.Difficulty = difficulty + 1;
                            difficulty = DifficultyManager.Instance.Difficulty;
                        }
                        UpdateLevelText(difficulty);
                        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
                        UpdateFont(LocalizationSettings.SelectedLocale.Identifier.Code);
                        dddd.GetComponent<Image>().sprite = sprites[difficulty];
                        if (dddd2 != null)
                        {
                            dddd2.GetComponent<Image>().sprite = sprites[difficulty];
                        }
                    }
                    else
                    {
                        difficulty = DifficultyManager.Instance.Difficulty;
                        if (difficulty == 0)
                        {
                            DifficultyManager.Instance.Difficulty = difficulty + 1;
                            difficulty = DifficultyManager.Instance.Difficulty;
                        }

                        if (difficulty == 1)
                        {
                            DifficultyManager.Instance.Difficulty = difficulty + 1;
                            difficulty = DifficultyManager.Instance.Difficulty;
                        }
                        UpdateImageText(difficulty);
                    }
                    break;
            }
        }

        if (SceneManager.GetActiveScene().name == "UnitSelect")
        {
            string savedIds = PlayerPrefs.GetString("SelectedUnitIds", "");
            MaeselectedUnitIds = new List<string>(savedIds.Split(','));
            if (MaeselectedUnitIds != null && MaeselectedUnitIds.Count > 0)
            {
                float initialX = 928.4f; 
                float initialY = -964.5f;  
                float spacing = 153.3f;   

                int index = 0;

                foreach (var unitId in MaeselectedUnitIds)
                {
                    foreach (var job in selController.Job)
                    {
                        Drop drop = job.GetComponent<Drop>();
                        if (drop != null && drop.UnitId == unitId)
                        {
                            foreach (var sel in selController.selObjects)
                            {
                                if (!sel.IsUnitDropped)
                                {
                                    sel.SetDroppedObject(drop.unit, drop.UnitId);
                                    drop.currentSel = sel;

                                    RectTransform dropRect = drop.GetComponent<RectTransform>();
                                    if (dropRect != null)
                                    {
                                        dropRect.anchoredPosition = new Vector2(
                                            initialX + (spacing * index), 
                                            initialY                    
                                        );
                                    }
                                    index++;

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "UnitSelect" && DifficultyManager.Instance.DifficultyMode == DifficultyButton.DifficultyMode.Sandbag)
        {
            TX.SetActive(false);
            JwaU.SetActive(false);
        }
    }

    void OnLocaleChanged(Locale newLocale)
    {
        if (localizeStringEvent != null)
        {
            UpdateFont(newLocale.Identifier.Code);
        }
    }

    void UpdateFont(string localeCode)
    {
        if (localeCode == "ja")
        {
            LevelT.font = japaneseFont;
            if (SM != null)
            {
                SM.font = japaneseFont;
            }
        }
        else
        {
            LevelT.font = defaultFont;
            if (SM != null)
            {
                SM.font = defaultFont;
            }
        }
    }

    public void UpdateLevelText(int difficulty)
    {
        difficulty++;
        string key = $"UnitSelect-Difficulty{difficulty}";
        if (localizeStringEvent != null)
        {
            localizeStringEvent.StringReference.TableEntryReference = key;
            localizeStringEvent.RefreshString();
        }
    }


    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    public void UpdateImageText(int difficulty)
    {
        def.GetComponent<Image>().sprite = sprites[difficulty];

        string mat = null;

        switch (difficulty)
        {
            case 0:
                mat = "Easy";
                break;
            case 1:
                mat = "Middle";
                break;
            case 2:
                mat = "Hard";
                break;
            case 3:
                mat = "Extreme";
                break;
        }

        var localizedString = new LocalizedString { TableReference = "UI", TableEntryReference = "Main-UI-" + mat };

        localizedString.StringChanged += (string localizedText) =>
        {
            TEX.text = localizedText;
        };
    }

    public void Left()
    {
        switch (DifficultyManager.Instance.DifficultyMode)
        {
            case DifficultyButton.DifficultyMode.Basics:
                if (difficulty > 0)
                {
                    difficulty -= 1;
                    DifficultyManager.Instance.Difficulty = difficulty;
                    UpdateImageText(difficulty);
                }
                break;
            case DifficultyButton.DifficultyMode.Infinite:
                if (difficulty > 2)
                {
                    difficulty -= 1;
                    DifficultyManager.Instance.Difficulty = difficulty;
                    UpdateImageText(difficulty);
                }
                break;
        }
    }

    public void Right()
    {
        switch (DifficultyManager.Instance.DifficultyMode)
        {
            case DifficultyButton.DifficultyMode.Basics:
                if (difficulty < sprites.Length - 1)
                {
                    difficulty += 1;
                    DifficultyManager.Instance.Difficulty = difficulty;
                    UpdateImageText(difficulty);
                }
                break;
            case DifficultyButton.DifficultyMode.Infinite:
                if (difficulty < sprites.Length - 1)
                {
                    difficulty += 1;
                    DifficultyManager.Instance.Difficulty = difficulty;
                    UpdateImageText(difficulty);
                }
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Stage1" && SceneManager.GetActiveScene().name != "Infinite Stage" && SceneManager.GetActiveScene().name != "Sandbag")
        {
            SceneLoader.Instance.LoadScene("Main");
        }
    }

    public void LoadStage1()
    {
        if (UnitSelectionScreen.Instance.IsAllUnitsSelected())
        {
            MaeselectedUnitIds = unitSelectionScreen.selectedUnitIds;
            PlayerPrefs.SetString("SelectedUnitIds", string.Join(",", MaeselectedUnitIds));
            PlayerPrefs.Save();
            if (DifficultyManager.Instance.DifficultyMode == DifficultyButton.DifficultyMode.Basics)
            {
                SceneLoader.Instance.LoadScene("Stage1");
            }
            else if (DifficultyManager.Instance.DifficultyMode == DifficultyButton.DifficultyMode.Infinite)
            {
                SceneLoader.Instance.LoadScene("Infinite Stage");
            }
            else if (DifficultyManager.Instance.DifficultyMode == DifficultyButton.DifficultyMode.Sandbag)
            {
                SceneLoader.Instance.LoadScene("Sandbag");
            }
        }
        else
        {
            warning.gameObject.SetActive(true);
        }
    }

    public void SaveNan()
    {
        PlayerData playerData = holomoney.LoadPlayerData();
        playerData.SellastNan = DifficultyManager.Instance.Difficulty;
        holomoney.SavePlayerData(playerData);
    }

    public void MainGO()
    {
        SceneLoader.Instance.LoadScene("Main");
    }

    public void UnitCC()
    {
        if (UnitC == null) { return; }
        DeselectAllButtons();
        SelectButton(UnitButton);
        BackC.gameObject.SetActive(false);
        SkinC.gameObject.SetActive(false);
        UpC.gameObject.SetActive(false);
        Back1.gameObject.SetActive(true);
        Back2.gameObject.SetActive(false);
        gamePart.SetActive(false);
        gameMO.SetActive(true);
        upgrade.ResetInfo();
        UnitC.gameObject.SetActive(true);
    }

    public void SkinCC()
    {
        DeselectAllButtons();
        SelectButton(SkinButton);
        BackC.gameObject.SetActive(false);
        UnitC.gameObject.SetActive(false);
        UpC.gameObject.SetActive(false);
        SkinC.gameObject.SetActive(true);
        Back1.gameObject.SetActive(true);
        Back2.gameObject.SetActive(false);
        gamePart.SetActive(false);
        gameMO.SetActive(true);
        upgrade.ResetInfo();
    }

    public void BackCC()
    {
        DeselectAllButtons();
        SelectButton(BackButton);
        UnitC.gameObject.SetActive(false);
        SkinC.gameObject.SetActive(false);
        UpC.gameObject.SetActive(false);
        BackC.gameObject.SetActive(true);
        Back1.gameObject.SetActive(true);
        Back2.gameObject.SetActive(false);
        gamePart.SetActive(true);
        gameMO.SetActive(false);
        upgrade.ResetInfo();
    }

    public void UpCC()
    {
        DeselectAllButtons();
        SelectButton(UpButton);
        unlockM.UpdateButtonT();
        UnitC.gameObject.SetActive(false);
        SkinC.gameObject.SetActive(false);
        BackC.gameObject.SetActive(false);
        UpC.gameObject.SetActive(true);
        Back1.gameObject.SetActive(false);
        Back2.gameObject.SetActive(true);
        gamePart.SetActive(false);
        gameMO.SetActive(true);
        upgrade.ResetInfo();
    }

    public void UnitCCC()
    {
        if (UnitC == null) { return; }
        DeselectAllButtons();
        SelectButton(UnitButton);
        SkinC.gameObject.SetActive(false);
        artifactD.Reset();
        artifacts.UpdateArtifactButtons();
        upgrade.ResetInfoAt();
        UnitC.gameObject.SetActive(true);
    }

    public void SkinCCC()
    {
        DeselectAllButtons();
        SelectButton(SkinButton);
        artifactD.first = true;
        artifactD.UpdateArtifactButtons();
        UnitC.gameObject.SetActive(false);
        SkinC.gameObject.SetActive(true);
        upgrade.ResetInfoAt();
    }

    public void Ok()
    {
        Time.timeScale = 1f;
    }

    private void DeselectAllButtons()
    {
        if (selectedButton != null)
        {
            selectedButton.interactable = true;
        }
    }

    private void SelectButton(Button button)
    {
        selectedButton = button;
        selectedButton.interactable = false;
    }
}
