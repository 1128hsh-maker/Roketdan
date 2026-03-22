using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    public TowerData Data { get; private set; }
    public Vector2Int CellPos { get; private set; }

    public void Initialize(TowerData data, Vector2Int cellPos)
    {
        Data = data;
        CellPos = cellPos;

        if (data != null)
        {
            gameObject.name = $"{data.towerId}_G{data.grade}_{cellPos.x}_{cellPos.y}";
        }
    }
}