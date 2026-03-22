using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellRuntime
{
    public Vector2Int coord;
    public CellType cellType;
    public bool isAdjacentToPath;
    public TowerInstance placedTower;

    public bool HasTower => placedTower != null;

    public bool IsEmptyBuildable
    {
        get
        {
            return cellType == CellType.Buildable && placedTower == null;
        }
    }

    public bool IsEmptyLocked
    {
        get
        {
            return cellType == CellType.Locked && placedTower == null;
        }
    }
}