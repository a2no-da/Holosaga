using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class UpSS : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    private string currentLanguage;

    void Start()
    {
        skeletonAnimation.AnimationState.Complete += HandleAnimationComplete;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        CCS();
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        currentLanguage = newLocale.Identifier.Code;
    }

    public void CCS()
    {
        string ame = null; 

        switch (currentLanguage)
        {
            case "ko":
                ame = "korean";
                break;
            case "en":
                ame = "english";
                break;
            case "ja":
                ame = "japanese";
                break;
            default:
                ame = "artGetKorean";
                break;
        }
        ChangeSkin(ame, skeletonAnimation);
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

    public void ChangeAnimationBasedOnLanguage(string name)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, name, false);
    }

    private void HandleAnimationComplete(Spine.TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}