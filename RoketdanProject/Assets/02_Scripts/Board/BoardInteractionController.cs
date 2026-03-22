using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardInteractionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BoardGridByBounds boardGrid;
    [SerializeField] private BoardRuntimeManager boardRuntime;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private ActionPanelUI actionPanelUI;

    private bool hasSelectedCell;
    private Vector2Int selectedCell;
    private TowerInstance selectedTower;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (actionPanelUI != null)
            actionPanelUI.Hide();
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI(-1))
                return;

            HandlePointer(Input.mousePosition);
        }
#endif

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            int fingerId = Input.GetTouch(0).fingerId;

            if (IsPointerOverUI(fingerId))
                return;

            HandlePointer(Input.GetTouch(0).position);
        }
    }

    private bool IsPointerOverUI(int pointerId)
    {
        if (EventSystem.current == null)
            return false;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (pointerId == -1)
            return EventSystem.current.IsPointerOverGameObject();
#endif

        return EventSystem.current.IsPointerOverGameObject(pointerId);
    }

    private void HandlePointer(Vector2 screenPos)
    {
        Vector3 world = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z)
        );
        world.z = 0f;

        if (!boardGrid.TryWorldToCell(world, out Vector2Int cellPos))
        {
            ClearSelection();
            return;
        }

        SelectCell(cellPos);
    }

    private void SelectCell(Vector2Int cellPos)
    {
        hasSelectedCell = true;
        selectedCell = cellPos;
        selectedTower = null;

        CellRuntime cell = boardRuntime.GetCell(cellPos.x, cellPos.y);
        if (cell == null)
        {
            ClearSelection();
            return;
        }

        if (cell.cellType == CellType.Blocked || cell.cellType == CellType.Path)
        {
            actionPanelUI.Hide();
            return;
        }

        if (cell.cellType == CellType.Locked && !cell.HasTower)
        {
            actionPanelUI.ShowUnlock(cellPos, boardRuntime.DefaultUnlockCost);
            return;
        }

        if (cell.cellType == CellType.Buildable && !cell.HasTower)
        {
            TowerData buildData = towerManager.GetBuildDataForCell(cell);
            actionPanelUI.ShowBuild(cellPos, buildData);
            return;
        }

        if (cell.HasTower)
        {
            selectedTower = cell.placedTower;

            bool canMerge = towerManager.CanMerge(selectedTower);
            bool canTranscend = towerManager.CanTranscend(selectedTower);

            actionPanelUI.ShowTowerActions(selectedTower, canMerge, canTranscend);
            return;
        }

        actionPanelUI.Hide();
    }

    public void OnClickUnlockSelectedCell()
    {
        if (!hasSelectedCell)
            return;

        CellRuntime cell = boardRuntime.GetCell(selectedCell.x, selectedCell.y);
        if (cell == null || cell.cellType != CellType.Locked)
            return;

        int cost = boardRuntime.DefaultUnlockCost;

        if (!currencyManager.Spend(cost))
        {
            Debug.Log("[BoardInteraction] 골드 부족으로 잠금 해제 실패");
            return;
        }

        if (boardRuntime.UnlockCell(selectedCell.x, selectedCell.y))
        {
            SelectCell(selectedCell);
        }
    }

    public void OnClickBuildSelectedCell()
    {
        if (!hasSelectedCell)
            return;

        if (towerManager.TryBuildDefault(selectedCell, out TowerInstance builtTower))
        {
            selectedTower = builtTower;
            SelectCell(selectedCell);
        }
    }

    public void OnClickMergeSelectedTower()
    {
        if (selectedTower == null)
            return;

        if (towerManager.TryMerge(selectedTower, out TowerInstance mergedTower))
        {
            selectedTower = mergedTower;
            hasSelectedCell = true;
            selectedCell = mergedTower.CellPos;
            SelectCell(selectedCell);
        }
    }

    public void OnClickTranscendSelectedTower()
    {
        if (selectedTower == null)
            return;

        if (towerManager.TryTranscend(selectedTower, out TowerInstance transcendTower))
        {
            selectedTower = transcendTower;
            hasSelectedCell = true;
            selectedCell = transcendTower.CellPos;
            SelectCell(selectedCell);
        }
    }

    public void ClearSelection()
    {
        hasSelectedCell = false;
        selectedTower = null;

        if (actionPanelUI != null)
            actionPanelUI.Hide();
    }
}
