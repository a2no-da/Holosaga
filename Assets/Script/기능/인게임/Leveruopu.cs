using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Event = Spine.Event;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI; 

public class Leveruopu : MonoBehaviour
{
    public SkeletonGraphic skeletonGraphic; 
    private string currentLanguage;

    void Start()
    {
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
                ame = "korean";
                break;
        }
        ChangeSkin(ame, skeletonGraphic);
    }

    public void ChangeSkin(string skinName, SkeletonGraphic skeletonGraphic)
    {
        if (string.IsNullOrEmpty(skinName)) return;
        var skeletonData = skeletonGraphic.SkeletonDataAsset.GetSkeletonData(true);
        var skin = skeletonData.FindSkin(skinName);
        if (skin == null) return;
        skeletonGraphic.initialSkinName = skinName;
        skeletonGraphic.Skeleton.SetSkin(skin);
        skeletonGraphic.Skeleton.SetSlotsToSetupPose();
        skeletonGraphic.AnimationState.Apply(skeletonGraphic.Skeleton);
    }
}
