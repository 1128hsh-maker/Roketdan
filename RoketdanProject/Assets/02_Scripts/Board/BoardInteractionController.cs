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
    [SerializeField] private HeroManager heroManager;
    [SerializeField] private ActionPanelUI actionPanelUI;
    [SerializeField] private BoardVisualBuilder boardVisualBuilder;
    [SerializeField] private TranscendPanelUI transcendPanelUI;

    private bool hasSelectedCell;
    private Vector2Int selectedCell;
    private HeroInstance selectedHero;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (actionPanelUI != null)
            actionPanelUI.Hide();

        if (transcendPanelUI != null)
            transcendPanelUI.Hide();
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
        selectedHero = null;

        if (transcendPanelUI != null)
            transcendPanelUI.Hide();

        CellRuntime cell = boardRuntime.GetCell(cellPos.x, cellPos.y);
        if (cell == null)
        {
            ClearSelection();
            return;
        }

        Vector3 cellWorldPos = boardGrid.GetCellCenter(cellPos.x, cellPos.y);

        if (cell.cellType == CellType.Blocked || cell.cellType == CellType.Path)
        {
            actionPanelUI.Hide();
            return;
        }

        if (cell.cellType == CellType.Locked && !cell.HasHero)
        {
            actionPanelUI.ShowUnlock(cellPos, boardRuntime.DefaultUnlockCost, cellWorldPos);
            return;
        }

        if (cell.cellType == CellType.Buildable && !cell.HasHero)
        {
            HeroData summonData = heroManager.GetSummonDataForCell(cell);
            actionPanelUI.ShowBuild(cellPos, summonData != null ? summonData.summonCost : 0, cellWorldPos);
            return;
        }

        if (cell.HasHero)
        {
            selectedHero = cell.placedHero;

            bool canPromote = heroManager.CanPromote(selectedHero);
            bool canTranscend = heroManager.CanTranscend(selectedHero);
            int promoteCost = heroManager.GetPromoteCost(selectedHero);
            int transcendCost = heroManager.GetTranscendCost(selectedHero);

            actionPanelUI.ShowHeroActions(
                selectedHero,
                canPromote,
                canTranscend,
                promoteCost,
                transcendCost,
                selectedHero.transform.position
            );
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
            if (boardVisualBuilder != null)
                boardVisualBuilder.RefreshCell(selectedCell.x, selectedCell.y);

            SelectCell(selectedCell);
        }
    }

    public void OnClickBuildSelectedCell()
    {
        if (!hasSelectedCell)
            return;

        if (heroManager.TrySummonDefault(selectedCell, out HeroInstance summonedHero))
        {
            selectedHero = summonedHero;
            SelectCell(selectedCell);
        }
    }

    public void OnClickPromoteSelectedHero()
    {
        if (selectedHero == null)
            return;

        if (heroManager.TryPromote(selectedHero, out HeroInstance promotedHero))
        {
            selectedHero = promotedHero;
            hasSelectedCell = true;
            selectedCell = promotedHero.CellPos;
            SelectCell(selectedCell);
        }
    }

    public void OnClickOpenTranscendPanel()
    {
        if (selectedHero == null)
            return;

        if (!heroManager.CanTranscend(selectedHero))
            return;

        if (transcendPanelUI == null)
            return;

        HeroData optionA = heroManager.GetTranscendOption(selectedHero, 0);
        HeroData optionB = heroManager.GetTranscendOption(selectedHero, 1);
        HeroData optionC = heroManager.GetTranscendOption(selectedHero, 2);
        int cost = heroManager.GetTranscendCost(selectedHero);

        transcendPanelUI.Show(selectedHero, optionA, optionB, optionC, cost);
    }

    public void OnClickTranscendOptionA()
    {
        TryTranscendOption(0);
    }

    public void OnClickTranscendOptionB()
    {
        TryTranscendOption(1);
    }

    public void OnClickTranscendOptionC()
    {
        TryTranscendOption(2);
    }

    private void TryTranscendOption(int optionIndex)
    {
        if (selectedHero == null)
            return;

        if (heroManager.TryTranscend(selectedHero, optionIndex, out HeroInstance transcendedHero))
        {
            if (transcendPanelUI != null)
                transcendPanelUI.Hide();

            selectedHero = transcendedHero;
            hasSelectedCell = true;
            selectedCell = transcendedHero.CellPos;
            SelectCell(selectedCell);
        }
    }

    public void ClearSelection()
    {
        hasSelectedCell = false;
        selectedHero = null;

        if (actionPanelUI != null)
            actionPanelUI.Hide();

        if (transcendPanelUI != null)
            transcendPanelUI.Hide();
    }
}