using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Local : MonoBehaviour
{
    bool isAt;

    public void ChangeL(int index)
    {
        if (isAt)
            return;
        StartCoroutine(ChangeLL(index));
    }

    IEnumerator ChangeLL(int index)
    {
        isAt = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isAt = false;
    }
}
