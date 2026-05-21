using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sushi : MonoBehaviour
{
    public RectTransform panel;
    public List<GameObject> prefabs;
    public int poolSize = 10;
    public float speed = 1f;
    public float resetPosition = 10f;
    public float endPosition = -10f;
    public float spawnRate = 3f;
    public float Downtown;

    private int currentPrefabIndex = 0;
    int currentObjectIndex = 0;
    private List<GameObject> objects;
    private float timer = 0f;

    void Start()
    {
        objects = new List<GameObject>();
        for (int i = 0; i < poolSize * prefabs.Count; i++)
        {
            GameObject prefab = prefabs[currentPrefabIndex];
            GameObject obj = Instantiate(prefab, panel.transform);
            obj.transform.localPosition = new Vector3(resetPosition, Downtown, 0);
            obj.SetActive(false);
            objects.Add(obj);

            currentPrefabIndex = (currentPrefabIndex + 1) % prefabs.Count;
        }

        int[] xPositions = new int[] { -385, -80, 195, 480, 765};
        for (int i = 0; i < 5; i++)
        {
            var obj = objects[i];
            obj.transform.localPosition = new Vector3(xPositions[i], Downtown, 0);
            obj.SetActive(true);
        }
        currentObjectIndex = 5;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            var obj = objects[currentObjectIndex];

            if (!obj.activeInHierarchy)
            {
                obj.transform.localPosition = new Vector3(resetPosition, Downtown, 0);
                obj.SetActive(true);

                timer = 0f;

                currentObjectIndex++;
                if (currentObjectIndex >= objects.Count)
                {
                    currentObjectIndex = 0;
                }
            }
        }

        foreach (var obj in objects)
        {
            if (obj.activeInHierarchy)
            {
                obj.transform.localPosition += Vector3.left * speed * Time.deltaTime;

                if (obj.transform.localPosition.x <= endPosition)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
