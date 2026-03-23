using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellRuntime
{
    public Vector2Int coord;
    public CellType cellType;
    public bool isAdjacentToPath;
    public HeroInstance placedHero;

    public bool HasHero => placedHero != null;

    public bool IsEmptyBuildable
    {
        get
        {
            return cellType == CellType.Buildable && placedHero == null;
        }
    }

    public bool IsEmptyLocked
    {
        get
        {
            return cellType == CellType.Locked && placedHero == null;
        }
    }
}