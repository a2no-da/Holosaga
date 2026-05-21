using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public Camera mainCamera; 
    public Camera subCamera; 
    public float zoomedSize = 5f; // 줌인할 Orthographic Size
    public float normalSize = 5.2f; // 원래 Orthographic Size
    public float zoomDuration = 2f;
    public float gas;

    private void Start()
    {
        subCamera.orthographicSize = zoomedSize;
        StartCoroutine(ZoomOut());
    }

    private IEnumerator ZoomOut()
    {
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            float curveValue = Mathf.SmoothStep(0f, 1f, t);

            subCamera.orthographicSize = Mathf.Lerp(zoomedSize, normalSize, curveValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        subCamera.orthographicSize = normalSize;

        subCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        GameManager.instance.isokCamera = false; 
    }
}
