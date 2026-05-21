using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CriText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float alphaSpeed = 2f;
    public float destroyTime = 1f;
    public Vector3 randomIntensity = new Vector3(0.01f, 0, 0);

    private RectTransform imgTransform;
    public TextMeshProUGUI textMesh;
    private Color alpha;
    private Vector3 direction;
    private float damage;
    private float scale;
    private float time;

    private void Start()
    {
        imgTransform = GetComponent<RectTransform>();
        alpha = GetComponent<Image>().color;
        direction = new Vector3(Random.Range(-randomIntensity.x, randomIntensity.x), 0);
        Destroy(gameObject, destroyTime);
        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
        direction.y = 0.1f / (0.01f + time);
        direction.x -= direction.x / 200 * Time.deltaTime;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        GetComponent<Image>().color = alpha; 
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
        int intDamage = Mathf.FloorToInt(damage);
        textMesh.text = intDamage.ToString();
        if (imgTransform == null)
        {
            imgTransform = GetComponent<RectTransform>();
        }

        float damageNormalized = Mathf.InverseLerp(0, 1000, damage);
        float newScale = Mathf.Lerp(0.75f, 1.5f, damageNormalized);
        imgTransform.localScale = new Vector3(newScale, newScale, 1f);
        scale = Mathf.Clamp01(Mathf.Sqrt(damage) / 30);

        transform.position = transform.position + new Vector3(0, 1f, 0);

        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = true; 
        }
    }
}
