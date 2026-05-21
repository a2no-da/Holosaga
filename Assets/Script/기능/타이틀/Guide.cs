using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class Guide : MonoBehaviour
{
    private int textIndex = 1;
    public LocalizeStringEvent Title;
    public LocalizeStringEvent Text;
    public Image[] guideI;

    public void OnLeftButtonClicked()
    {
        ChangeText(-1); 
    }

    public void OnRightButtonClicked()
    {
        ChangeText(1); 
    }

    private void ChangeText(int change)
    {
        if ((textIndex == 1 && change == -1) || (textIndex == 8 && change == 1))
        {
            return;
        }
        else
        {
            textIndex += change;
        }

        string key = $"Guide-Title{textIndex}";
        string keyT = $"Guide-Text{textIndex}";
        if (Title != null)
        {
            Title.StringReference.TableEntryReference = key;
            Text.StringReference.TableEntryReference = keyT;

            Title.RefreshString();
            Text.RefreshString();
        }

        foreach (Image img in guideI)
        {
            img.gameObject.SetActive(false);
        }

        if (textIndex - 1 < guideI.Length)
        {
            guideI[textIndex - 1].gameObject.SetActive(true);
        }
    }
}
