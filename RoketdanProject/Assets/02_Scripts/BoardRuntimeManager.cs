using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRuntimeManager : MonoBehaviour
{
    [SerializeField] private BoardGridByBounds boardGrid;
    [SerializeField] private StageMapData stageMapData;

    private CellRuntime[,] cells;

    public int Columns => stageMapData != null ? stageMapData.columns : 0;
    public int Rows => stageMapData != null ? stageMapData.rows : 0;
    public int DefaultUnlockCost => stageMapData != null ? stageMapData.defaultUnlockCost : 0;

    private static readonly Vector2Int[] AdjacentDirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (stageMapData == null)
        {
            Debug.LogError("[BoardRuntimeManager] StageMapData가 연결되지 않았습니다.");
            return;
        }

        stageMapData.EnsureLayoutBuilt();

        cells = new CellRuntime[Columns, Rows];

        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                cells[x, y] = new CellRuntime
                {
                    coord = new Vector2Int(x, y),
                    cellType = stageMapData.GetCellType(x, y),
                    isAdjacentToPath = false,
                    placedTower = null
                };
            }
        }

        CalculatePathAdjacency();
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Columns && y >= 0 && y < Rows;
    }

    public CellRuntime GetCell(int x, int y)
    {
        if (!IsInBounds(x, y))
            return null;

        return cells[x, y];
    }

    public bool UnlockCell(int x, int y)
    {
        CellRuntime cell = GetCell(x, y);

        if (cell == null)
            return false;

        if (cell.cellType != CellType.Locked)
            return false;

        if (cell.HasTower)
            return false;

        cell.cellType = CellType.Buildable;
        return true;
    }

    public void SetPlacedTower(int x, int y, TowerInstance tower)
    {
        CellRuntime cell = GetCell(x, y);
        if (cell == null)
            return;

        cell.placedTower = tower;
    }

    public void ClearPlacedTower(int x, int y)
    {
        CellRuntime cell = GetCell(x, y);
        if (cell == null)
            return;

        cell.placedTower = null;
    }

    public List<Vector3> GetEnemyPathWorldPositions()
    {
        List<Vector3> result = new List<Vector3>();

        if (stageMapData == null || boardGrid == null)
            return result;

        for (int i = 0; i < stageMapData.enemyPath.Count; i++)
        {
            Vector2Int coord = stageMapData.enemyPath[i];

            if (!IsInBounds(coord.x, coord.y))
                continue;

            result.Add(boardGrid.GetCellCenter(coord.x, coord.y));
        }

        return result;
    }

    private void CalculatePathAdjacency()
    {
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                CellRuntime cell = cells[x, y];
                cell.isAdjacentToPath = false;

                if (cell.cellType != CellType.Buildable && cell.cellType != CellType.Locked)
                    continue;

                for (int i = 0; i < AdjacentDirs.Length; i++)
                {
                    Vector2Int next = new Vector2Int(x, y) + AdjacentDirs[i];

                    if (!IsInBounds(next.x, next.y))
                        continue;

                    if (cells[next.x, next.y].cellType == CellType.Path)
                    {
                        cell.isAdjacentToPath = true;
                        break;
                    }
                }
            }
        }
    }
}