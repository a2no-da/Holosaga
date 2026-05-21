using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization.Settings;
using System;

public class rol : MonoBehaviour
{
    public Image panel1; 
    public Image logo1; 

    public Image panel2; 
    public Image logo2;

    public Image Select;
    public Image Setting;
    public Image Credit;
    public DictonManager dictonManager;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown QualityDropdown;
    public TMP_Dropdown LanguageDropdown;
    private Coroutine fadeOnCoroutine;
    private bool isEscPressed = false;
    private static bool hasStarted = false;
    public GameObject exitPanel;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerPrefs.DeleteKey("hasStarted");
    }

    void Start()
    {
        if (!hasStarted)
        {
            PlayerPrefs.DeleteKey("SelectedUnitIds"); //데이터 삭★제★
            StartCoroutine(FadeInAndOutSequence());
            hasStarted = true;
        }
        if(panel1 != null)
        {
            Time.timeScale = 1f;
        }

        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 1);
        resolutionDropdown.value = resolutionIndex; 
        ChangeResolution(resolutionIndex); 

        QualityDropdown.value = 2;
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        InitializeDropdown();
        InitializeQualityDropdown();
        if (PlayerPrefs.HasKey("SelectedLanguage"))
        {
            int selectedLanguageIndex = PlayerPrefs.GetInt("SelectedLanguage");
            LanguageDropdown.value = selectedLanguageIndex;
            ChangeLanguage(selectedLanguageIndex);
        }
    }

    void Update()
    {
        if (panel1 != null)
        {
            if (Input.anyKeyDown)
            {
                if (panel1.gameObject.activeSelf)
                {
                    isEscPressed = true;
                    StopCoroutine(FadeInAndOut(panel1, logo1));
                    panel1.gameObject.SetActive(false);
                    logo1.gameObject.SetActive(false);

                    StartCoroutine(FadeInAndOut(panel2, logo2));
                }
                else if (panel2.gameObject.activeSelf)
                {
                    StopCoroutine(FadeInAndOut(panel2, logo2));
                    panel2.gameObject.SetActive(false);
                    logo2.gameObject.SetActive(false);
                    BGMManager.instance.StartBGMAfterLogo();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DisableIfActive(Setting.gameObject);
                DisableIfActive(Select.gameObject);
            }
        }
    }

    IEnumerator FadeInAndOutSequence()
    {
        yield return StartCoroutine(FadeInAndOut(panel1, logo1));
        if (!isEscPressed)
        {
            yield return StartCoroutine(FadeInAndOut(panel2, logo2));
        }
    }

    IEnumerator FadeInAndOut(Image panel, Image logo)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(true);
            logo.gameObject.SetActive(true);

            yield return StartCoroutine(FadeIn(logo));
            yield return StartCoroutine(FadeOut(logo));

            panel.gameObject.SetActive(false);
            logo.gameObject.SetActive(false);
        }
    }

    IEnumerator FadeIn(Image img)
    {
        float fadeSpeed = 0.5f; 
        for (float i = 0; i <= 1; i += Time.deltaTime * fadeSpeed)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, i);
            yield return null;
        }
    }

    IEnumerator FadeOut(Image img)
    {
        float fadeSpeed = 0.5f; 
        for (float i = 1; i >= 0; i -= Time.deltaTime * fadeSpeed)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, i);
            yield return null;
        }

        if(img == logo2)
        {
            BGMManager.instance.StartBGMAfterLogo();
        }
    }

    IEnumerator FadeInC(Image img)
    {
        float fadeSpeed = 0.5f;
        for (float i = 0f; i <= 0.75f; i += Time.deltaTime * fadeSpeed)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, i);
            yield return null;
        }
    }

    public void CreditOn()
    {
        Credit.gameObject.SetActive(true);
        if (fadeOnCoroutine != null)
        {
            StopCoroutine(fadeOnCoroutine);
        }
        StartCoroutine(FadeInC(Credit));
    }

    public void CreditOff()
    {
        fadeOnCoroutine = StartCoroutine(FadeOut(Credit));
        Credit.gameObject.SetActive(false);
    }

    void DisableIfActive(GameObject obj)
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
    }

    public void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(1920, 1080, true);
                break;
            case 1:
                Screen.SetResolution(1280, 720, false);
                break;
            case 2:
                Screen.SetResolution(1600, 900, false);
                break;
            case 3:
                Screen.SetResolution(1920, 1080, false);
                break;
            case 4:
                Screen.SetResolution(2560, 1440, false);
                break;
            default:
                break;
        }
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save(); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void InitializeDropdown()
    {
        LanguageDropdown.ClearOptions();

        List<string> languages = new List<string> { "한국어", "English", "日本語" };

        foreach (var language in languages)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(language);
            LanguageDropdown.options.Add(option);
        }

        LanguageDropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    private void ChangeLanguage(int index)
    {
        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;

        PlayerPrefs.SetInt("SelectedLanguage", index);
        PlayerPrefs.Save();
    }

    private void InitializeQualityDropdown()
    {
        QualityDropdown.ClearOptions(); 

        List<string> options = new List<string> { "LOW", "MIDDLE", "HIGH" };
        QualityDropdown.AddOptions(options);
        QualityDropdown.value = QualitySettings.GetQualityLevel();
        QualityDropdown.onValueChanged.AddListener(delegate { ChangeQuality(QualityDropdown.value); });
    }

    public void ChangeQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    public void Exi()
    {
        exitPanel.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
