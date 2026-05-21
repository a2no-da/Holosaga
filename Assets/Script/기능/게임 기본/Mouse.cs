using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.SceneManagement;

public class Mouse : MonoBehaviour
{
    public GameObject effectPrefab;
    public static Mouse instance;
    private Texture2D currentCursorImg;

    [SerializeField] Texture2D cursorImg;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Instantiate(effectPrefab, mousePosition, Quaternion.identity);
        }

        if (currentCursorImg != cursorImg)
        {
            Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);
            currentCursorImg = cursorImg;
        }
    }
}
