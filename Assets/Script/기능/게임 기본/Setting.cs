using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using Spine;
using Spine.Unity;

public class Setting : MonoBehaviour
{
    public AudioMixer theMixer;

    public Slider bgmSlider; 
    public Slider vfxSlider;

    private const string BGM_PREF = "BGMVolume";
    private const string SFX_PREF = "SFXVolume";

    public TextMeshProUGUI S1;
    public TextMeshProUGUI S2;
    public TextMeshProUGUI S3;
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset japaneseFont;

    public Dicton[] dictions;
    public SkeletonGraphic skeletonGraphic;
    public SkeletonGraphic skeletonGraphic2;
    public SkeletonGraphic Back;

    private bool oneP;

    void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat(BGM_PREF, 0.5f);
        vfxSlider.value = PlayerPrefs.GetFloat(SFX_PREF, 0.5f);

        theMixer.SetFloat("BGM", bgmSlider.value);
        theMixer.SetFloat("SFX", vfxSlider.value);

        SetMusicVol();
        SetSFXVol();

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        UpdateFont(LocalizationSettings.SelectedLocale.Identifier.Code);
    }

    void Update()
    {
        if (Back != null)
        {

            if (Back.gameObject.activeSelf)
            {
                if (oneP == false)
                {
                    oneP = true;
                    PlaySpineUIAnimationOnceback("wide");
                }
            }
            else
            {
                oneP = false;
            }
        }
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(Locale newLocale)
    {
        UpdateFont(newLocale.Identifier.Code);
    }

    void UpdateFont(string localeCode)
    {
        TMP_FontAsset targetFont = localeCode == "ja" ? japaneseFont : defaultFont;

        if (S1 != null) S1.font = targetFont;
        if (S2 != null) S2.font = targetFont;
        if (S3 != null) S3.font = targetFont;
    }

    public void SetMusicVol()
    {
        float bgmValue = bgmSlider.value;
        if (bgmValue <= -40f)
        {
            theMixer.SetFloat("BGM", -80f);
        }
        else
        {
            theMixer.SetFloat("BGM", bgmValue);
        }
        PlayerPrefs.SetFloat(BGM_PREF, bgmValue);
    }

    public void SetSFXVol()
    {
        float vfxValue = vfxSlider.value;
        if (vfxValue <= -40f)
        {
            theMixer.SetFloat("SFX", -80f);
        }
        else
        {
            theMixer.SetFloat("SFX", vfxValue);
        }
        PlayerPrefs.SetFloat(SFX_PREF, vfxValue);
    }

    public void TimeGo()
    {
        Time.timeScale = 1;
    }

    public void GoMain()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("Main");
    }

    public void ReStart()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("Stage1");
    }

    public void ReStartIN()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("Infinite Stage");
    }

    public void LS()
    {
        //dictions;
    }

    public void PlaySpineUIAnimationOnce(string animationName)
    {
        if (skeletonGraphic == null)
        {
            return;
        }

        skeletonGraphic.AnimationState.ClearTracks();
        skeletonGraphic.AnimationState.SetAnimation(0, animationName, false);
    }

    public void PlaySpineUIAnimationOnce2(string animationName)
    {
        if (skeletonGraphic2 == null)
        {
            return;
        }

        skeletonGraphic2.AnimationState.ClearTracks();
        skeletonGraphic2.AnimationState.SetAnimation(0, animationName, false);
    }

    public void PlaySpineUIAnimationOnceback(string animationName)
    {
        if (Back == null)
        {
            return;
        }

        Back.AnimationState.ClearTracks();
        Back.AnimationState.SetAnimation(0, animationName, false);
    }
}