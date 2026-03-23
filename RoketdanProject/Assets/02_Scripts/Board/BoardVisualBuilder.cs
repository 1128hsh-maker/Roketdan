using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardVisualBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardGridByBounds boardGrid;
    [SerializeField] private BoardRuntimeManager boardRuntime;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject blockedTilePrefab;
    [SerializeField] private GameObject buildableTilePrefab;
    [SerializeField] private GameObject lockedTilePrefab;
    [SerializeField] private GameObject pathTilePrefab;

    [Header("Parent")]
    [SerializeField] private Transform tileRoot;

    [Header("Build Options")]
    [SerializeField] private bool rebuildOnStart = true;

    private GameObject[,] spawnedTiles;

    private void Start()
    {
        if (rebuildOnStart)
        {
            Rebuild();
        }
    }

    [ContextMenu("Rebuild Visuals")]
    public void Rebuild()
    {
        if (boardGrid == null || boardRuntime == null)
        {
            Debug.LogError("[BoardVisualBuilder] boardGrid 또는 boardRuntime이 연결되지 않았습니다.");
            return;
        }

        if (tileRoot == null)
        {
            Debug.LogError("[BoardVisualBuilder] tileRoot가 연결되지 않았습니다.");
            return;
        }

        if (!tileRoot.gameObject.activeSelf)
            tileRoot.gameObject.SetActive(true);

        ClearAllTiles();

        spawnedTiles = new GameObject[boardRuntime.Columns, boardRuntime.Rows];

        for (int y = 0; y < boardRuntime.Rows; y++)
        {
            for (int x = 0; x < boardRuntime.Columns; x++)
            {
                CreateOrRefreshCellVisual(x, y);
            }
        }

        Debug.Log("[BoardVisualBuilder] 전체 맵 타일 재생성 완료");
    }

    public void RefreshCell(int x, int y)
    {
        if (boardGrid == null || boardRuntime == null)
        {
            Debug.LogError("[BoardVisualBuilder] boardGrid 또는 boardRuntime이 연결되지 않았습니다.");
            return;
        }

        if (tileRoot == null)
        {
            Debug.LogError("[BoardVisualBuilder] tileRoot가 연결되지 않았습니다.");
            return;
        }

        if (!boardRuntime.IsInBounds(x, y))
        {
            Debug.LogWarning($"[BoardVisualBuilder] RefreshCell 범위 밖 좌표: ({x}, {y})");
            return;
        }

        if (spawnedTiles == null ||
            spawnedTiles.GetLength(0) != boardRuntime.Columns ||
            spawnedTiles.GetLength(1) != boardRuntime.Rows)
        {
            spawnedTiles = new GameObject[boardRuntime.Columns, boardRuntime.Rows];
        }

        if (spawnedTiles[x, y] != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(spawnedTiles[x, y]);
            else
                Destroy(spawnedTiles[x, y]);
#else
            Destroy(spawnedTiles[x, y]);
#endif
            spawnedTiles[x, y] = null;
        }

        CreateOrRefreshCellVisual(x, y);

        Debug.Log($"[BoardVisualBuilder] 셀 갱신 완료: ({x}, {y})");
    }

    private void CreateOrRefreshCellVisual(int x, int y)
    {
        CellRuntime cell = boardRuntime.GetCell(x, y);
        if (cell == null)
            return;

        GameObject prefab = GetPrefab(cell.cellType);
        if (prefab == null)
        {
            Debug.LogWarning($"[BoardVisualBuilder] {cell.cellType}에 해당하는 프리팹이 없습니다. ({x}, {y})");
            return;
        }

        Vector3 pos = boardGrid.GetCellCenter(x, y);
        GameObject tileObj = Instantiate(prefab, pos, Quaternion.identity, tileRoot);

        tileObj.name = $"{cell.cellType}_{x}_{y}";
        FitToCell(tileObj);

        spawnedTiles[x, y] = tileObj;
    }

    private GameObject GetPrefab(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Blocked:
                return blockedTilePrefab;
            case CellType.Buildable:
                return buildableTilePrefab;
            case CellType.Locked:
                return lockedTilePrefab;
            case CellType.Path:
                return pathTilePrefab;
            default:
                return null;
        }
    }

    private void FitToCell(GameObject tileObj)
    {
        SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
            return;

        Vector2 cellSize = boardGrid.GetCellSize();
        Vector2 spriteSize = sr.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        tileObj.transform.localScale = new Vector3(
            cellSize.x / spriteSize.x,
            cellSize.y / spriteSize.y,
            1f
        );
    }

    private void ClearAllTiles()
    {
        if (tileRoot == null)
            return;

        for (int i = tileRoot.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(tileRoot.GetChild(i).gameObject);
            else
                Destroy(tileRoot.GetChild(i).gameObject);
#else
            Destroy(tileRoot.GetChild(i).gameObject);
#endif
        }
    }
}