using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FitCameraToBackground : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        if (backgroundRenderer != null && cam != null)
            Apply();
    }
#endif

    [ContextMenu("Apply Fit")]
    public void Apply()
    {
        if (backgroundRenderer == null)
        {
            Debug.LogWarning("[FitCameraToBackgroundHeight] backgroundRenderer가 연결되지 않았습니다.");
            return;
        }

        cam.orthographic = true;

        float bgHeight = backgroundRenderer.bounds.size.y;
        cam.orthographicSize = bgHeight * 0.5f;

        Vector3 center = backgroundRenderer.bounds.center;
        transform.position = new Vector3(center.x, center.y, transform.position.z);
    }
}
