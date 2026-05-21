using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float alphaSpeed = 2f;
    public float destroyTime = 1f;
    public Vector3 randomIntensity = new Vector3(0.01f, 0, 0);

    private TextMeshProUGUI textMesh; 
    private Color alpha;
    private Vector3 direction;
    private float damage;
    private float scale;
    private float time;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        alpha = textMesh.color;
        direction = new Vector3(Random.Range(-randomIntensity.x, randomIntensity.x), 0);
        Destroy(gameObject, destroyTime);
        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
        direction.y = 0.1f / (0.01f + time); 
        //direction.y = 2f - 6f * time;
        direction.x -= direction.x / 200 * Time.deltaTime;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        //alpha.a = Mathf.Lerp(alpha.a, 0, alphaSpeed * Time.deltaTime);
        textMesh.color = alpha;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
        float damageNormalized = Mathf.InverseLerp(0, 1000, damage);
        textMesh.fontSize = Mathf.Lerp(0.75f, 1.5f, damageNormalized);
        //textMesh.fontSize = Mathf.Lerp(0.35f, 0.75f, damageNormalized);
        scale = Mathf.Clamp01(Mathf.Sqrt(damage) / 30);
        int intDamage = Mathf.FloorToInt(damage);
        textMesh.text = intDamage.ToString();

        transform.position = transform.position + new Vector3(0, 1f, 0); 
    }
}