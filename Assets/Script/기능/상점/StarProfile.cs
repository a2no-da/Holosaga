using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarProfile : MonoBehaviour
{
    public Dicton towerInfo;
    public Image[] starImages;

    private int previousLevelSS;
    private bool previousPurchasable;

    void Start()
    {
        if (towerInfo != null)
        {
            UpdateState(towerInfo.isPurchasable, towerInfo.LevelSS);
            previousLevelSS = towerInfo.LevelSS;
        }
    }

    void Update()
    {
        if (towerInfo.LevelSS != previousLevelSS)
        {
            UpdateState(towerInfo.isPurchasable, towerInfo.LevelSS);
            previousLevelSS = towerInfo.LevelSS;
        }

        if(towerInfo.isPurchasable != previousPurchasable)
        {
            UpdateState(towerInfo.isPurchasable, towerInfo.LevelSS);
            previousPurchasable = towerInfo.isPurchasable;
        }
    }

    public void UpdateState(bool isPurchasable, int LevelSS)
    {
        if (!isPurchasable)
        {
            foreach (var starImage in starImages)
            {
                starImage.gameObject.SetActive(true);
            }

            if (LevelSS == 1)
            {
                for (int i = 1; i < starImages.Length; i++)
                {
                    starImages[i].gameObject.SetActive(false);
                }
            }
            else if (LevelSS == 2)
            {
                for (int i = 2; i < starImages.Length; i++)
                {
                    starImages[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var starImage in starImages)
            {
                starImage.gameObject.SetActive(false);
            }
        }
    }
}
