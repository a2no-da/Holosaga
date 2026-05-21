using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DropArti : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Artifact artifact;
    private string currentLanguage;

    void Start()
    {
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        CCS();
        ChangeAnimationBasedOnLanguage(currentLanguage);
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        currentLanguage = newLocale.Identifier.Code;
    }

    public void CCS()
    {
        string meg;

        switch (artifact.grade)
        {
            case Grade.Normal:
                meg = "c";
                break;
            case Grade.Epic:
                meg = "r";
                break;
            case Grade.Unique:
                meg = "u";
                break;
            default:
                meg = null;
                break;
        }

        ChangeSkin(meg + "_ar" + artifact.index, skeletonAnimation);
    }

    public void ChangeSkin(string skinName, SkeletonAnimation skeletonAnimation)
    {
        if (string.IsNullOrEmpty(skinName)) return;
        var skeletonData = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true);
        var skin = skeletonData.FindSkin(skinName);
        if (skin == null) return;
        skeletonAnimation.initialSkinName = skinName;
        skeletonAnimation.Skeleton.SetSkin(skin);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
    }

    private void ChangeAnimationBasedOnLanguage(string baseAnimationName)
    {
        string animationName = baseAnimationName;

        switch (currentLanguage)
        {
            case "ko":
                animationName = "artGetKorean";
                break;
            case "en":
                animationName = "artGetEnglish";
                break;
            case "ja":
                animationName = "artGetJapanese";
                break;
            default:
                animationName = "artGetKorean";
                break;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}

