using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Giftbox : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Holomoney holomoney;
    private string currentLanguage;

    void Start()
    {
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;

        PlayerData data = holomoney.LoadPlayerData();
        data.BoxCount++;
        holomoney.SavePlayerData(data);
        ChangeAnimationBasedOnLanguage("boxGetKorean");
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        currentLanguage = newLocale.Identifier.Code;
    }

    private void ChangeAnimationBasedOnLanguage(string baseAnimationName)
    {
        skeletonAnimation.transform.position = new Vector3(-0.97f, 0, 0);

        string animationName = baseAnimationName;

        switch (currentLanguage)
        {
            case "ko":
                animationName = "boxGetKorean";
                break;
            case "en":
                animationName = "boxGetEnglish";
                break;
            case "ja":
                animationName = "boxGetJapanese";
                break;
            default:
                animationName = "boxGetKorean"; 
                break;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
