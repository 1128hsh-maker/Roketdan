using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FitCameraToBackgroundHeight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    private void Apply()
    {
        if (backgroundRenderer == null)
        {
            Debug.LogWarning("Background Renderer가 연결되지 않았습니다.");
            return;
        }

        cam.orthographic = true;

        float bgHeight = backgroundRenderer.bounds.size.y;
        cam.orthographicSize = bgHeight * 0.5f;

        Vector3 center = backgroundRenderer.bounds.center;
        transform.position = new Vector3(center.x, center.y, transform.position.z);
    }
}
