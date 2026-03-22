using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanelUI : MonoBehaviour
{
    [SerializeField] private BoardInteractionController controller;
    [SerializeField] private GameObject root;

    [Header("Buttons")]
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button mergeButton;
    [SerializeField] private Button transcendButton;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (unlockButton != null)
            unlockButton.onClick.AddListener(controller.OnClickUnlockSelectedCell);

        if (buildButton != null)
            buildButton.onClick.AddListener(controller.OnClickBuildSelectedCell);

        if (mergeButton != null)
            mergeButton.onClick.AddListener(controller.OnClickMergeSelectedTower);

        if (transcendButton != null)
            transcendButton.onClick.AddListener(controller.OnClickTranscendSelectedTower);

        Hide();
    }

    public void ShowUnlock(Vector2Int cellPos, int cost)
    {
        ShowRoot();
        HideAllButtons();

        if (unlockButton != null)
            unlockButton.gameObject.SetActive(true);

        Debug.Log($"[UI] Locked Cell {cellPos} / Unlock Cost = {cost}");
    }

    public void ShowBuild(Vector2Int cellPos, TowerData buildData)
    {
        ShowRoot();
        HideAllButtons();

        if (buildButton != null)
            buildButton.gameObject.SetActive(buildData != null);

        if (buildData != null)
        {
            Debug.Log($"[UI] Build Cell {cellPos} / Build Tower = {buildData.towerId} / Cost = {buildData.buildCost}");
        }
    }

    public void ShowTowerActions(TowerInstance tower, bool canMerge, bool canTranscend)
    {
        ShowRoot();
        HideAllButtons();

        if (mergeButton != null)
            mergeButton.gameObject.SetActive(canMerge);

        if (transcendButton != null)
            transcendButton.gameObject.SetActive(canTranscend);

        if (tower != null && tower.Data != null)
        {
            Debug.Log($"[UI] Tower Selected = {tower.Data.towerId} / Grade = {tower.Data.grade}");
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
}
