using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance; 

    private Image fadeImage;
    public float fadeDuration = 1f;
    private bool isFirstLoad = true;
    private bool isLoad = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject outInPanel = GameObject.Find("outInPanel");
        if (outInPanel != null)
        {
            Transform outInTransform = outInPanel.transform.Find("OutIn");
            if (outInTransform != null)
            {
                fadeImage = outInTransform.GetComponent<Image>();
                if (fadeImage != null)
                {
                    if (isFirstLoad && (SceneManager.GetActiveScene().name != "Stage1" || SceneManager.GetActiveScene().name != "Infinite Stage" || SceneManager.GetActiveScene().name != "Sandbag"))
                    {
                        isFirstLoad = false;
                        return;
                    }
                    isLoad = true;
                    outInTransform.gameObject.SetActive(true);
                    StartCoroutine(Fade(1, 0));
                }
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));
    }

    IEnumerator ChangeScene(string sceneName)
    {
        if (sceneName == "Stage1" || sceneName == "Infinite Stage" || sceneName == "Sandbag") 
        {
            isLoad = false;
            yield return StartCoroutine(Fade(0, 1));
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float start, float end)
    {
        float elapsedTime = 0f;
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(start, end, elapsedTime / fadeDuration);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
                yield return null;
            }
            if (isLoad)
            {
                fadeImage.gameObject.SetActive(false);
            }
        }
    }
}