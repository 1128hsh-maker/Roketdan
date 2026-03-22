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

        ClearChildren();

        for (int y = 0; y < boardRuntime.Rows; y++)
        {
            for (int x = 0; x < boardRuntime.Columns; x++)
            {
                CellRuntime cell = boardRuntime.GetCell(x, y);
                if (cell == null)
                    continue;

                GameObject prefab = GetPrefab(cell.cellType);
                if (prefab == null)
                    continue;

                Vector3 pos = boardGrid.GetCellCenter(x, y);
                GameObject tileObj = Instantiate(prefab, pos, Quaternion.identity, tileRoot);

                tileObj.name = $"{cell.cellType}_{x}_{y}";

                FitToCell(tileObj);
            }
        }
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

    private void ClearChildren()
    {
        Transform root = tileRoot != null ? tileRoot : transform;

        for (int i = root.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(root.GetChild(i).gameObject);
            else
                Destroy(root.GetChild(i).gameObject);
#else
            Destroy(root.GetChild(i).gameObject);
#endif
        }
    }
}