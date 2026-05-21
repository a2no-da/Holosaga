using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public Image whiteImage;
    public float fadeDuration;

    private Coroutine fadeCoroutine;

    public void StartFadeOut()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        whiteImage.gameObject.SetActive(true);
        Color originalColor = whiteImage.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            whiteImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime);
            yield return null;
        }
        whiteImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        whiteImage.gameObject.SetActive(false);

        fadeCoroutine = null;
    }
}
