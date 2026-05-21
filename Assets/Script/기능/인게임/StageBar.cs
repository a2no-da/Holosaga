using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageBar : MonoBehaviour
{
    public Image progressBar;
    public float maxTime;
    public float currentTime;

    public void SetMaxTime(float max)
    {
        maxTime = max;
        currentTime = 0;
        UpdateProgress();
    }

    public void IncreaseProgress(float amount)
    {
        currentTime += amount;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        progressBar.fillAmount = currentTime / maxTime;
    }
}