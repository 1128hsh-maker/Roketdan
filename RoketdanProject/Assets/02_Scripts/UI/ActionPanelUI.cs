using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanelUI : MonoBehaviour
{
    [SerializeField] private BoardInteractionController controller;
    [SerializeField] private GameObject root;

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, -80f);

    [Header("Buttons")]
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button mergeButton;
    [SerializeField] private Button transcendButton;

    private RectTransform panelRect;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        panelRect = root.GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (worldCamera == null)
            worldCamera = Camera.main;

        if (unlockButton != null)
            unlockButton.onClick.AddListener(controller.OnClickUnlockSelectedCell);

        if (buildButton != null)
            buildButton.onClick.AddListener(controller.OnClickBuildSelectedCell);

        if (mergeButton != null)
            mergeButton.onClick.AddListener(controller.OnClickPromoteSelectedHero);

        Hide();
    }

    public void ShowUnlock(Vector2Int cellPos, int cost, Vector3 worldPos)
    {
        ShowRoot();
        HideAllButtons();
        MovePanelToWorld(worldPos);

        if (unlockButton != null)
            unlockButton.gameObject.SetActive(true);

        Debug.Log($"[UI] Locked Cell {cellPos} / Unlock Cost = {cost}");
    }

    public void ShowBuild(Vector2Int cellPos, int cost, Vector3 worldPos)
    {
        ShowRoot();
        HideAllButtons();
        MovePanelToWorld(worldPos);

        if (buildButton != null)
            buildButton.gameObject.SetActive(true);

        Debug.Log($"[UI] Build Cell {cellPos} / Summon Cost = {cost}");
    }

    public void ShowHeroActions(HeroInstance hero, bool canPromote, bool canTranscend, Vector3 worldPos)
    {
        ShowRoot();
        HideAllButtons();
        MovePanelToWorld(worldPos);

        if (mergeButton != null)
            mergeButton.gameObject.SetActive(canPromote);

        if (transcendButton != null)
            transcendButton.gameObject.SetActive(canTranscend);

        if (hero != null && hero.Data != null)
        {
            Debug.Log($"[UI] Hero Selected = {hero.Data.heroId} / Grade = {hero.Data.grade}");
        }
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void ShowRoot()
    {
        if (root != null)
            root.SetActive(true);
    }

    private void HideAllButtons()
    {
        if (unlockButton != null)
            unlockButton.gameObject.SetActive(false);

        if (buildButton != null)
            buildButton.gameObject.SetActive(false);

        if (mergeButton != null)
            mergeButton.gameObject.SetActive(false);

        if (transcendButton != null)
            transcendButton.gameObject.SetActive(false);
    }

    private void MovePanelToWorld(Vector3 worldPos)
    {
        if (panelRect == null || canvas == null)
            return;

        Camera camForWorld = worldCamera != null ? worldCamera : Camera.main;
        Camera camForCanvas = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camForWorld, worldPos);
        screenPoint += screenOffset;

        RectTransform canvasRect = canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, camForCanvas, out Vector2 localPoint))
        {
            panelRect.anchoredPosition = localPoint;
        }
    }
}